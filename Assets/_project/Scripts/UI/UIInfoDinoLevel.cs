using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SpawnManager;

public class UIInfoDinoLevel : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] TextMeshProUGUI _textDinoName;
    [SerializeField] TextMeshProUGUI _textDinoContraintePositive;
    [SerializeField] TextMeshProUGUI _textDinoContrainteNegative;

    [Header("Button")]
    [SerializeField] GameObject _buttonNextDino;
    [SerializeField] GameObject _buttonPreviousDino;

    [Header("Visuel")]
    [SerializeField] Image _imagePreviewDino;

    public event Func<bool, InfoDino> OnNextDino;
    public event Func<InfoDino> OnNextDinoForced;
    public event Func<InfoDino> OnPreviousDino;

    public void NextDino(bool forced)
    {
        InfoDino infoDino =  OnNextDino?.Invoke(forced);

        if (infoDino == null) { return; }

        _textDinoName.text = infoDino.label;
        _textDinoContraintePositive.text = infoDino.contraintePositive;
        _textDinoContrainteNegative.text = infoDino.contrainteNegative;
        _imagePreviewDino.sprite = infoDino.sprite;

    }

    public void PreviousDino()
    {
        InfoDino infoDino = OnPreviousDino?.Invoke();

        if (infoDino == null) { return; }

        _textDinoName.text = infoDino.label;
        _textDinoContraintePositive.text = infoDino.contraintePositive;
        _textDinoContrainteNegative.text = infoDino.contrainteNegative;
        _imagePreviewDino.sprite = infoDino.sprite;
    }
}