using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelScorer : MonoBehaviour
{
    [Header("UI Étoiles Niveau")]
    public Image[] starImages = new Image[3];
    public TextMeshProUGUI scoreText;

    [Header("Scoring")]
    public int requiredPerfect = 3; // Combien de dinos doivent être parfaits pour 3 étoiles
    public int requiredGood = 2;    // Pour 2 étoiles

    private int perfectCount = 0;
    private int totalDinosaurs = 0;

    public void UpdateScore()
    {
        // Compte les dinos parfaitement placés
        perfectCount = 0;
        totalDinosaurs = 0;

        Seat[] allSeats = FindObjectsByType<Seat>(FindObjectsSortMode.None);
        foreach (var seat in allSeats)
        {
            if (seat.occupant != null)
            {
                totalDinosaurs++;
                var evaluator = seat.GetComponent<SeatEvaluator>();
                if (evaluator && evaluator.currentState == PlacementState.Bon)
                    perfectCount++;
            }
        }

        // Calcul étoiles
        int stars = 0;
        if (perfectCount >= requiredPerfect) stars = 3;
        else if (perfectCount >= requiredGood) stars = 2;
        else if (perfectCount > 0) stars = 1;

        // UI
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].gameObject.SetActive(i < stars);
        }

        scoreText.text = $"{perfectCount}/{totalDinosaurs} ({stars}/3)";
    }
}
