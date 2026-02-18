using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using System;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject _prefabUILevel;
    [SerializeField] private GameObject _prefabUILevelInfoDino;

    private TextMeshProUGUI[] _textsToTranslate;

    public event Action OnPauseActive;
    public event Action OnPauseDesactive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    public void SetActiveMenu(params (GameObject, bool)[] menus)
    {
        foreach (var menu in menus)
        {
            menu.Item1.SetActive(menu.Item2);
        }
    }

    public void InitializeTextTranslate()
    {
        _textsToTranslate = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        foreach (var text in _textsToTranslate)
        {
            if (text.TryGetComponent<LocalizeStringEvent>(out var loca))
            {
                loca.OnUpdateString.RemoveAllListeners();
                loca.OnUpdateString.AddListener(text.SetText);
                loca.RefreshString();
            }
        }
    }

    public void LoadUILevel()
    {
        GameObject uiLevel = Instantiate(_prefabUILevel);
        GameObject uiLevelInfoDino = Instantiate(_prefabUILevelInfoDino);
        uiLevelInfoDino.GetComponent<Canvas>().worldCamera = Camera.main;
        uiLevel.GetComponentInChildren<UILevel>().InitializeParam();
    }

    public void SetPause(bool acive)
    {
        if (acive)
        {
            OnPauseActive?.Invoke();
        }
        else
        {
            OnPauseDesactive?.Invoke();
        }
    }

    public void InitializeVolume(Slider music, Slider sfx)
    {
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        music.value = savedMusicVolume;
        AudioManager.Instance.SetMusicVolume(savedMusicVolume);

        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfx.value = savedSFXVolume;
        AudioManager.Instance.SetSFXVolume(savedSFXVolume);
    }
}
