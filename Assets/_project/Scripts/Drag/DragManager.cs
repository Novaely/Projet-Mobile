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
                OnDinoClicked?.Invoke(dino);
                OnDrag?.Invoke();

                _currentDino = dino;

                _currentDino.SetDragging(true);
                
                if (_currentDino.LastPosition != null && 
                    _currentDino.LastPosition.TryGetComponent(out _oldSeat))
                {
                    UpdateNeighbors(_oldSeat);
                    ForceScoreUpdate(); 
                }
            }
        }
        else if (holding && _currentDino != null)
        {
            _currentDino.transform.position = worldPos;

            RaycastHit2D hitPlace = Physics2D.Raycast(worldPos, Vector2.zero, 100f, EmplacementMask);
            Seat seat = (hitPlace && hitPlace.transform.TryGetComponent(out Seat s)) ? s : null;

            if (_lastHoveredSeat != seat)
            {
                if (_lastHoveredSeat) ResetSeatColor(_lastHoveredSeat);
                _lastHoveredSeat = seat;
            }
            if (seat)
            {
                seat.GetComponent<SpriteRenderer>().color = conditionManager.GetHighlightColor(_currentDino, seat);
            }
        }
        else if ((released || !holding) && _currentDino != null)
        {
            OnDragCanceled?.Invoke();


            if (_lastHoveredSeat) 
            {
                ResetSeatColor(_lastHoveredSeat);
                _lastHoveredSeat = null;
                _currentDino.SetDragging(false);
            }

            bool success = false;
            RaycastHit2D hitPlace = Physics2D.Raycast(worldPos, Vector2.zero, 100f, EmplacementMask);

            if (hitPlace && hitPlace.transform.TryGetComponent(out Seat seat))
            {
                if (conditionManager.DropDino(_currentDino, seat))
                {
                    conditionManager.PickupDino(_oldSeat);

                    _currentDino.SetSlotPosition(seat.transform);
                    var eval = seat.GetComponent<SeatEvaluator>();
                    if (eval) eval.UpdateFeedback(_currentDino);

                    UpdateNeighbors(seat);
                    ForceScoreUpdate();

                    success = true;
                    _oldSeat = null;
                }
            }

            if (!success) _currentDino.ReturnToLastPosition();
            
            _nextInteractTime = Time.time + antiSpamDelay;
            _currentDino = null;
        }
    }

    private void SpawnDino(Dino dino, Transform pos)
    {
        if (dino != null && pos != null && pos.TryGetComponent(out Seat seat))
        {
            dino.SetSlotPosition(seat.transform);
            conditionManager.ForceDropDino(dino, seat);
            var eval = seat.GetComponent<SeatEvaluator>();
            if (eval) eval.UpdateFeedback(dino);
            UpdateNeighbors(seat);
            ForceScoreUpdate();
        }
    }

    private void UpdateNeighbors(Seat centerSeat)
    {
        if (centerSeat == null || centerSeat.neighbors == null) return;
        foreach (var neighbor in centerSeat.neighbors)
        {
            if (neighbor != null && neighbor.occupant != null)
            {
                var eval = neighbor.GetComponent<SeatEvaluator>();
                if (eval != null) eval.UpdateFeedback(neighbor.occupant);
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
        var sr = seat.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(1f, 1f, 1f, 0.3f); 
    }
}