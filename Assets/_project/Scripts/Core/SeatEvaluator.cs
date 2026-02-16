using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SeatEvaluator : MonoBehaviour
{
    [Header("Configuration Rendu")]
    public SpriteRenderer visualRenderer;
    public Sprite spriteBuche; 

    [Header("UI References")]
    public TextMeshProUGUI descriptionText;
    public SpriteRenderer feedbackIconRenderer; 

    [Header("Couleurs État Siège")]
    public Color colorSiegeLibre = new Color(0, 1, 0, 0.5f); 
    public Color colorSiegeOccupe = new Color(1, 0, 0, 0.5f);
    public Color colorSiegeVide = Color.white;

    [Header("Sprites Feedback")]
    public Sprite bonSprite;
    public Sprite mauvaisSprite;
    public Sprite neutreSprite;

    private Seat _seat;
    private bool _localIsDragging = false; 

    void Awake()
    {
        _seat = GetComponent<Seat>();
        
        if (visualRenderer != null && spriteBuche != null)
        {
            visualRenderer.sprite = spriteBuche;
        }
    }

    void Start()
    {
        var dm = FindFirstObjectByType<DragManager>(); //
        if (dm != null)
        {
            dm.OnDrag += HandleDragStart;
            dm.OnDragCanceled += HandleDragEnd;
        }

        ResetVisuals();
    }

    void OnDestroy()
    {
        var dm = FindFirstObjectByType<DragManager>();
        if (dm != null)
        {
            dm.OnDrag -= HandleDragStart;
            dm.OnDragCanceled -= HandleDragEnd;
        }
    }

    void HandleDragStart() => _localIsDragging = true;
    void HandleDragEnd() { _localIsDragging = false; ResetVisuals(); }

    public void UpdateFeedback(Dino dino)
    {
        if (dino != null)
        {
            dino.EvaluateSatisfaction(_seat);
            
            if (!_localIsDragging)
            {
                dino.PlayPlacementAnimation(); //
            }

            UpdateVisuals(dino);
        }
        else
        {
            ResetVisuals();
        }
    }

    void UpdateVisuals(Dino dino)
    {
        if (visualRenderer != null)
        {
            if (_localIsDragging)
            {
                bool estVraimentLibre = (_seat.occupant == null || _seat.occupant == dino);
                visualRenderer.color = estVraimentLibre ? colorSiegeLibre : colorSiegeOccupe;
            }
            else
            {
                visualRenderer.color = colorSiegeVide;
            }
        }

        if (feedbackIconRenderer != null)
        {
            feedbackIconRenderer.sprite = dino.currentState switch
            {
                PlacementState.Bon => bonSprite,
                PlacementState.Mauvais => mauvaisSprite,
                _ => neutreSprite
            };
            feedbackIconRenderer.gameObject.SetActive(true);
        }

        if (descriptionText != null)
        {
            // Texte d'aide uniquement pendant le drag
            if (_localIsDragging && dino.profile != null)
            {
                descriptionText.text = $"<color=green>Aime : {dino.profile.positiveCondition}</color>\n" +
                                     $"<color=red>Déteste : {dino.profile.negativeCondition}</color>";
            }
            else
            {
                descriptionText.text = "";
            }
        }
    }

    public void ResetVisuals()
    {
        if (visualRenderer != null)
        {
            visualRenderer.color = colorSiegeVide;
        }
        
        if (feedbackIconRenderer != null)
        {
            feedbackIconRenderer.gameObject.SetActive(false);
        }

        if (descriptionText != null)
        {
            descriptionText.text = "";
        }
    }

    public float GetCurrentSatisfaction()
    {
        if (_seat != null && _seat.occupant != null)
            return _seat.occupant.currentSatisfaction; //
        return 0f;
    }
}