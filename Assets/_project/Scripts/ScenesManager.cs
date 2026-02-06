using System.Collections;
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
        StartCoroutine(LoadSceneRoutine(index));
    }

    private IEnumerator LoadSceneRoutine(int index)
    {
        Debug.Log("Loading  Level" + index);

        yield return SceneManager.LoadSceneAsync("Level" + index);

        UIManager.Instance.LoadUILevel();
    }

    public void LoadSceneMainMenu()
    {
        StartCoroutine(LoadMainMenuRoutine(null));
    }

    public void LoadSceneMainMenu(bool? isLevelSelectActive)
    {
        StartCoroutine(LoadMainMenuRoutine(isLevelSelectActive));
    }

    private IEnumerator LoadMainMenuRoutine(bool? isLevelSelectActive)
    {
        Debug.Log(isLevelSelectActive == null ? "Loading MainMenu" : "Loading MainMenu with LevelSelect");

        yield return SceneManager.LoadSceneAsync("MainMenu");

        if (isLevelSelectActive != null) {
            FindFirstObjectByType<UIMainMenu>().IsLevelSelectActive((bool)isLevelSelectActive);
        }
    }
}
