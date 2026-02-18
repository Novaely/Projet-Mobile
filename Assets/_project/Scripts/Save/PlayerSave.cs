using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSave : MonoBehaviour
{
    public static PlayerSave Instance;

    [field:SerializeField] public LevelsSaveData LevelSave {  get; private set; }

    int _currentLevel;

    public int CurrentLevel { get {  return _currentLevel; } }

    public int NumberOfLevel;

    public bool IsSaveLoad { get; private set; }
    public event Action OnLoadSave;
    public event Action OnLevelEnd;

    void Awake()
    {
        DontDestroyOnLoad(this);

        if (Instance == null) { Instance = this; }

        else { Destroy(gameObject); }

        IsSaveLoad = false;

        NumberOfLevel = SceneManager.sceneCountInBuildSettings - 1;
    }

    void Start()
    {
        ScenesManager.Instance.OnSceneLoad += SettScene;

        StartCoroutine(WaitForItem());
        IEnumerator WaitForItem()
        {
            yield return new WaitForEndOfFrame();
            LoadSave();
        }
    }
    void OnApplicationQuit()
    {
        SaveManager.Save(LevelSave);
        Debug.Log("Sauvegarde effectuée");
    }

    void LoadSave()
    {
        LevelSave = SaveManager.Load<LevelsSaveData>();

        if (LevelSave == null)
        {
            Debug.Log("Aucune sauvegarde trouvée");
            LevelSave = CreateNewSave();
        }
        else if (LevelSave.starsLevels.Count != NumberOfLevel)
        {
            LevelSave = UpdateNumberOfLevel();
        }

        Debug.Log("Sauvegarde chargée");

        IsSaveLoad = true;

        OnLoadSave?.Invoke();
    }

    LevelsSaveData CreateNewSave()
    {

        LevelsSaveData data = new LevelsSaveData
        {
            starsLevels = new List<int>(new int[NumberOfLevel]),
        };

        SaveManager.Save(data);
        Debug.Log("Nouvelle sauvegarde crée");

        return data;
    }

    LevelsSaveData UpdateNumberOfLevel()
    {
        LevelsSaveData data = new LevelsSaveData
        {
            starsLevels = LevelSave.starsLevels,
        };

        for (int i = data.starsLevels.Count; i < NumberOfLevel; i++)
        {
            data.starsLevels.Add(0);
        }

        return data;
    }

    public void UpdateStarOfOneLevel(int numberOfStar)
    {
        OnLevelEnd?.Invoke();

        if (_currentLevel < 0 || _currentLevel > LevelSave.starsLevels.Count) return;
        if (numberOfStar < 0 || numberOfStar > 3) return;

        if (LevelSave.starsLevels[_currentLevel] >= numberOfStar) return;

        LevelSave.starsLevels[_currentLevel] = numberOfStar;

        SaveManager.Save(LevelSave);
    }

    void SettScene(int index)
    {
        _currentLevel = index - 1;
    }
}