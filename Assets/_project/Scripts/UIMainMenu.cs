using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
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
    [SerializeField] LevelsSave _levelData;
    [SerializeField] private GameObject _levelSelect;
    [SerializeField] private Button _buttonLevelSelectExit;
    [SerializeField] private GameObject _levelList;
    [SerializeField] private GameObject _prefabButtonLevel;
    private List<Button> _buttonsLevel = new List<Button>();
    private bool _isActive = false;
    public void IsLevelSelectActive(bool isActive) => _isActive = isActive;
    [Header("Credits")]
    [SerializeField] private GameObject _credits;
    [SerializeField] private Button _buttonCreditsExit;


    private void Start()
    {;
        UIManager.Instance.SetActiveMenu((_mainMenu, true), (_parameters, true), (_levelSelect, true), (_credits, true));

        _buttonMainMenuLevelSelect.onClick.RemoveAllListeners();
        _buttonMainMenuParameters.onClick.RemoveAllListeners();
        _buttonMainMenuCredit.onClick.RemoveAllListeners();
        _buttonParamExit.onClick.RemoveAllListeners();
        _buttonFrench.onClick.RemoveAllListeners();
        _buttonEnglish.onClick.RemoveAllListeners();
        _buttonLevelSelectExit.onClick.RemoveAllListeners();
        _buttonCreditsExit.onClick.RemoveAllListeners();

        _buttonMainMenuLevelSelect.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_levelSelect, true)));
        _buttonMainMenuParameters.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_parameters, true)));
        _localeSelector = FindAnyObjectByType<LocaleSelector>();
        _buttonFrench.onClick.AddListener(() => _localeSelector.SetLanguage(1));
        _buttonEnglish.onClick.AddListener(() => _localeSelector.SetLanguage(0));
        _buttonMainMenuCredit.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_credits, true)));

        _buttonParamExit.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_parameters, false)));
        _buttonLevelSelectExit.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_levelSelect, false)));
        _buttonCreditsExit.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_credits, false)));

        for (int nLevel = 1; nLevel < SceneManager.sceneCountInBuildSettings; nLevel++)
        {
            int index = nLevel;
            var buttonLevel = Instantiate(_prefabButtonLevel, _levelList.transform);
            buttonLevel.name = "ButtonLevel" + nLevel;
            var button = buttonLevel.GetComponent<Button>();
            _buttonsLevel.Add(button);
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            text.text = nLevel.ToString();
            text.fontSize = 40;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => ScenesManager.Instance.LoadSceneLevel(index));
            if (index > 1)
            {
                if (_levelData.starsLevels[index - 2] <= 0)
                {
                    button.interactable = false;
                }
            }
            buttonLevel.GetComponent<ButtonLevelInfo>().index = index-1;
        }

        UIManager.Instance.InitializeTextTranslate();

        UIManager.Instance.SetActiveMenu((_mainMenu, true), (_parameters, false), (_levelSelect, _isActive), (_credits, false));
    }
}
