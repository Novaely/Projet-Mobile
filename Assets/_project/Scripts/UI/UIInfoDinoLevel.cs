using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SpawnManager;

public class UIInfoDinoLevel : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI _textDinoName;
    [SerializeField] private TextMeshProUGUI _textDinoContraintePositive;
    [SerializeField] private TextMeshProUGUI _textDinoContrainteNegative;

    [Header("Navigation Buttons")]
    [SerializeField] private GameObject _buttonNextDino;
    [SerializeField] private GameObject _buttonPreviousDino;

    [Header("Visual Preview")]
    [SerializeField] private Image _imagePreviewDino;

    public event Func<bool, InfoDino> OnNextDino;
    public event Func<InfoDino> OnPreviousDino;

    public void NextDino(bool forced)
    {
        InfoDino infoDino = OnNextDino?.Invoke(forced);
        UpdateUI(infoDino);
    }

    public void PreviousDino()
    {
        InfoDino infoDino = OnPreviousDino?.Invoke();
        UpdateUI(infoDino);
    }

    private void UpdateUI(InfoDino infoDino)
    {
        if (infoDino == null) return;

        if (_textDinoName != null) 
            _textDinoName.text = infoDino.label;

        if (_textDinoContraintePositive != null) 
            _textDinoContraintePositive.text = infoDino.contraintePositive;

        if (_textDinoContrainteNegative != null) 
            _textDinoContrainteNegative.text = infoDino.contrainteNegative;

        if (_imagePreviewDino != null)
        {
            _imagePreviewDino.sprite = infoDino.sprite;
            _imagePreviewDino.enabled = (infoDino.sprite != null);
        }
    }
}