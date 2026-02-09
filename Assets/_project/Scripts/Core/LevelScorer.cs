using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class LevelScorer : MonoBehaviour
{
    [Header("UI Score")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI percentText;
    public TextMeshProUGUI starsText;
    public Image[] starImages; 
    
    [Header("UI End Level")]
    public Button validateButton;
    public GameObject resultPanel;
    
    [Header("Settings")]
    public Seat[] seats;
    public float targetSatisfaction = 0.8f;

    [Header("Filtering")]
    public LayerMask ignoreLayers; 
    
    [Header("Level Config")]
    public int totalDinosToPlace = 0; 
    
    private int currentScore = 0;
    private int maxScore = 0;
    private bool levelValidated = false;


    void Start()
    {
        InitializeLevel();

        if (validateButton != null)
        {
            validateButton.onClick.RemoveAllListeners();
            validateButton.onClick.AddListener(OnValidateClick);
            validateButton.interactable = false;
        }

        if (resultPanel != null) resultPanel.SetActive(false);

        UpdateScore();
    }

    public void InitializeLevel() 
    {
        var allSeats = FindObjectsByType<Seat>(FindObjectsSortMode.None);
        seats = allSeats.Where(s => !IsIgnored(s.gameObject)).ToArray();

        if (totalDinosToPlace > 0)
        {
            maxScore = totalDinosToPlace;
        }
        else
        {
            var allDinos = FindObjectsByType<Dino>(FindObjectsSortMode.None);
            var validDinos = allDinos.Where(d => !IsIgnored(d.gameObject)).ToArray();

            maxScore = (validDinos.Length > 0) ? validDinos.Length : seats.Length;
        }
    }

    bool IsIgnored(GameObject obj)
    {
        return (ignoreLayers.value & (1 << obj.layer)) != 0;
    }

    public void ForceUpdateScore()
    {
        if (!levelValidated) UpdateScore();
    }

    void UpdateScore() 
    {
        currentScore = 0;
        
        foreach (var seat in seats)
        {
            if (seat == null) continue;
            
            var evaluator = seat.GetComponent<SeatEvaluator>();
            if (evaluator != null && evaluator.GetCurrentSatisfaction() >= targetSatisfaction)
            {
                currentScore++;
            }
        }

        if (currentScore > maxScore) currentScore = maxScore;
        
        UpdateUI();
    }

    void UpdateUI()
    {
        float percent = GetPercent();
        
        if (scoreText != null) scoreText.text = $"Score: {currentScore}/{maxScore}";
        if (percentText != null) percentText.text = $"{percent:F0}%";
        if (starsText != null) starsText.text = $"{GetStars()}★";

        UpdateStarImages();

        if (validateButton != null)
        {
            int occupiedSeats = 0;
            foreach(var seat in seats)
            {
                if (seat != null && seat.occupant != null) occupiedSeats++;
            }

            bool isFull = (occupiedSeats >= maxScore);
            validateButton.interactable = isFull;
        }
    }

    public void OnValidateClick()
    {
        levelValidated = true;
        
        if (validateButton != null) validateButton.gameObject.SetActive(false);
        if (resultPanel != null) { resultPanel.SetActive(true);
            PlayerSave.Instance.UpdateStarOfOneLevel(GetStars());
        }
    }

    void UpdateStarImages()
    {
        if (starImages != null)
        {
            int stars = GetStars();
            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] != null) starImages[i].enabled = (i < stars);
            }
        }
    }

    float GetPercent()
    {
        if (maxScore <= 0) return 0f;
        return Mathf.Clamp((float)currentScore / maxScore * 100f, 0f, 100f); 
    }

    int GetStars()
    {
        float p = GetPercent();
        if (p >= 90f) return 3;
        if (p >= 60f) return 2;
        if (p >= 30f) return 1;
        return 0;
    }
}
