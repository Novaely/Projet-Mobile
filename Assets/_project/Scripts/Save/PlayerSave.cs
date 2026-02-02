using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class PlayerSave : MonoBehaviour
{
    [SerializeField] LevelsSave _levelData;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        LoadSaveOnStart.Instance.OnNoSaveFound += CreateNewSave;
        LoadSaveOnStart.Instance.OnNoAllLevelFoundOnSave += UpdateNumberOfLevel;
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


    [ContextMenu("Save le niveau 1 avec 3 etoiles")]
    void Save3StarAtlevel1()
    {
        UpdateStarOfOneLevel(1, 3);
    }


    void UpdateStarOfOneLevel(int level, int numberOfStar)
    {
        if (level < 0 || level > _levelData.starsLevels.Count) return;
        if (numberOfStar < 0 || numberOfStar > 3) return;

        if (_levelData.starsLevels[level] >= numberOfStar) return;

        _levelData.starsLevels[level] = numberOfStar;

        LevelsSaveData data = new LevelsSaveData
        {
            starsLevels = _levelData.starsLevels,
        };

        SaveManager.Save(data);
    }
}