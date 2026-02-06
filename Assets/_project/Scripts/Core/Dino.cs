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
    public DinoColor color; 
    
    // Raccourcis
    public string dinoName => profile ? profile.speciesName : "Unknown";
    public DietType diet => profile ? profile.diet : DietType.Herbivore;

    [Header("--- VISUEL BULLES ---")]
    [Tooltip("Glisse ici l'objet enfant 'EmoteBubble'")]
    public SpriteRenderer emoteRenderer; 

    [Tooltip("Image quand il est Content")]
    public Sprite bubbleHappy;  
    [Tooltip("Image quand il est Enervé")]
    public Sprite bubbleAngry;  

    [Header("--- MOUVEMENT ---")]
    [SerializeField] private Transform _lastPosition;
    public Transform LastPosition => _lastPosition;

    [Header("--- ÉTAT ---")]
    public PlacementState currentState = PlacementState.Neutre;
    public float currentSatisfaction = 0f;

    private Coroutine _currentAnim;

    private void Awake()
    {
        // Sécurité : On cache la bulle au démarrage
        if (emoteRenderer != null) 
            emoteRenderer.transform.localScale = Vector3.zero;
    }

    // --- 1. GESTION MOUVEMENT ---
    
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
            SetState(PlacementState.Neutre, 0f);
        }
    }

    // --- 2. GESTION ÉTAT ---

    public void EvaluateSatisfaction(Seat seat)
    {
        if (seat == null || profile == null) { SetState(PlacementState.Neutre, 0f); return; }

        if (profile.myRules == null || profile.myRules.Count == 0) { SetState(PlacementState.Bon, 1f); return; }

        int satisfiedRules = 0;
        foreach (var rule in profile.myRules)
        {
            if (rule != null && rule.IsSatisfied(this, seat)) satisfiedRules++;
        }

        float ratio = (float)satisfiedRules / profile.myRules.Count;
        currentSatisfaction = ratio;

        if (ratio >= 0.8f) SetState(PlacementState.Bon, ratio);
        else if (ratio <= 0.3f) SetState(PlacementState.Mauvais, ratio);
        else SetState(PlacementState.Neutre, ratio);
    }

    private void SetState(PlacementState state, float satisfaction)
    {
        if (currentState == state) return;

        currentState = state;
        currentSatisfaction = satisfaction;

        UpdateBubbleVisuals(state);
    }

    // --- 3. ANIMATION BULLE (POP) ---

    private void UpdateBubbleVisuals(PlacementState state)
    {
        if (emoteRenderer == null) return;

        // Arrêter l'animation en cours
        if (_currentAnim != null) StopCoroutine(_currentAnim);

        switch (state)
        {
            case PlacementState.Bon:
                if (bubbleHappy != null)
                {
                    emoteRenderer.sprite = bubbleHappy;
                    _currentAnim = StartCoroutine(PopInBubble());
                }
                break;

            case PlacementState.Mauvais:
                if (bubbleAngry != null)
                {
                    emoteRenderer.sprite = bubbleAngry;
                    _currentAnim = StartCoroutine(PopInBubble());
                }
                break;

            case PlacementState.Neutre:
            default:
                // Disparaître
                _currentAnim = StartCoroutine(PopOutBubble());
                break;
        }
    }

    // Effet de "Pop" élastique (Apparition)
    private IEnumerator PopInBubble()
    {
        emoteRenderer.gameObject.SetActive(true);
        Transform t = emoteRenderer.transform;
        
        float timer = 0f;
        float duration = 0.3f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float p = timer / duration;

            // Courbe "Overshoot" (dévasse un peu 1 puis revient)
            float scale = Mathf.Sin(p * Mathf.PI * 0.8f) * 1.2f; 
            if (scale > 1f) scale = 1f; // On clippe un peu pour stabiliser

            // Version simple smooth :
            // float scale = Mathf.SmoothStep(0f, 1f, p);

            t.localScale = Vector3.one * scale;
            yield return null;
        }
        t.localScale = Vector3.one;
    }

    // Effet de disparition rapide
    private IEnumerator PopOutBubble()
    {
        Transform t = emoteRenderer.transform;
        float startScale = t.localScale.x;
        float timer = 0f;
        float duration = 0.15f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float p = timer / duration;
            t.localScale = Vector3.one * Mathf.Lerp(startScale, 0f, p);
            yield return null;
        }
        t.localScale = Vector3.zero;
        emoteRenderer.gameObject.SetActive(false);
    }
}