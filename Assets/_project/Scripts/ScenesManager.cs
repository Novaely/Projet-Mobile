using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public WorldData _worlds;

    public static ScenesManager Instance;

    public event Action<int> OnSceneLoad;

    public event Action OnMenuLoad;
    public event Action OnLevelLoad;

    public int ActiveSceneIndex { get; private set; }

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
        ActiveSceneIndex = index;

        Debug.Log("Loading  Level " + index);

        yield return SceneManager.LoadSceneAsync("Level" + index);

        UIManager.Instance.LoadUILevel();
        OnSceneLoad?.Invoke(index);
        OnLevelLoad?.Invoke();
    }

    public void LoadSceneMainMenu()
    {
        ActiveSceneIndex = 0;
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

        OnMenuLoad?.Invoke();

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

    public Scene? GetLevelScene(int index)
    {
        string sceneName = "Level" + index;

        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (scene != null)
        {
            return scene;
        }

        Debug.LogWarning("Scene index out of range.");
        return null;
    }

    public int GetStarsForNextWorld(int index)
    {
        foreach (var world in _worlds.Worlds)
        {
            index -= world.levelInThisWorld;
            if (index < 0) return -1;
            if (index == 0) return world.numberStarNeed;
        }
        return -1;
    }

    public int GetStarsObtained()
    {
        int numberStar = 0;
        foreach (var nbStarLevel in PlayerSave.Instance.LevelSave.starsLevels)
        {
            numberStar += nbStarLevel;
        }
        return numberStar;
    }
}
