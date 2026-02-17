using System;
using UnityEngine;
using UnityEngine.InputSystem; 

public class DragManager : MonoBehaviour
{
    [Header("Configuration des Layers")]
    [SerializeField] LayerMask EmplacementMask;
    [SerializeField] LayerMask DinoMask;
    
    [Header("Références")]
    [SerializeField] private ConditionManager conditionManager;
    private Dino setdragging; 

    [Header("Sécurité")]
    [SerializeField] float antiSpamDelay = 0.15f; 

    Dino _currentDino;
    private Seat _lastHoveredSeat;
    private float _nextInteractTime = 0f;
    Seat _oldSeat;

    Vector2 _startPosition;
    bool _isDragging;

    public event Action<Dino> OnDinoClicked;
    public event Action OnDrag;
    public event Action OnDragCanceled;

    private void Start()
    {
        if (conditionManager == null) 
            conditionManager = FindFirstObjectByType<ConditionManager>();
        
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
        if (GameManager.Instance.GameState != GameManager.GameStates.Play) { return; }

        bool pressed = false, released = false, holding = false;
        Vector2 screenPos = Vector2.zero;

        if (Mouse.current != null)
        {
            screenPos = Mouse.current.position.ReadValue();
            if (Mouse.current.leftButton.wasPressedThisFrame) pressed = true;
            if (Mouse.current.leftButton.wasReleasedThisFrame) released = true;
            if (Mouse.current.leftButton.isPressed) holding = true;
        }
        if (Touchscreen.current != null)
        {
            var t = Touchscreen.current.primaryTouch;
            if (t.press.isPressed || t.press.wasReleasedThisFrame) screenPos = t.position.ReadValue();
            if (t.press.wasPressedThisFrame) pressed = true;
            if (t.press.wasReleasedThisFrame) released = true;
            if (t.press.isPressed) holding = true;
        }

        if (!holding && !pressed && !released && _currentDino == null) return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        if (pressed)
        {
            if (Time.time < _nextInteractTime) return;

            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100f, DinoMask);
            if (hit && hit.transform.TryGetComponent(out Dino dino))
            {
                _startPosition = worldPos;
                _currentDino = dino;
                OnDinoClicked?.Invoke(_currentDino);
            }
        }
        else if (holding && _currentDino != null)
        {
            if (!_isDragging && (worldPos - _startPosition).magnitude >= 0.5f)
            {
                _isDragging = true;
                OnDrag?.Invoke();
                _currentDino.SetDragging(true);

                if (_currentDino.LastPosition != null &&
                    _currentDino.LastPosition.TryGetComponent(out _oldSeat))
                {
                    ForceScoreUpdate();
                }
            }

            if (_isDragging)
            {
                _currentDino.transform.position = worldPos;

                RaycastHit2D hitPlace = Physics2D.Raycast(worldPos, Vector2.zero, 100f, EmplacementMask);
                Seat seat = (hitPlace.collider != null && hitPlace.transform.TryGetComponent(out Seat s)) ? s : null;

                if (_lastHoveredSeat != seat)
                {
                    if (_lastHoveredSeat) ResetSeatColor(_lastHoveredSeat);
                    _lastHoveredSeat = seat;
                    
                    if (_lastHoveredSeat)
                    {
                        var eval = _lastHoveredSeat.GetComponent<SeatEvaluator>();
                        if (eval) eval.UpdateFeedback(_currentDino);
                    }
                }
            }
        }
        else if ((released || !holding) && _currentDino != null)
        {
            if (_isDragging == true)
            {
                OnDragCanceled?.Invoke();
                _isDragging = false;

                if (_lastHoveredSeat)
                {
                    ResetSeatColor(_lastHoveredSeat);
                    _lastHoveredSeat = null;
                    _currentDino.SetDragging(false);
                }

                bool success = false;
                RaycastHit2D hitPlace = Physics2D.Raycast(worldPos, Vector2.zero, 100f, EmplacementMask);

                if (hitPlace.collider != null && hitPlace.transform.TryGetComponent(out Seat seat))
                {
                    if (conditionManager.DropDino(_currentDino, seat))
                    {
                        conditionManager.PickupDino(_oldSeat);
                        _currentDino.SetSlotPosition(seat.transform);
                        
                        var eval = seat.GetComponent<SeatEvaluator>();
                        if (eval) eval.UpdateFeedback(_currentDino);

                        _currentDino.PlayPlacementAnimation();

                        UpdateAllBoard(); 

                        ForceScoreUpdate();

                        success = true;
                        _oldSeat = null;
                    }
                }

                if (!success) 
                {
                    _currentDino.ReturnToLastPosition();
                    
                    if (_oldSeat != null) 
                    {
                        _oldSeat.occupant = _currentDino;
                        UpdateAllBoard();
                    }
                }

                _nextInteractTime = Time.time + antiSpamDelay;
            }

            _currentDino = null;
        }
    }

    private void SpawnDino(Dino dino, Transform pos)
    {
        if (dino != null && pos != null && pos.TryGetComponent(out Seat seat))
        {
            dino.SetSlotPosition(seat.transform);
            conditionManager.ForceDropDino(dino, seat);
        }
    }

    private void UpdateAllBoard()
    {
        Seat[] allSeats = FindObjectsByType<Seat>(FindObjectsSortMode.None);
        int spawnLayer = LayerMask.NameToLayer("Spawn");

        foreach (Seat seat in allSeats)
        {
            if (seat.isSpawnSeat || (spawnLayer != -1 && seat.gameObject.layer == spawnLayer)) 
                continue; 

            var eval = seat.GetComponent<SeatEvaluator>();

            if (seat.occupant != null)
            {
                seat.occupant.EvaluateSatisfaction(seat);

                if (eval != null) eval.UpdateFeedback(seat.occupant);

                seat.occupant.PlayPlacementAnimation();
            }
            else
            {
                if (eval != null) eval.ResetVisuals();
            }
        }
    }

    private void ForceScoreUpdate()
    {
        var scorer = FindFirstObjectByType<LevelScorer>();
        if (scorer != null) scorer.ForceUpdateScore();
    }
  
    private void ResetSeatColor(Seat seat)
    {
        var eval = seat.GetComponent<SeatEvaluator>();
        if (eval != null) eval.ResetVisuals();
    }
}