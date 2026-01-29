using TMPro;
using UnityEngine;
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
    [Header("Level Select")]
    [SerializeField] private GameObject _levelSelect;
    [SerializeField] private Button _buttonLevelSelectExit;
    [SerializeField] private GameObject _levelList;
    private Button[] _buttonsLevel;
    [Header("Credits")]
    [SerializeField] private GameObject _credits;
    [SerializeField] private Button _buttonCreditsExit;
    

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
        _mainMenu.SetActive(true);
        _parameters.SetActive(false);
        _levelSelect.SetActive(false);

        _buttonMainMenuLevelSelect.onClick.AddListener(() => MenuActive(_levelSelect, true));
        _buttonMainMenuParameters.onClick.AddListener(() => MenuActive(_parameters, true));
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
            button.onClick.AddListener(() => ScenesManager.Instance.LoadSceneLevel(levelIndex));
        }
    }

    private void MenuActive(GameObject menu, bool open)
    {
        menu.SetActive(open);
    }
}
