using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILevel : MonoBehaviour
{
    [Header("Dictionnary")]
    [SerializeField] private GameObject _dictionnary;
    [SerializeField] private DicoDinoDatabase _databaseDico;
    [SerializeField] private TextMeshProUGUI _textDico;
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

    [Header("End Level")]
    [SerializeField] private Button _btnValidateButton;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI percentText;
    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] private Image[] starImages;
    [SerializeField] private Button _btnLevelSelect;
    [SerializeField] private Button _btnRestartLevel;
    [SerializeField] private Button _btnNextLevel;
    private LevelScorer _levelScorer;

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    void Start()
    {
        _levelScorer = FindFirstObjectByType<LevelScorer>();

        if (resultPanel != null) resultPanel.SetActive(true);

        _levelScorer.InitializeLevel();

        InitializeUI();

        if (_btnValidateButton != null)
        {
            _btnValidateButton.onClick.RemoveAllListeners();
            _btnValidateButton.onClick.AddListener(OnValidateClick);
            _btnValidateButton.interactable = false;
        }

        if (resultPanel != null) resultPanel.SetActive(false);

        _levelScorer.UpdateScore();
    }

    private void InitializeUI()
    {
        _btnLevelSelect.onClick.RemoveAllListeners();
        _btnRestartLevel.onClick.RemoveAllListeners();
        _btnNextLevel.onClick.RemoveAllListeners();

        _btnLevelSelect.onClick.AddListener(() => ScenesManager.Instance.LoadSceneMainMenu(true));
        _btnRestartLevel.onClick.AddListener(() => ScenesManager.Instance.LoadSceneLevel(ScenesManager.Instance.GetLevelIndex(SceneManager.GetActiveScene())));
        _btnNextLevel.onClick.AddListener(() => ScenesManager.Instance.LoadSceneLevel(ScenesManager.Instance.GetLevelIndex(SceneManager.GetActiveScene()) + 1));
    }

    public void InitializeParam()
    {
        UIManager.Instance.SetActiveMenu((_dictionnary, true), (_parameters, true));

        _textDico.GetComponent<LocalizeStringEvent>().OnUpdateString.RemoveAllListeners();
        _buttonDictionnary.onClick.RemoveAllListeners();
        _buttonDicoExit.onClick.RemoveAllListeners();
        _buttonMainMenu.onClick.RemoveAllListeners();
        _buttonLevelSelect.onClick.RemoveAllListeners();
        _buttonParameters.onClick.RemoveAllListeners();
        _buttonParamExit.onClick.RemoveAllListeners();
        _buttonFrench.onClick.RemoveAllListeners();
        _buttonEnglish.onClick.RemoveAllListeners();

        _textDico.GetComponent<LocalizeStringEvent>().OnUpdateString.AddListener(_ => UpdateDicoDino());
        _buttonDictionnary.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_dictionnary, true)));
        _buttonMainMenu.onClick.AddListener(() => ScenesManager.Instance.LoadSceneMainMenu());
        _buttonLevelSelect.onClick.AddListener(() => ScenesManager.Instance.LoadSceneMainMenu(true));
        _buttonParameters.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_parameters, true)));
        _localeSelector = FindAnyObjectByType<LocaleSelector>();
        _buttonFrench.onClick.AddListener(() => _localeSelector.SetLanguage(1));
        _buttonEnglish.onClick.AddListener(() => _localeSelector.SetLanguage(0));

        _buttonParamExit.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_parameters, false)));
        _buttonDicoExit.onClick.AddListener(() => UIManager.Instance.SetActiveMenu((_dictionnary, false)));

        UpdateDicoDino();

        UIManager.Instance.InitializeTextTranslate();

        UIManager.Instance.SetActiveMenu((_dictionnary, false), (_parameters, false));
    }

    private void UpdateDicoDino()
    {
        var table = LocalizationSettings.StringDatabase.GetTable("DicoDino");
        _textDico.text = string.Empty;

        foreach (var word in _databaseDico.Dictionnary)
        {
            var entry = table.GetEntry(word.KeyUniversalTraduction);
            if (entry == null)
                continue;

            _textDico.text += word.DinoWord + " : " + entry.GetLocalizedString() + "\n";
        }
    }

    public void UpdateUI()
    {
        float percent = _levelScorer.GetPercent();

        if (scoreText != null) scoreText.text = $"Score: {_levelScorer.currentScore}/{_levelScorer.maxScore}";
        if (percentText != null) percentText.text = $"{percent:F0}%";
        if (starsText != null) starsText.text = $"{_levelScorer.GetStars()}★";

        UpdateStarImages();

        if (_btnValidateButton != null)
        {
            int occupiedSeats = 0;
            foreach (var seat in _levelScorer.seats)
            {
                if (seat != null && seat.occupant != null) occupiedSeats++;
            }

            bool isFull = (occupiedSeats >= _levelScorer.maxScore);
            _btnValidateButton.interactable = isFull;
        }
    }

    public void OnValidateClick()
    {
        _levelScorer.levelValidated = true;

        if (_btnValidateButton != null) _btnValidateButton.gameObject.SetActive(false);
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
            PlayerSave.Instance.UpdateStarOfOneLevel(_levelScorer.GetStars());
        }
    }

    void UpdateStarImages()
    {
        if (starImages != null)
        {
            int stars = _levelScorer.GetStars();
            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] != null) starImages[i].enabled = (i < stars);
            }
        }
    }

    private void OnLocaleChanged(Locale locale)
    {
        UpdateDicoDino();
    }
}
