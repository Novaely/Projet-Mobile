using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor;

public class PlayerSave : MonoBehaviour
{
    public static PlayerSave Instance;

    [SerializeField] LevelsSave _levelData;

    int _currentLevel;

    void Awake()
    {
        DontDestroyOnLoad(this);

        if (Instance == null) { Instance = this; }

        else { Destroy(gameObject); }
    }

    private void Start()
    {
        LoadSaveOnStart.Instance.OnNoSaveFound += CreateNewSave;
        LoadSaveOnStart.Instance.OnNoAllLevelFoundOnSave += UpdateNumberOfLevel;
        ScenesManager.Instance.OnSceneLoad += SettScene;
    }

    void OnApplicationQuit()
    {
        SaveManager.Save(_levelData);
        Debug.Log("Sauvegarde effectuée");
    }

    LevelsSaveData CreateNewSave()
    {
        int numberOfScene = Directory.GetFiles("Assets/_project/Scenes/Levels").Length / 2;

        LevelsSaveData data = new LevelsSaveData
        {
            starsLevels = new List<int>(new int[numberOfScene]),
        };
        SaveManager.Save(data);
        Debug.Log("Nouvelle sauvegarde crée");

        return data;
    }

    LevelsSaveData UpdateNumberOfLevel()
    {
        int numberOfScene = Directory.GetFiles("Assets/_project/Scenes/Levels").Length / 2;

        LevelsSaveData data = new LevelsSaveData
        {
            starsLevels = _levelData.starsLevels,
        };

        for (int i = data.starsLevels.Count; i < numberOfScene; i++)
        {
            data.starsLevels.Add(0);
        }

        return data;
    }

    public void UpdateStarOfOneLevel(int numberOfStar)
    {
        if (_currentLevel < 0 || _currentLevel > _levelData.starsLevels.Count) return;
        if (numberOfStar < 0 || numberOfStar > 3) return;

        if (_levelData.starsLevels[_currentLevel] >= numberOfStar) return;

        _levelData.starsLevels[_currentLevel] = numberOfStar;

        LevelsSaveData data = new LevelsSaveData
        {
            starsLevels = _levelData.starsLevels,
        };

        SaveManager.Save(data);
    }

    public void SettScene(int index)
    {
        _currentLevel = index - 1;
    }
}