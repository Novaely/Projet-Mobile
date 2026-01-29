using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("MainMenu")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private Button _buttonMainMenuLevelSelect;
    [SerializeField] private Button _buttonMainMenuParameters;
    [SerializeField] private Button _buttonMainMenuCredit;
    [Header("Parameters")]
    [SerializeField] private GameObject _parameters;
    [SerializeField] private Button _buttonParamExit;
    private LocaleSelector _localeSelector;
    [SerializeField] private Button _buttonFrench;
    [SerializeField] private Button _buttonEnglish;
    [Header("Level Select")]
    [SerializeField] private GameObject _levelSelect;
    [SerializeField] private Button _buttonLevelSelectExit;
    [SerializeField] private GameObject _levelList;
    private Button[] _buttonsLevel;
    [Header("Credits")]
    [SerializeField] private GameObject _credits;
    [SerializeField] private Button _buttonCreditsExit;

    private TextMeshProUGUI[] _textsToTranslate;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        SetActiveMenu(true, true, true, true);

        _buttonMainMenuLevelSelect.onClick.RemoveAllListeners();
        _buttonMainMenuParameters.onClick.RemoveAllListeners();
        _buttonMainMenuCredit.onClick.RemoveAllListeners();
        _buttonParamExit.onClick.RemoveAllListeners();
        _buttonLevelSelectExit.onClick.RemoveAllListeners();
        _buttonCreditsExit.onClick.RemoveAllListeners();

        _buttonMainMenuLevelSelect.onClick.AddListener(() => MenuActive(_levelSelect, true));
        _buttonMainMenuParameters.onClick.AddListener(() => MenuActive(_parameters, true));
        _localeSelector = FindAnyObjectByType<LocaleSelector>();
        _buttonFrench.onClick.AddListener(() => _localeSelector.SetLanguage(1));
        _buttonEnglish.onClick.AddListener(() => _localeSelector.SetLanguage(0));
        _buttonMainMenuCredit.onClick.AddListener(() => MenuActive(_credits, true));

        _buttonParamExit.onClick.AddListener(() => MenuActive(_parameters, false));
        _buttonLevelSelectExit.onClick.AddListener(() => MenuActive(_levelSelect, false));
        _buttonCreditsExit.onClick.AddListener(() => MenuActive(_credits, false));

        _buttonsLevel = _levelList.GetComponentsInChildren<Button>();
        int index = 1;
        foreach (var button in _buttonsLevel)
        {
            int levelIndex = index++;
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            text.text = levelIndex.ToString();
            text.fontSize = 40;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => ScenesManager.Instance.LoadSceneLevel(levelIndex));
        }

        _textsToTranslate = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        foreach (var text in _textsToTranslate)
        {
            if (text.TryGetComponent<LocalizeStringEvent>(out var loca))
            {
                loca.OnUpdateString.RemoveAllListeners();
                loca.OnUpdateString.AddListener(text.SetText);
            }
        }

        _mainMenu.SetActive(true);
        _parameters.SetActive(false);
        _levelSelect.SetActive(false);
        SetActiveMenu(true, false, false, false);
    }

    private void MenuActive(GameObject menu, bool open)
    {
        menu.SetActive(open);
    }

    public void SetActiveMenu(bool mainMenu, bool levelSelect, bool parameters, bool credits)
    {
        _mainMenu.SetActive(mainMenu);
        _parameters.SetActive(levelSelect);
        _levelSelect.SetActive(parameters);
        _credits.SetActive(credits);
    }
}
