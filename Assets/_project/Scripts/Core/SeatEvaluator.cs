using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PlacementState
{
    Neutre,
    Bon,
    Mauvais
}

public class SeatEvaluator : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI descriptionText;
    public Button validateButton;

    [Header("États Visuels")]
    public Color neutreColor = Color.gray;
    public Color bonColor = Color.green;
    public Color mauvaisColor = Color.red;

    [Header("Sprites/FX par État (GD)")]
    public Sprite neutreSprite;
    public Sprite bonSprite;
    public Sprite mauvaisSprite;

    [Header("FX GameObjects (optionnel)")]
    public GameObject neutreFX;
    public GameObject bonFX;
    public GameObject mauvaisFX;

    [Header("Image Target pour Sprite")]
    public Image spriteDisplay;

    private Seat seat;
    private Dino currentDino;
    public PlacementState currentState = PlacementState.Neutre;

    void Start()
    {
        seat = GetComponent<Seat>();
        if (validateButton != null) validateButton.onClick.AddListener(OnValidate);
        UpdateVisual();
    }

    public void Evaluate(Dino dino)
    {
        currentDino = dino;
        if (dino != null)
        {
            currentState = EvaluateState(dino);
        }
        UpdateVisual();
    }

    PlacementState EvaluateState(Dino dino)
    {
        if (dino == null || dino.profile == null || dino.profile.myRules == null || dino.profile.myRules.Count == 0) 
            return PlacementState.Neutre;

        int satisfiedRules = 0;
        int totalRules = dino.profile.myRules.Count;

        foreach (var rule in dino.profile.myRules)
        {
            if (rule != null && rule.IsSatisfied(dino, seat))
                satisfiedRules++;
        }

        float ratio = totalRules > 0 ? (float)satisfiedRules / totalRules : 0f;
        if (ratio >= 0.8f) return PlacementState.Bon;
        if (ratio <= 0.3f) return PlacementState.Mauvais;
        return PlacementState.Neutre;
    }

    void UpdateVisual()
    {
        UpdateStateText();
        UpdateDescription();
        UpdateButton();
        UpdateSpriteFX();
    }

    void UpdateStateText()
    {
        if (stateText != null)
        {
            stateText.text = currentState.ToString();
            stateText.color = currentState == PlacementState.Bon ? bonColor : 
                             currentState == PlacementState.Mauvais ? mauvaisColor : neutreColor;
        }
    }

    void UpdateDescription()
    {
        if (descriptionText != null && currentDino != null && currentDino.profile != null)
        {
            descriptionText.text = string.IsNullOrEmpty(currentDino.profile.designerDescription) ? 
                "Aucune description" : currentDino.profile.designerDescription;
        }
    }

    void UpdateButton()
    {
        if (validateButton != null)
        {
            validateButton.interactable = currentState == PlacementState.Bon;
        }
    }

    void UpdateSpriteFX()
    {
        if (spriteDisplay != null)
        {
            spriteDisplay.sprite = currentState switch
            {
                PlacementState.Neutre => neutreSprite,
                PlacementState.Bon => bonSprite,
                PlacementState.Mauvais => mauvaisSprite,
                _ => null
            };
            spriteDisplay.gameObject.SetActive(spriteDisplay.sprite != null);
        }

        UpdateFX();
    }

    void UpdateFX()
    {
        if (neutreFX != null) neutreFX.SetActive(currentState == PlacementState.Neutre);
        if (bonFX != null) bonFX.SetActive(currentState == PlacementState.Bon);
        if (mauvaisFX != null) mauvaisFX.SetActive(currentState == PlacementState.Mauvais);
    }

    void OnValidate()
    {
        if (currentState == PlacementState.Bon && seat != null)
        {
            Debug.Log($"✅ {seat.name} validé !");
        }
    }

    void OnDestroy()
    {
        if (validateButton != null)
        {
            validateButton.onClick.RemoveListener(OnValidate);
        }
    }
}
