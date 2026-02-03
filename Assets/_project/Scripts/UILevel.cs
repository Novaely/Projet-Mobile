using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevel : MonoBehaviour
{
    [Header("Dictionnary")]
    [SerializeField] private GameObject _dictionnary;
    [SerializeField] private Button _buttonDictionnary;
    [SerializeField] private Button _buttonDicoExit;
    [SerializeField] private Button _buttonMainMenu;
    [SerializeField] private Button _buttonLevelSelect;

    [Header("Parameters")]
    [SerializeField] private GameObject _parameters;
    [SerializeField] private Button _buttonParameters;
    [SerializeField] private Button _buttonParamExit;
    private LocaleSelector _localeSelector;
    [SerializeField] private Button _buttonFrench;
    [SerializeField] private Button _buttonEnglish;

    void Start()
    {
        UIManager.Instance.SetActiveMenu((_dictionnary, true), (_parameters, true));

        _buttonDictionnary.onClick.RemoveAllListeners();
        _buttonDicoExit.onClick.RemoveAllListeners();
        _buttonMainMenu.onClick.RemoveAllListeners();
        _buttonLevelSelect.onClick.RemoveAllListeners();
        _buttonParameters.onClick.RemoveAllListeners();
        _buttonParamExit.onClick.RemoveAllListeners();
        _buttonFrench.onClick.RemoveAllListeners();
        _buttonEnglish.onClick.RemoveAllListeners();

        _buttonDictionnary.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_dictionnary, true)));
        _buttonMainMenu.onClick.AddListener(() => ScenesManager.Instance.LoadSceneMainMenu());
        _buttonLevelSelect.onClick.AddListener(() => ScenesManager.Instance.LoadSceneMainMenu(true));
        _buttonParameters.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_parameters, true)));
        _localeSelector = FindAnyObjectByType<LocaleSelector>();
        _buttonFrench.onClick.AddListener(() => _localeSelector.SetLanguage(1));
        _buttonEnglish.onClick.AddListener(() => _localeSelector.SetLanguage(0));

        _buttonParamExit.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_parameters, false)));
        _buttonDicoExit.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_dictionnary, false)));

        UIManager.Instance.InitializeTextTranslate();

        UIManager.Instance.SetActiveMenu((_dictionnary, false), (_parameters, false));
    }
}
