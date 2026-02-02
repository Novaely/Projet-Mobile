using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void LoadSceneLevel(int index)
    {
        Debug.Log("Loading Level" + index);
        SceneManager.LoadScene("Level"+index);
    }

    public void LoadSceneMainMenu()
    {
        Debug.Log("Loading MainMenu");
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadSceneMainMenu(bool isLevelSelectActive)
    {
        Debug.Log("Loading MainMenu with LevelSelect");
        SceneManager.LoadScene("MainMenu");
        FindFirstObjectByType<UIMainMenu>().IsLevelSelectActive(isLevelSelectActive);
    }
}
