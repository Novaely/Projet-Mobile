using UnityEngine;

public class UiStarLevel : MonoBehaviour
{
    private void OnEnable()
    {
        ButtonLevelInfo[] buttonLevelInfos = GetComponentsInChildren<ButtonLevelInfo>();

        foreach (var buttonLevelInfo in buttonLevelInfos)
        {
            for (int i = 0; i < 3; i++)
            {
                switch (PlayerSave.Instance.LevelSave.starsLevels[buttonLevelInfo.index])
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
