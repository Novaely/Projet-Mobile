using UnityEngine;
using System.Collections;

public enum PlacementState
{
    Neutre,
    Bon,
    Mauvais
}

public class Dino : MonoBehaviour
{
    [Header("--- DATA ---")]
    public DinoProfileSO profile;
    public string dinoName => profile ? profile.speciesName : "Unknown";
    public DietType diet => profile ? profile.diet : DietType.Herbivore;

    [Header("--- VISUEL PASSIF (Spawn & Drag) ---")]
    public Sprite passiveSprite; 

    [Header("--- VISUEL ACTIF (Posé en Jeu) ---")]
    public Sprite idleSprite;   
    public Sprite happySprite;  
    public Sprite angrySprite;  

    [Header("--- VISUEL BULLES ---")]
    public SpriteRenderer emoteRenderer; 
    public Sprite bubbleHappy;  
    public Sprite bubbleAngry;  

    [Header("--- ETAT ---")]
    public PlacementState currentState = PlacementState.Neutre;
    public float currentSatisfaction = 0f;

    private SpriteRenderer _renderer;
    private Coroutine _currentAnim;
    private Vector3 _originalScale;
    private Transform _lastPosition;
    public Transform LastPosition => _lastPosition;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _originalScale = transform.localScale;
        
        if (emoteRenderer != null) 
            emoteRenderer.transform.localScale = Vector3.zero;

        SetVisualMode(false); 
    }
    
    public void SetSlotPosition(Transform slotTransform)
    {
        _lastPosition = slotTransform;
        transform.position = slotTransform.position;
    }

    public void ReturnToLastPosition()
    {
        if (_lastPosition != null)
        {
            transform.position = _lastPosition.position;
            if (_lastPosition.TryGetComponent(out Seat seat))
            {
                EvaluateSatisfaction(seat);
            }
        }
    }

    public void SetDragging(bool isDragging)
    {
        if (isDragging)
        {
            SetVisualMode(false);
        }
    }

    public void EvaluateSatisfaction(Seat seat)
    {
        if (seat == null) return;

        int spawnLayerIndex = LayerMask.NameToLayer("Spawn");
        
        if (seat.gameObject.layer == spawnLayerIndex)
        {
            SetVisualMode(false);
            currentState = PlacementState.Neutre;
            currentSatisfaction = 0f;
            return;
        }
        
        if (profile == null || profile.myRules == null || profile.myRules.Count == 0) 
        { 
            currentState = PlacementState.Bon;
            currentSatisfaction = 1f;
            return; 
        }

        int satisfiedRules = 0;
        foreach (var rule in profile.myRules)
        {
            if (rule != null && rule.IsSatisfied(this, seat)) satisfiedRules++;
        }

        float ratio = (float)satisfiedRules / profile.myRules.Count;
        currentSatisfaction = ratio;

        if (ratio >= 0.8f) currentState = PlacementState.Bon;
        else if (ratio <= 0.3f) currentState = PlacementState.Mauvais;
        else currentState = PlacementState.Neutre;
    }

    private void SetVisualMode(bool isActiveMode)
    {
        if (_currentAnim != null) StopCoroutine(_currentAnim);
        
        transform.localScale = _originalScale;
        transform.rotation = Quaternion.identity;

        if (!isActiveMode)
        {
            if (passiveSprite != null) _renderer.sprite = passiveSprite;
            else if (idleSprite != null) _renderer.sprite = idleSprite;

            if (emoteRenderer != null) emoteRenderer.gameObject.SetActive(false);
        }
    }

    public void PlayPlacementAnimation()
    {
        PlayActiveAnimation(currentState);
    }

    private void PlayActiveAnimation(PlacementState state)
    {
        if (_currentAnim != null) StopCoroutine(_currentAnim);
        
        if (_lastPosition != null && Vector3.Distance(transform.position, _lastPosition.position) < 0.5f)
             transform.position = _lastPosition.position;

        switch (state)
        {
            case PlacementState.Bon:
                _renderer.sprite = happySprite ? happySprite : idleSprite;
                UpdateBubbleVisuals(state);
                _currentAnim = StartCoroutine(AnimHappyJump());
                break;

            case PlacementState.Mauvais:
                _renderer.sprite = angrySprite ? angrySprite : idleSprite;
                UpdateBubbleVisuals(state);
                _currentAnim = StartCoroutine(AnimAngryShake());
                break;

            case PlacementState.Neutre:
            default:
                _renderer.sprite = idleSprite;
                UpdateBubbleVisuals(state);
                break;
        }
    }

    private void UpdateBubbleVisuals(PlacementState state)
    {
        if (emoteRenderer == null) return;

        switch (state)
        {
            case PlacementState.Bon:
                if (bubbleHappy != null)
                {
                    emoteRenderer.sprite = bubbleHappy;
                    StartCoroutine(PopInBubble());
                }
                break;
            case PlacementState.Mauvais:
                if (bubbleAngry != null)
                {
                    emoteRenderer.sprite = bubbleAngry;
                    StartCoroutine(PopInBubble());
                }
                break;
            case PlacementState.Neutre:
            default:
                StartCoroutine(PopOutBubble());
                break;
        }
    }

    private IEnumerator AnimHappyJump()
    {
        float duration = 0.4f;
        float timer = 0f;
        Vector3 startScale = _originalScale;
        Vector3 peakScale = _originalScale * 1.3f;
        while (timer < duration) {
            timer += Time.deltaTime;
            float progress = timer / duration;
            float curve = Mathf.Sin(progress * Mathf.PI); 
            transform.localScale = Vector3.Lerp(startScale, peakScale, curve);
            if (_lastPosition != null) transform.position = _lastPosition.position + new Vector3(0, curve * 0.5f, 0);
            yield return null;
        }
        transform.localScale = startScale;
        if (_lastPosition != null) transform.position = _lastPosition.position;
    }

    private IEnumerator AnimAngryShake()
    {
        float duration = 0.4f;
        float timer = 0f;
        Vector3 centerPos = transform.position;
        while (timer < duration) {
            timer += Time.deltaTime;
            float x = Mathf.Sin(timer * 30f) * 0.15f; 
            transform.position = centerPos + new Vector3(x, 0, 0);
            float z = Mathf.Cos(timer * 20f) * 5f;
            transform.rotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }
        transform.position = centerPos;
        transform.rotation = Quaternion.identity;
    }

    private IEnumerator PopInBubble()
    {
        emoteRenderer.gameObject.SetActive(true);
        Transform t = emoteRenderer.transform;
        float timer = 0f;
        float duration = 0.3f;
        while (timer < duration) {
            timer += Time.deltaTime;
            float p = timer / duration;
            float scale = Mathf.Min(Mathf.Sin(p * Mathf.PI * 0.8f) * 1.2f, 1f); 
            t.localScale = Vector3.one * scale;
            yield return null;
        }
        t.localScale = Vector3.one;
    }

    private IEnumerator PopOutBubble()
    {
        Transform t = emoteRenderer.transform;
        if (t.localScale.x <= 0) {
            emoteRenderer.gameObject.SetActive(false);
            yield break;
        }
        float startScale = t.localScale.x;
        float timer = 0f;
        float duration = 0.15f;
        while (timer < duration) {
            timer += Time.deltaTime;
            float p = timer / duration;
            t.localScale = Vector3.one * Mathf.Lerp(startScale, 0f, p);
            yield return null;
        }
        t.localScale = Vector3.zero;
        emoteRenderer.gameObject.SetActive(false);
    }
}