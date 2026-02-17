using GooglePlayGames;
using GooglePlayGames.BasicApi;
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

    void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services
        }
        else
        {
            // Disable your integration with Play Games Services or show a login button
            // to ask users to authenticate. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
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
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);

    }

    void Update()
    {
        switch (GameState)
        {
            case GameStates.Init:
                if (PlayerSave.Instance.IsSaveLoad) { GameState = GameStates.Menu; }
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

    void PauseActive()
    {
        GameState = GameStates.Pause;
    }

    void PauseDesactive()
    {
        GameState = GameStates.Play;
    }
}