using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class LoadSaveOnStart : MonoBehaviour
{
    public static LoadSaveOnStart Instance;

    [SerializeField] LevelsSave _levelData;

    public bool IsSaveLoad {  get; private set; }
    public event Action OnLoadSave;
    public event Func<LevelsSaveData> OnNoSaveFound;
    public event Func<LevelsSaveData> OnNoAllLevelFoundOnSave;

    void Awake()
    {
        DontDestroyOnLoad(this);

        if (Instance == null) { Instance = this; }

        else { Destroy(gameObject); }

        IsSaveLoad = false;
    }

    void Start()
    {
        StartCoroutine(WaitForItem());
        IEnumerator WaitForItem()
        {
            yield return new WaitForEndOfFrame();
            LoadSave();
        }
    }

    void LoadSave()
    {
        LevelsSaveData data = SaveManager.Load<LevelsSaveData>();

        int numberOfScene = Directory.GetFiles("Assets/_project/Scenes/Levels").Length / 2;

        if (data == null)
        {
            Debug.Log("Aucune sauvegarde trouvée");
            data = OnNoSaveFound?.Invoke();
        }
        else if (data.starsLevels.Count != numberOfScene)
        {
            data = OnNoAllLevelFoundOnSave?.Invoke();
        }


        _levelData.starsLevels = data.starsLevels;

        Debug.Log("Sauvegarde chargée");

        IsSaveLoad = true;

        OnLoadSave?.Invoke();
    }
}