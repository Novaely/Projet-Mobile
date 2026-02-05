using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SeatEvaluator : MonoBehaviour
{
    [Header("UI References (Local)")]
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI descriptionText;
    public Image spriteDisplay;

    [Header("Visuels")]
    public Color neutreColor = Color.gray;
    public Color bonColor = Color.green;
    public Color mauvaisColor = Color.red;

    public Sprite neutreSprite;
    public Sprite bonSprite;
    public Sprite mauvaisSprite;

    private Seat seat;

    void Start()
    {
        seat = GetComponent<Seat>();
        UpdateVisuals(null);
    }

    public void UpdateFeedback(Dino dino)
    {
        if (dino != null)
        {
            dino.EvaluateSatisfaction(seat);
        }
        UpdateVisuals(dino);
    }

    void UpdateVisuals(Dino dino)
    {
        PlacementState state = dino != null ? dino.currentState : PlacementState.Neutre;

        if (stateText != null)
        {
            stateText.text = state.ToString();
            stateText.color = state == PlacementState.Bon ? bonColor : 
                             state == PlacementState.Mauvais ? mauvaisColor : neutreColor;
        }

        if (spriteDisplay != null)
        {
            spriteDisplay.sprite = state switch
            {
                PlacementState.Bon => bonSprite,
                PlacementState.Mauvais => mauvaisSprite,
                _ => neutreSprite
            };
            spriteDisplay.gameObject.SetActive(spriteDisplay.sprite != null);
        }

        if (descriptionText != null)
        {
            if (dino != null && dino.profile != null)
                descriptionText.text = dino.profile.designerDescription;
            else
                descriptionText.text = "Siège vide";
        }
        
    }
    
    public float GetCurrentSatisfaction()
    {
        if (seat != null && seat.occupant != null)
            return seat.occupant.currentSatisfaction;
        return 0f;
    }
}