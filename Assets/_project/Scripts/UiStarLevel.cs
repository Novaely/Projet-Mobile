using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UI;

public class UiStarLevel : MonoBehaviour
{
    [SerializeField] LevelsSave _levelData;

    private void OnEnable()
    {
        ButtonLevelInfo[] buttonLevelInfos = GetComponentsInChildren<ButtonLevelInfo>();

        foreach (var buttonLevelInfo in buttonLevelInfos)
        {
            for (int i = 0; i < 3; i++)
            {
                switch (_levelData.starsLevels[buttonLevelInfo.index])
                {
                    case 0:
                        buttonLevelInfo.stars[0].color = Color.black;
                        buttonLevelInfo.stars[1].color = Color.black;
                        buttonLevelInfo.stars[2].color = Color.black;
                        break;
                    case 1:
                        buttonLevelInfo.stars[0].color = Color.yellow;
                        buttonLevelInfo.stars[1].color = Color.black;
                        buttonLevelInfo.stars[2].color = Color.black;
                        break;
                    case 2:
                        buttonLevelInfo.stars[0].color = Color.yellow;
                        buttonLevelInfo.stars[1].color = Color.yellow;
                        buttonLevelInfo.stars[2].color = Color.black;
                        break;
                    case 3:
                        buttonLevelInfo.stars[0].color = Color.yellow;
                        buttonLevelInfo.stars[1].color = Color.yellow;
                        buttonLevelInfo.stars[2].color = Color.yellow;
                        break;
                }
            }
        }
    }
}
