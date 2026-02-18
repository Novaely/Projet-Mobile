using GooglePlayGames.OurUtils;
using System.Collections;
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
    private AudioManager _audioManager;
    [SerializeField] private Slider _sliderMusic;
    [SerializeField] private Slider _sliderSFX;

    [Header("Level Select")]
    [SerializeField] WorldData _worldData;
    [SerializeField] private GameObject _levelSelect;
    [SerializeField] private Button _buttonLevelSelectExit;
    [SerializeField] GameObject _worldContainer;
    [SerializeField] GameObject _prefabsLevelContainer;
    [SerializeField] private List<GameObject> _levelContainers;
    [SerializeField] private GameObject _prefabButtonLevel;
    private List<Button> _buttonsLevel = new List<Button>();
    private bool _isActive = false;
    [SerializeField] GameObject _buttonPreviousWorld;
    [SerializeField] GameObject _buttonNextWorld;
    int _currentWorldId = 0;
    [SerializeField] GameObject _textStar;
    public void IsLevelSelectActive(bool isActive) => _isActive = isActive;

    [Header("Credits")]
    [SerializeField] private GameObject _credits;
    [SerializeField] private Button _buttonCreditsExit;


    private void Start()
    {
        UIManager.Instance.SetActiveMenu((_mainMenu, true), (_parameters, true), (_levelSelect, true), (_credits, true));

        _buttonMainMenuLevelSelect.onClick.RemoveAllListeners();
        _buttonMainMenuParameters.onClick.RemoveAllListeners();
        _buttonMainMenuCredit.onClick.RemoveAllListeners();
        _buttonParamExit.onClick.RemoveAllListeners();
        _buttonFrench.onClick.RemoveAllListeners();
        _buttonEnglish.onClick.RemoveAllListeners();
        _buttonLevelSelectExit.onClick.RemoveAllListeners();
        _buttonCreditsExit.onClick.RemoveAllListeners();
        _sliderMusic.onValueChanged.RemoveAllListeners();
        _sliderSFX.onValueChanged.RemoveAllListeners();

        _buttonMainMenuLevelSelect.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_levelSelect, true)));
        _buttonMainMenuParameters.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_parameters, true)));
        _localeSelector = LocaleSelector.Instance;
        _buttonFrench.onClick.AddListener(() => _localeSelector.SetLanguage(1));
        _buttonEnglish.onClick.AddListener(() => _localeSelector.SetLanguage(0));
        _audioManager = AudioManager.Instance;
        _sliderMusic.onValueChanged.AddListener((value) => _audioManager.SetMusicVolume(value));
        _sliderSFX.onValueChanged.AddListener((value) => _audioManager.SetSFXVolume(value));
        UIManager.Instance.InitializeVolume(_sliderMusic, _sliderSFX);
        _buttonMainMenuCredit.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_credits, true)));

        _buttonParamExit.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_parameters, false)));
        _buttonLevelSelectExit.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_levelSelect, false)));
        _buttonCreditsExit.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_credits, false)));

        if (PlayerSave.Instance.IsSaveLoad)
        {
            InitialiseMenuLevelSelection();
        }
        else
        {
            StartCoroutine(WaitForSave());
        }

        UIManager.Instance.InitializeTextTranslate();

        UIManager.Instance.SetActiveMenu((_mainMenu, true), (_parameters, false), (_levelSelect, _isActive), (_credits, false));
    }


    void InitialiseMenuLevelSelection()
    {
        _buttonPreviousWorld.SetActive(false);
        int idWorld = 0;
        GameObject world = Instantiate(_prefabsLevelContainer, _worldContainer.transform);
        world.name = _worldData.Worlds[idWorld].label;
        _levelContainers.Add(world);
        for (int nLevel = 1; nLevel<SceneManager.sceneCountInBuildSettings; nLevel++)
        {
            if (_levelContainers[idWorld].transform.childCount == _worldData.Worlds[idWorld].levelInThisWorld)
            {
                idWorld++;
                world = Instantiate(_prefabsLevelContainer, _worldContainer.transform);
                world.name = _worldData.Worlds[idWorld].label;
                _levelContainers.Add(world);
                world.SetActive(false);
            }
            int index = nLevel;
            var buttonLevel = Instantiate(_prefabButtonLevel, _levelContainers[idWorld].transform);
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
                if (PlayerSave.Instance.LevelSave.starsLevels[index - 2] <= 0)
                {
                    button.interactable = false;
                }
            }
            buttonLevel.GetComponent<ButtonLevelInfo>().index = index - 1;
        }

        UpdateButtonNextWorld();
    }

    IEnumerator WaitForSave()
    {
        yield return new WaitForSeconds(0.01f);

        InitialiseMenuLevelSelection();
    }

    public void NextWorld()
    {
        if (_currentWorldId >= _levelContainers.Count -1) { return; }

        if (_currentWorldId == 0)
        {
            _buttonPreviousWorld.SetActive(true);
        }

        _levelContainers[_currentWorldId].SetActive(false);
        _currentWorldId++;
        _levelContainers[_currentWorldId].SetActive(true);

        if (_currentWorldId >= _levelContainers.Count - 1)
        {
            _buttonNextWorld.SetActive(false);
        }

        UpdateButtonNextWorld();
    }

    public void PreviousWorld()
    {
        if (_currentWorldId == 0) { return; }

        if (_currentWorldId >= _levelContainers.Count - 1)
        {
            _buttonNextWorld.SetActive(true);
        }

        _levelContainers[_currentWorldId].SetActive(false);
        _currentWorldId--;
        _levelContainers[_currentWorldId].SetActive(true);

        if (_currentWorldId == 0)
        {
            _buttonPreviousWorld.SetActive(false);
        }

        UpdateButtonNextWorld();
    }



    void UpdateButtonNextWorld()
    {
        if (_currentWorldId+1 >  _worldData.Worlds.Count-1) { return; }

        int numberStar = 0;
        foreach (var nbStarLevel in PlayerSave.Instance.LevelSave.starsLevels)
        {
            numberStar += nbStarLevel;
        }


        Button button = _buttonNextWorld.GetComponent<Button>();
        TextMeshProUGUI textStar = _textStar.GetComponentInChildren<TextMeshProUGUI>();

        if (numberStar >= _worldData.Worlds[_currentWorldId+1].numberStarNeed)
        {
            button.interactable = true;
            _textStar.SetActive(false);
        }
        else
        {
            button.interactable = false;
            _textStar.SetActive(true);
            textStar.text = numberStar.ToString() + " / " + _worldData.Worlds[_currentWorldId + 1].numberStarNeed.ToString();
        }
    }
}
