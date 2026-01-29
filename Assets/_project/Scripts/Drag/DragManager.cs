using UnityEngine;
using UnityEngine.InputSystem;

public class DragManager : MonoBehaviour
{
    [SerializeField] InputActionReference _dragInputAction;
    [SerializeField] LayerMask layerMask;
    [SerializeField] private ConditionManager conditionManager;

    DinoController _currentItemSelected;
    EmplacementController _lastEmplacement;
    
    private Seat _lastHoveredSeat;

    private void Start()
    {
        // Input
        _dragInputAction.action.started += DragStarted;
        _dragInputAction.action.canceled += DragCanceled;
        
        // Sécurité
        if (conditionManager == null) conditionManager = FindFirstObjectByType<ConditionManager>();
        
        //Event
        if(SpawnManager.Instance != null)
             SpawnManager.Instance.OnSpawn += SpawnDino;
    }

    private void OnDestroy()
    {
        _dragInputAction.action.started -= DragStarted;
        _dragInputAction.action.canceled -= DragCanceled;

        SpawnManager.Instance.OnSpawn -= SpawnDino;
    }

    private void Update()
    {
        if (_currentItemSelected == null) { return; }

        _currentItemSelected.Displace();
        
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 10000, layerMask);
        
        Seat currentSeat = null;
        if (hit && hit.transform.TryGetComponent(out Seat s))
            currentSeat = s;

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

    void DragStarted(InputAction.CallbackContext context)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit)
        {
            if (hit.transform.TryGetComponent(out DinoController item))
            {
                _currentItemSelected = item;
                if (_currentItemSelected.LastPosition.TryGetComponent<EmplacementController>(out _lastEmplacement))
                {
                    if (_currentItemSelected.LastPosition.TryGetComponent(out Seat seatScript))
                    {
                        conditionManager.PickupDino(seatScript);
                    }

                    _lastEmplacement.Storage = null;
                }
            }
        }
    }

    private void DragCanceled(InputAction.CallbackContext context)
    {
        if (_currentItemSelected == null) { return; }

        if (_lastHoveredSeat != null)
        {
            ResetSeatColor(_lastHoveredSeat);
            _lastHoveredSeat = null;
        }

        Vector2 mousePosPlacement = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hitPlacement = Physics2D.Raycast(mousePosPlacement, Vector2.zero, 10000, layerMask);

        bool dropSuccess = false; 

        if (hitPlacement)
        {
            Debug.Log("Raycast Hit -> " + hitPlacement.transform.name);
            if (hitPlacement.transform.TryGetComponent(out EmplacementController item))
            {
                Dino dinoScript = _currentItemSelected.GetComponent<Dino>();
                Seat seatScript = item.GetComponent<Seat>();
                
                if (item.Storage == null && 
                    (dinoScript != null && seatScript != null && conditionManager.DropDino(dinoScript, seatScript)))
                {
                    item.Storage = _currentItemSelected.gameObject;
                    _currentItemSelected.SetPosition(item.transform);
                    
                    if (_lastEmplacement != null)
                    {
                        _lastEmplacement.Storage = null;
                        
                        if(_lastEmplacement.TryGetComponent(out Seat oldSeat))
                            conditionManager.ClearSeat(oldSeat);
                            
                        _lastEmplacement = null;
                    }
                    dropSuccess = true;
                }
                else
                {
                    Debug.Log("Drop refusé : Occupé ou Règles non valides");
                }
            }
        }

        if (!dropSuccess)
        {
            _currentItemSelected.ReturnToLastPosition();
            
        }
        
        _currentItemSelected = null;
    }

    private void SpawnDino(DinoController dino, Transform position)
    {
        if (dino == null) { return; }

        if (position != null)
        {
            if (position.transform.TryGetComponent(out EmplacementController item))
            {
                item.Storage = dino.gameObject;
                dino.SetPosition(item.transform);
                
                var s = item.GetComponent<Seat>();
                var d = dino.GetComponent<Dino>();
                if (s && d) conditionManager.DropDino(d, s); 
            }
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
