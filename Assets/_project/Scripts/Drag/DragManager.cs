using UnityEngine;
using UnityEngine.InputSystem; 

public class DragManager : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] private ConditionManager conditionManager;

    DinoController _currentItemSelected;
    private Seat _lastHoveredSeat;

    private void Start()
    {
        if (conditionManager == null) 
            conditionManager = Object.FindFirstObjectByType<ConditionManager>();
        
        if(SpawnManager.Instance != null)
            SpawnManager.Instance.OnSpawn += SpawnDino;
    }

    private void OnDestroy()
    {
        if(SpawnManager.Instance != null)
            SpawnManager.Instance.OnSpawn -= SpawnDino;
    }

    private void Update()
    {
        // --- 1. DÉTECTION DES ENTRÉES (CORRIGÉE) ---
        bool pressedThisFrame = false;
        bool releasedThisFrame = false;
        bool isHolding = false;
        Vector2 screenPos = Vector2.zero;

        // A. SOURIS (PC)
        if (Mouse.current != null)
        {
            // On lit la position de la souris en priorité si elle bouge
            screenPos = Mouse.current.position.ReadValue();
            
            if (Mouse.current.leftButton.wasPressedThisFrame) pressedThisFrame = true;
            if (Mouse.current.leftButton.wasReleasedThisFrame) releasedThisFrame = true;
            if (Mouse.current.leftButton.isPressed) isHolding = true;
        }

        // B. TACTILE (Mobile - Écrase la souris si détecté)
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            
            // Si on touche l'écran, on prend cette position
            if (touch.press.isPressed || touch.press.wasReleasedThisFrame)
            {
                screenPos = touch.position.ReadValue();
            }

            if (touch.press.wasPressedThisFrame) pressedThisFrame = true;
            if (touch.press.wasReleasedThisFrame) releasedThisFrame = true;
            if (touch.press.isPressed) isHolding = true;
        }

        // Si aucune interaction active et aucun dino sélectionné, on ne fait rien
        if (!isHolding && !pressedThisFrame && !releasedThisFrame && _currentItemSelected == null) return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        // --- 2. LOGIQUE DE JEU ---

        // ÉTAPE 1 : CLIC (RAMASSER)
        if (pressedThisFrame)
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit)
            {
                if (hit.transform.TryGetComponent(out DinoController item))
                {
                    _currentItemSelected = item;
                    
                    // Libérer l'ancien siège
                    if (_currentItemSelected.LastPosition != null && 
                        _currentItemSelected.LastPosition.TryGetComponent(out EmplacementController oldEmplacement))
                    {
                        if (oldEmplacement.GetComponent<Seat>() is Seat oldSeat)
                        {
                            conditionManager.PickupDino(oldSeat);
                        }
                        oldEmplacement.Storage = null;
                    }
                }
            }
        }
        // ÉTAPE 2 : DRAG (DÉPLACER - TANT QU'ON TIENT)
        else if (isHolding && _currentItemSelected != null)
        {
            _currentItemSelected.transform.position = worldPos;

            // Feedback Siège
            RaycastHit2D hitPlacement = Physics2D.Raycast(worldPos, Vector2.zero, 100f, layerMask);
            Seat currentSeat = (hitPlacement && hitPlacement.transform.TryGetComponent(out Seat s)) ? s : null;

            if (_lastHoveredSeat != currentSeat)
            {
                if (_lastHoveredSeat != null) ResetSeatColor(_lastHoveredSeat);
                _lastHoveredSeat = currentSeat;
            }

            if (currentSeat != null)
            {
                Dino dinoScript = _currentItemSelected.GetComponent<Dino>();
                if (dinoScript != null)
                {
                    Color color = conditionManager.GetHighlightColor(dinoScript, currentSeat);
                    SetSeatColor(currentSeat, color);
                }
            }
        }
        // ÉTAPE 3 : DROP (RELÂCHER - ESSENTIEL !)
        else if ((releasedThisFrame || !isHolding) && _currentItemSelected != null)
        {
            // Note : !isHolding est une sécurité supplémentaire si le release est raté
            
            if (_lastHoveredSeat != null)
            {
                ResetSeatColor(_lastHoveredSeat);
                _lastHoveredSeat = null;
            }

            bool dropSuccess = false;
            RaycastHit2D hitPlacement = Physics2D.Raycast(worldPos, Vector2.zero, 100f, layerMask);

            if (hitPlacement)
            {
                if (hitPlacement.transform.TryGetComponent(out EmplacementController item))
                {
                    Dino dinoScript = _currentItemSelected.GetComponent<Dino>();
                    Seat seatScript = item.GetComponent<Seat>();

                    if (item.Storage == null && 
                        dinoScript != null && seatScript != null && 
                        conditionManager.DropDino(dinoScript, seatScript))
                    {
                        item.Storage = _currentItemSelected.gameObject;
                        _currentItemSelected.SetPosition(item.transform);

                        var evaluator = seatScript.GetComponent<SeatEvaluator>();
                        if (evaluator != null) evaluator.Evaluate(dinoScript);

                        var scorer = Object.FindFirstObjectByType<LevelScorer>();
                        if (scorer != null) scorer.ForceUpdateScore();

                        dropSuccess = true;
                    }
                }
            }

            if (!dropSuccess)
            {
                _currentItemSelected.ReturnToLastPosition();
            }

            // 🔥 C'est ici que le dino est "oublié" pour ne plus suivre la souris
            _currentItemSelected = null;
        }
    }

    private void SpawnDino(DinoController dino, Transform position)
    {
        if (dino == null) return;
        if (position != null && position.TryGetComponent(out EmplacementController item))
        {
            item.Storage = dino.gameObject;
            dino.SetPosition(item.transform);
            var s = item.GetComponent<Seat>();
            var d = dino.GetComponent<Dino>();
            if (s && d) conditionManager.DropDino(d, s); 
        }
    }
  
    private void SetSeatColor(Seat seat, Color color)
    {
        var sr = seat.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = color;
    }

    private void ResetSeatColor(Seat seat)
    {
        var sr = seat.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(1f, 1f, 1f, 0.3f); 
    }
}