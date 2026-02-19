using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

public class TutoManager : MonoBehaviour
{

    [SerializeField] Canvas _canvasTuto;
    [SerializeField] TextMeshProUGUI _zoneText;

    [Serializable]
    struct InfoTuto
    {
        public RectTransform position;
        [TextArea(5,9)] public string text;
        [TextArea(5,9)] public string trad;
    }

    [SerializeField] List<InfoTuto> InfoTutos;

    int index = 0;

    public event Action OnTutoEnd;

    private void Start()
    {
        index = 0;
        if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[1])
        {
            _zoneText.text = InfoTutos[index].text;
        }
        else if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0])
        {
            _zoneText.text = InfoTutos[index].trad;
        }

        _zoneText.rectTransform.position = InfoTutos[index].position.position;
    }

    private void Update()
    {
        if (Touchscreen.current != null)
        {
            var t = Touchscreen.current.primaryTouch;

            if (t.press.wasReleasedThisFrame)
            {
                index++;
                if (index >= InfoTutos.Count)
                {
                    _canvasTuto.gameObject.SetActive(false);
                    OnTutoEnd?.Invoke();
                    return;
                }
                if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[1])
                {
                    _zoneText.text = InfoTutos[index].text;
                }
                else if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0])
                {
                    _zoneText.text = InfoTutos[index].trad;
                }
                _zoneText.rectTransform.position = InfoTutos[index].position.position;
            }
        }
    }
}
