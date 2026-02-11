using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class LevelScorer : MonoBehaviour
{
    [Header("Settings")]
    public Seat[] seats;
    public float targetSatisfaction = 0.8f;

    [Header("Filtering")]
    public LayerMask ignoreLayers;
    
    [Header("Level Config")]
    public int totalDinosToPlace = 0; 
    public int currentScore { get; private set; }
    public int maxScore { get; private set; }
    public bool levelValidated = false;

    private void Awake()
    {
        currentScore = 0;
        maxScore = 0;
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

        if (currentScore > maxScore) currentScore = maxScore;
        
        FindFirstObjectByType<UILevel>().UpdateUI();
    }

    public float GetPercent()
    {
        if (maxScore <= 0) return 0f;
        return Mathf.Clamp((float)currentScore / maxScore * 100f, 0f, 100f); 
    }

    public int GetStars()
    {
        float p = GetPercent();
        if (p >= 90f) return 3;
        if (p >= 60f) return 2;
        if (p >= 30f) return 1;
        return 0;
    }
}
