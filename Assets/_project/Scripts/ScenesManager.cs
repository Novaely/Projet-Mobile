using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

    public event Action<int> OnSceneLoad; 

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
        Debug.Log("Loading  Level " + index);

        yield return SceneManager.LoadSceneAsync("Level" + index);

        UIManager.Instance.LoadUILevel();
        OnSceneLoad?.Invoke(index);
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

    public int GetLevelIndex(Scene scene)
    {
        string sceneName = scene.name;

        string levelNumber = sceneName.Replace("Level", "");

        if (int.TryParse(levelNumber, out int index))
            return index;

        Debug.LogWarning("Scene name format invalid.");
        return -1;
    }
}
