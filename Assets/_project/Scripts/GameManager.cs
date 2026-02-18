using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] AudioClip MainMenuMusic;
    [SerializeField] AudioClip[] LevelMusics;

    public enum GameStates
    {
        Init,
        Menu,
        Tuto,
        Play,
        Pause,
        EndLevel,
    }

    public GameStates GameState { get; private set; }

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

        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnPauseActive += PauseActive;
            UIManager.Instance.OnPauseDesactive += PauseDesactive;
        }
    }

    void Update()
    {
        switch (GameState)
        {
            case GameStates.Init:
                if (PlayerSave.Instance.IsSaveLoad) { MenuLoad(); }
                break;
            case GameStates.Menu:
                break;
            case GameStates.Tuto:
                break;
            case (GameStates.Play):
                break;
            case (GameStates.Pause):
                break;
            case (GameStates.EndLevel):
                break;
        }
    }

    void MenuLoad()
    {
        GameState = GameStates.Menu;
        AudioManager.Instance.PlayMusic(MainMenuMusic);
    }

    void LevelLoad()
    {
        AudioManager.Instance.StopMusic();

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

        int r = Random.Range(0, LevelMusics.Length);
        AudioManager.Instance.PlayMusic(LevelMusics[r]);
    }

    void TutoEnd()
    {
        GameState = GameStates.Play;
    }

    void LevelEnd()
    {
        GameState = GameStates.EndLevel;
    }

    void PauseActive()
    {
        GameState = GameStates.Pause;
    }

    void PauseDesactive()
    {
        GameState = GameStates.Play;
    }
}