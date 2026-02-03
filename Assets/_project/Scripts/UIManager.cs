using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private TextMeshProUGUI[] _textsToTranslate;

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
}
