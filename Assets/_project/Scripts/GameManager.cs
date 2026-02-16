using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameStates
    {
        Init,
        Menu,
        Tuto,
        Play,
        EndLevel,
    }

    public GameStates GameState {  get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        GameState = GameStates.Init;

        if (ScenesManager.Instance != null)
        {
            ScenesManager.Instance.OnMenuLoad += MenuLoad;
            ScenesManager.Instance.OnLevelLoad += LevelLoad;
        }

        if (PlayerSave.Instance != null)
        {
            PlayerSave.Instance.OnLevelEnd += LevelEnd;
        }
    }

    void Update()
    {
        switch (GameState) {
            case GameStates.Init:
                if (PlayerSave.Instance.IsSaveLoad) { GameState = GameStates.Menu; }
                break;
            case GameStates.Menu:
                break;
            case GameStates.Tuto:
                break;
            case (GameStates.Play):
                break;
            case (GameStates.EndLevel):
                break;
        }
    }

    void MenuLoad()
    {
        GameState = GameStates.Menu;
    }

    void LevelLoad()
    {
        TutoManager tutoManager = FindFirstObjectByType<TutoManager>();

        if (tutoManager != null)
        {
            GameState = GameStates.Tuto;

            tutoManager.OnTutoEnd += TutoEnd;
        }

        else
        {
            GameState = GameStates.Play;
        }
    }

    void TutoEnd()
    {
        GameState = GameStates.Play;
    }

    void LevelEnd()
    {
        GameState = GameStates.EndLevel;
    }
}
