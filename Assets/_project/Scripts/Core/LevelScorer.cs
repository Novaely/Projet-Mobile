using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class LevelScorer : MonoBehaviour
{
    [Header("UI Score (Pendant le jeu)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI percentText;
    public TextMeshProUGUI starsText;
    public Image[] starImages; 
    
    [Header("UI Fin de Niveau (Interaction)")]
    public Button validateButton;      
    public GameObject resultPanel;     

    [Header("Scoring Settings")]
    public Seat[] seats;
    [Tooltip("Ratio de satisfaction minimum pour gagner le point (0.8 = 80%)")]
    public float targetSatisfaction = 0.8f;
    
    [Header("Level Config")]
    [Tooltip("Force le nombre total de dinos (Score Max). Si 0, détecte automatiquement les dinos en scène.")]
    public int totalDinosToPlace = 0; 
    
    [Header("Debug")]
    public bool showDebugLogs = true;

    private int currentScore = 0;
    private int maxScore = 0;
    private float lastUpdateTime = 0f;
    private const float UPDATE_INTERVAL = 0.5f;
    
    private bool levelValidated = false;

    void Start()
    {
        InitializeLevel();

        if (validateButton != null)
        {
            validateButton.onClick.RemoveAllListeners();
            validateButton.onClick.AddListener(OnValidateClick);
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        UpdateScore();
    }

    void Update()
    {
        if (levelValidated) return;

        if (Time.time - lastUpdateTime >= UPDATE_INTERVAL)
        {
            UpdateScore();
            lastUpdateTime = Time.time;
        }
    }

    public void InitializeLevel() 
    {
        if (seats == null || seats.Length == 0)
        {
            seats = FindObjectsByType<Seat>(FindObjectsSortMode.None);
        }

        if (totalDinosToPlace > 0)
        {
            maxScore = totalDinosToPlace;
        }
        else
        {
            Dino[] dinosInScene = FindObjectsByType<Dino>(FindObjectsSortMode.None);
            
            if (dinosInScene != null && dinosInScene.Length > 0)
            {
                maxScore = dinosInScene.Length;
            }
            else
            {
                maxScore = seats.Length;
            }
        }

        if (showDebugLogs)
            Debug.Log($"📍 CONFIG NIVEAU : {seats.Length} Sièges trouvés | Objectif Score : {maxScore}");
    }

    public void OnValidateClick()
    {
        levelValidated = true; 
        UpdateScore(); 

        Debug.Log($"🏁 FIN DU NIVEAU ! Score Final : {GetPercent():F0}% ({GetStars()} étoiles)");

        if (validateButton != null) validateButton.gameObject.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(true);
    }

    public void UpdateScore() 
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
        
        UpdateUI();
    }

    void UpdateUI()
    {
        float percent = GetPercent();
        
        if (scoreText != null) scoreText.text = $"Score: {currentScore}/{maxScore}";
        if (percentText != null) percentText.text = $"{percent:F0}%";
        if (starsText != null) starsText.text = $"{GetStars()}★";

        UpdateStarImages();
    }

    void UpdateStarImages()
    {
        if (starImages != null && starImages.Length > 0)
        {
            int stars = GetStars();
            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] != null)
                    starImages[i].enabled = i < stars;
            }
        }
    }

    float GetPercent()
    {
        if (maxScore <= 0) return 0f;
        float p = (float)currentScore / maxScore * 100f;
        return Mathf.Clamp(p, 0f, 100f); 
    }

    int GetStars()
    {
        float percent = GetPercent();
        if (currentScore > 0 && percent < 30f) return 1;

        return percent switch
        {
            >= 90f => 3, 
            >= 60f => 2,
            >= 30f => 1,
            _ => 0
        };
    }

    public void InitializeSeats() => InitializeLevel();
    public void ForceUpdateScore() => UpdateScore();
    public int GetCurrentScore() => currentScore;
    public int GetMaxScore() => maxScore;
    public float GetPercentScore() => GetPercent();
    public int GetStarRating() => GetStars();
    public bool IsLevelComplete() => GetPercent() >= 60f; 
    public Image[] GetStarImages() => starImages; 
}