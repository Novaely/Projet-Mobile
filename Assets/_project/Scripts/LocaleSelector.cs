using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour
{
    public static LocaleSelector Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);
        SetLanguage(PlayerPrefs.GetInt("Language"));
    }

    public void SetLanguage(int localeIndex)
    {
        StartCoroutine(SetLocale(localeIndex));
    }

    IEnumerator SetLocale(int index)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        PlayerPrefs.SetInt("Language", index);
    }
}
