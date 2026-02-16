using UnityEngine;
using TMPro;

public class SeatEvaluator : MonoBehaviour
{
    [Header("Configuration Rendu")]
    public SpriteRenderer visualRenderer; // Glisse ici l'enfant "VisuelBuche"
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

    void Awake()
    {
        _seat = GetComponent<Seat>();
        
        // Si tu n'as rien glissé dans l'inspecteur, on ne fait rien par sécurité
        if (visualRenderer != null && spriteBuche != null)
        {
            visualRenderer.sprite = spriteBuche;
        }
    }

    void Start()
    {
        ResetVisuals();
    }

    public void UpdateFeedback(Dino dino)
    {
        if (dino != null)
        {
            dino.EvaluateSatisfaction(_seat);
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
            bool estVraimentLibre = (_seat.occupant == null || _seat.occupant == dino);
            visualRenderer.color = estVraimentLibre ? colorSiegeLibre : colorSiegeOccupe;
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

        if (descriptionText != null && dino.profile != null)
        {
            descriptionText.text = $"<color=green>Aime : {dino.profile.positiveCondition}</color>\n" +
                                 $"<color=red>Déteste : {dino.profile.negativeCondition}</color>";
        }
    }

    public void ResetVisuals()
    {
        if (visualRenderer != null)
        {
            // Retour à la couleur vide (souvent blanc ou gris transparent)
            visualRenderer.color = (_seat != null && _seat.occupant != null) ? colorSiegeOccupe : colorSiegeVide;
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
            return _seat.occupant.currentSatisfaction;
        return 0f;
    }
}