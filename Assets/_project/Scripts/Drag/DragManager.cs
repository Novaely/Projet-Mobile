using UnityEngine;

public class DragManager : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] private ConditionManager conditionManager;

    DinoController _currentItemSelected;
    EmplacementController _lastEmplacement;
    private Seat _lastHoveredSeat;

    private void Start()
    {
        if (conditionManager == null) conditionManager = FindFirstObjectByType<ConditionManager>();
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
        if (Input.touchCount > 0)
        {
            Touch touch0 = Input.GetTouch(0);
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(touch0.position);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Debug.Log("Start Drag");
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
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (_currentItemSelected == null) { return; }

                Debug.Log("bouge");
                _currentItemSelected.Displace();

                Vector2 mousePosPlacement = Camera.main.ScreenToWorldPoint(touch0.position);
                RaycastHit2D hitPlacement = Physics2D.Raycast(mousePosPlacement, Vector2.zero, 10000, layerMask);

                Seat currentSeat = null;
                if (hitPlacement && hitPlacement.transform.TryGetComponent(out Seat s))
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
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Debug.Log("end drag");
                if (_currentItemSelected == null) { return; }

                if (_lastHoveredSeat != null)
                {
                    ResetSeatColor(_lastHoveredSeat);
                    _lastHoveredSeat = null;
                }

                bool dropSuccess = false;

                Vector2 mousePosPlacement = Camera.main.ScreenToWorldPoint(touch0.position);
                RaycastHit2D hitPlacement = Physics2D.Raycast(mousePosPlacement, Vector2.zero, 10000, layerMask);

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

                            var evaluator = seatScript.GetComponent<SeatEvaluator>();
                            if (evaluator != null) evaluator.Evaluate(dinoScript);

                            var scorer = FindObjectOfType<LevelScorer>();   
                            if (scorer != null) scorer.UpdateScore();

                            if (_lastEmplacement != null)
                            {
                                _lastEmplacement.Storage = null;
                                if (_lastEmplacement.TryGetComponent(out Seat oldSeat))
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
        }
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
