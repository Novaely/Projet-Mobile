using UnityEngine;

public class UiStarLevel : MonoBehaviour
{
    [SerializeField] Sprite[] _starObtained;
    [SerializeField] Sprite[] _StarNotObtained;

    private void Start()
    {
        ButtonLevelInfo[] buttonLevelInfos = GetComponentsInChildren<ButtonLevelInfo>();


        foreach (var buttonLevelInfo in buttonLevelInfos)
        {

            for (int i = 0; i < 3; i++)
            {
                int nbStar = PlayerSave.Instance.LevelSave.starsLevels[buttonLevelInfo.index];
                buttonLevelInfo.stars[0].sprite = nbStar >= 1 ? _starObtained[0] : _StarNotObtained[0];
                buttonLevelInfo.stars[1].sprite = nbStar >= 2 ? _starObtained[1] : _StarNotObtained[1];
                buttonLevelInfo.stars[2].sprite = nbStar >= 3 ? _starObtained[2] : _StarNotObtained[2];
            }
        }
    }
}
