using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public enum AchivementEnum
{
    Tuto = 1,
    Level2 = 2,
    Level3 = 3,
    Level4 = 4,
    Level5 = 5,
    Level6 = 6,
    Level7 = 7,
    Level8 = 8,
    Level9 = 9,
    Level10 = 10,
    Level11 = 11,
    Level12 = 12,
    Level13 = 13,
    Level14 = 14,
    Level15 = 15,
}

public class GooglePlayManager : MonoBehaviour
{
#if !UNITY_EDITOR
    // ---------- VARIABLES ---------- \\

    // ----- Singleton ----- \\

    public static GooglePlayManager Instance { get; private set; }

    // ----- Prefabs & Assets ----- \\

    // ----- Objects ----- \\

    // ----- Others ----- \\

    private static Dictionary<AchivementEnum, string> _mapAchievmentIds = new()
    {
        {AchivementEnum.Tuto, "CgkIz4CsybwSEAIQAg" },
        {AchivementEnum.Level2, "CgkIz4CsybwSEAIQAw" },
        {AchivementEnum.Level3, "CgkIz4CsybwSEAIQBA" },
        {AchivementEnum.Level4, "CgkIz4CsybwSEAIQBQ" },
        {AchivementEnum.Level5, "CgkIz4CsybwSEAIQBg" },
        {AchivementEnum.Level6, "CgkIz4CsybwSEAIQBw" },
        {AchivementEnum.Level7, "CgkIz4CsybwSEAIQCA" },
        {AchivementEnum.Level8, "CgkIz4CsybwSEAIQCQ" },
        {AchivementEnum.Level9, "CgkIz4CsybwSEAIQCg" },
        {AchivementEnum.Level10, "CgkIz4CsybwSEAIQCw" },
        {AchivementEnum.Level11, "CgkIz4CsybwSEAIQDA" },
        {AchivementEnum.Level12, "CgkIz4CsybwSEAIQDQ" },
        {AchivementEnum.Level13, "CgkIz4CsybwSEAIQDg" },
        {AchivementEnum.Level14, "CgkIz4CsybwSEAIQDw" },
        {AchivementEnum.Level15, "CgkIz4CsybwSEAIQEA" },
    };

    private static Dictionary<string, bool> _mapAchivementsState = new();

    // ---------- FUNCTIONS ---------- \\

    // ----- Buil-in ----- \\

    private void OnEnable() { }

    private void OnDisable() { }

    private void Awake()
    {
        // Singleton
        if (Instance != null)
        {
            Debug.Log(nameof(GooglePlayManager) + " Instance already exist, destorying last added.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Google play (je crois)
        //PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);

        // Unity
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate(ProcessAuthentication);

        Social.LoadAchievements(LoadAchievements);
    }

    void Update() { }

    // ----- My Functions ----- \\

    public void CompleteAchievement(AchivementEnum achievement)
    {
        if (!_mapAchievmentIds.ContainsKey(achievement)) return;

        if (!IsAchievementFinished(achievement))
        {
            Social.ReportProgress(_mapAchievmentIds[achievement], 100.0f, (bool success) => { });
        }
    }

    private static void LoadAchievements(IAchievement[] achievements)
    {
        foreach (IAchievement achievement in achievements)
        {
            _mapAchivementsState.Add(achievement.id, achievement.completed);
        }
    }

    private static bool IsAchievementFinished(AchivementEnum achievement)
    {
        if (!_mapAchievmentIds.ContainsKey(achievement)) return false;
        if (!_mapAchivementsState.ContainsKey(_mapAchievmentIds[achievement])) return false;

        return _mapAchivementsState[_mapAchievmentIds[achievement]];
    }

    internal void ProcessAuthentication(bool status)
    {
        if (status)
        {
            Debug.Log("Singed in!");
        }
        else
        {
            Debug.Log("Not signed in.");
        }
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            Debug.Log("Singed in!");
        }
        else
        {
            Debug.Log("Not signed in.");
        }
    }

    // ----- Destructor ----- \\

    protected virtual void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
#endif
}