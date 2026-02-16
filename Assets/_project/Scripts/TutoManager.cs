using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutoManager : MonoBehaviour
{

    [SerializeField] Canvas _canvasTuto;
    [SerializeField] TextMeshProUGUI _zoneText;

    [Serializable]
    struct InfoTuto
    {
        public RectTransform position;
        public string text;
    }

    [SerializeField] List<InfoTuto> InfoTutos;

    int index = 0;

    private void Start()
    {
        index = 0;
        _zoneText.text = InfoTutos[index].text;
        _zoneText.rectTransform.position = InfoTutos[index].position.position;
    }

    private void Update()
    {
        if (Touchscreen.current != null)
        {
            var t = Touchscreen.current.primaryTouch;

            if (t.press.wasReleasedThisFrame && !t.press.isPressed)
            {
                index++;
                if (index >= InfoTutos.Count)
                {
                    _canvasTuto.gameObject.SetActive(false);
                    return;
                }
                _zoneText.text = InfoTutos[index].text;
                _zoneText.rectTransform.position = InfoTutos[index].position.position;
            }
        }
    }
}
