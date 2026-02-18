using UnityEngine;
using System.Collections;

public enum PlacementState { Neutre, Bon, Mauvais }

public class Dino : MonoBehaviour
{
    [Header("--- DATA ---")]
    public DinoProfileSO profile;
    public string dinoName => profile ? profile.speciesName : "Unknown";
    public DietType diet => profile ? profile.diet : DietType.Herbivore;

    [Header("--- VISUEL ---")]
    public Sprite passiveSprite; 
    public Sprite idleSprite;    
    public Sprite happySprite;  
    public Sprite angrySprite;  

    [Header("--- FX ---")]
    public ParticleSystem feedbackParticles; 
    public Sprite particleHappy;  
    public Sprite particleAngry;
    public SpriteRenderer bubbleRenderer; 
    public Sprite bubbleHappySprite;      
    public Sprite bubbleAngrySprite;

    [Header("--- SFX ---")]
    [SerializeField] AudioClip _SFXBubbleEmotion;
    [SerializeField, Range(0, 10)] float _volume = 1;

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

        if (bubbleRenderer != null) bubbleRenderer.gameObject.SetActive(false);
        if (idleSprite != null) _renderer.sprite = idleSprite;
        currentState = PlacementState.Neutre;
    }
    
    public void SetSlotPosition(Transform slotTransform)
    {
        _lastPosition = slotTransform;
        transform.position = slotTransform.position;
    }

    public void ReturnToLastPosition(bool playFX = true)
    {
        if (_lastPosition != null)
        {
            transform.position = _lastPosition.position;

            if (_lastPosition.TryGetComponent(out Seat seat))
            {
                EvaluateSatisfaction(seat);
                PlayPlacementAnimation(playFX); 
            }
        }
    }

    public void SetDragging(bool isDragging)
    {
        if (isDragging)
        {
            if (_currentAnim != null) StopCoroutine(_currentAnim);
            transform.localScale = _originalScale;
            transform.rotation = Quaternion.identity;
            if (passiveSprite != null) _renderer.sprite = passiveSprite;
            if (feedbackParticles != null) feedbackParticles.Stop();
            if (bubbleRenderer != null) bubbleRenderer.gameObject.SetActive(false);
        }
    }

    public void EvaluateSatisfaction(Seat seat)
    {
        if (seat == null) return;
        
        if (seat.isSpawnSeat || seat.gameObject.layer == LayerMask.NameToLayer("Spawn"))
        {
            currentState = PlacementState.Neutre;
            currentSatisfaction = 0f;
            
            if (feedbackParticles != null) 
            {
                feedbackParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            
            if (bubbleRenderer != null) 
            {
                bubbleRenderer.gameObject.SetActive(false);
            }
            
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

    public void PlayPlacementAnimation(bool playFX = true)
    {
        PlayActiveAnimation(currentState, playFX);
    }

    private void PlayActiveAnimation(PlacementState state, bool playFX)
    {
        if (_currentAnim != null) StopCoroutine(_currentAnim);
        UpdateBubbleState(state);

        bool isSpawn = false;
        if (_lastPosition != null && _lastPosition.TryGetComponent(out Seat s)) isSpawn = IsOnSpawn(s);

        if (!isSpawn && _lastPosition != null)
        {
             transform.position = _lastPosition.position;
        }

        transform.localScale = _originalScale; 
        transform.rotation = Quaternion.identity;

        switch (state)
        {
            case PlacementState.Bon:
                _renderer.sprite = happySprite ? happySprite : idleSprite;
                if (playFX) 
                {
                    TriggerParticles(particleHappy); 
                    _currentAnim = StartCoroutine(AnimHappyJump());
                }
                break;

            case PlacementState.Mauvais:
                _renderer.sprite = angrySprite ? angrySprite : idleSprite;
                if (playFX) 
                {
                    TriggerParticles(particleAngry); 
                    _currentAnim = StartCoroutine(AnimAngryShake());
                }
                break;

            case PlacementState.Neutre:
            default:
                _renderer.sprite = idleSprite; 
                if (feedbackParticles != null) feedbackParticles.Stop();
                break;
        }
    }

    private bool IsOnSpawn(Seat seat)
    {
        return (seat.isSpawnSeat || seat.gameObject.layer == LayerMask.NameToLayer("Spawn"));
    }

    private void UpdateBubbleState(PlacementState state)
    {
        if (bubbleRenderer == null) return;
        switch (state)
        {
            case PlacementState.Bon:
                bubbleRenderer.sprite = bubbleHappySprite;
                bubbleRenderer.gameObject.SetActive(true);
                break;
            case PlacementState.Mauvais:
                bubbleRenderer.sprite = bubbleAngrySprite;
                bubbleRenderer.gameObject.SetActive(true);
                break;
            default:
                bubbleRenderer.gameObject.SetActive(false);
                break;
        }
    }

    private void TriggerParticles(Sprite spriteToPlay)
    {
        if (feedbackParticles == null || spriteToPlay == null) return;
        
        feedbackParticles.Stop();
        feedbackParticles.Clear();
        var textureModule = feedbackParticles.textureSheetAnimation;
        textureModule.enabled = true;
        textureModule.mode = ParticleSystemAnimationMode.Sprites;
        textureModule.SetSprite(0, spriteToPlay);
        feedbackParticles.Play();

        if (AudioManager.Instance != null && _SFXBubbleEmotion != null)
        {
            AudioManager.Instance.PlaySFX(_SFXBubbleEmotion,_volume);
        }
    }

    private IEnumerator AnimHappyJump()
    {
        float duration = 0.4f; float timer = 0f;
        Vector3 startScale = _originalScale; Vector3 peakScale = _originalScale * 1.3f;
        while (timer < duration) {
            timer += Time.deltaTime; float progress = timer / duration;
            float curve = Mathf.Sin(progress * Mathf.PI); 
            transform.localScale = Vector3.Lerp(startScale, peakScale, curve);
            
            if (_lastPosition != null && !IsOnSpawn(_lastPosition.GetComponent<Seat>())) 
                transform.position = _lastPosition.position + new Vector3(0, curve * 0.5f, 0);
            
            yield return null;
        }
        transform.localScale = startScale;
        if (_lastPosition != null && !IsOnSpawn(_lastPosition.GetComponent<Seat>())) 
            transform.position = _lastPosition.position;
    }

    private IEnumerator AnimAngryShake()
    {
        float duration = 0.4f; float timer = 0f;
        Vector3 centerPos = _lastPosition != null ? _lastPosition.position : transform.position;
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
}