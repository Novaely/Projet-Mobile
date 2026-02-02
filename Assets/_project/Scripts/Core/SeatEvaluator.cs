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
    
    [Header("Image Sprite Display")]
    public Image spriteDisplay;

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

    private Seat seat;
    private Dino currentDino;
    public PlacementState currentState = PlacementState.Neutre;

    void Start()
    {
        seat = GetComponent<Seat>();
        if (validateButton != null) 
            validateButton.onClick.AddListener(OnValidate);
        UpdateVisual();
    }

    public void Evaluate(Dino dino)
    {
        currentDino = dino;
        if (dino != null)
        {
            currentState = EvaluateState(dino);
        }
        else
        {
            currentState = PlacementState.Neutre;
        }
        UpdateVisual();
    }

    PlacementState EvaluateState(Dino dino)
    {
        // 1. Pas de dino ou pas de profil -> Neutre
        if (dino == null || dino.profile == null) 
            return PlacementState.Neutre;

        // 2. 🔥 CORRECTION : Si 0 règles -> C'est automatiquement BON (100% content)
        if (dino.profile.myRules == null || dino.profile.myRules.Count == 0) 
            return PlacementState.Bon;

        // 3. Calcul pour les dinos AVEC règles
        int satisfiedRules = 0;
        int totalRules = dino.profile.myRules.Count;

        foreach (var rule in dino.profile.myRules)
        {
            if (rule != null && rule.IsSatisfied(dino, seat))
                satisfiedRules++;
        }

        float ratio = (float)satisfiedRules / totalRules;
        
        if (ratio >= 0.8f) return PlacementState.Bon;
        if (ratio <= 0.3f) return PlacementState.Mauvais;
        return PlacementState.Neutre;
    }

    public float GetSatisfactionRatio()
    {
        // 1. Si siège vide -> 0 point
        if (currentDino == null || currentDino.profile == null)
            return 0f;

        // 2. 🔥 CORRECTION : Si 0 règles -> 1.0 (100%)
        if (currentDino.profile.myRules == null || currentDino.profile.myRules.Count == 0)
            return 1f;

        // 3. Calcul normal
        int satisfiedRules = 0;
        int totalRules = currentDino.profile.myRules.Count;

        foreach (var rule in currentDino.profile.myRules)
        {
            if (rule != null && rule.IsSatisfied(currentDino, seat))
                satisfiedRules++;
        }

        return (float)satisfiedRules / totalRules;
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
        if (descriptionText != null)
        {
            if (currentDino != null && currentDino.profile != null)
            {
                descriptionText.text = string.IsNullOrEmpty(currentDino.profile.designerDescription) ? 
                    "Aucune description" : currentDino.profile.designerDescription;
            }
            else
            {
                descriptionText.text = "Siège vide";
            }
        }
    }

    void UpdateButton()
    {
        if (validateButton != null)
        {
            // Le bouton n'est cliquable que si le placement est Bon
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
            // Affiche l'image seulement si on a un sprite assigné
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
            Debug.Log($"✅ {seat.name} validé ! Dino: {currentDino?.name}");
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