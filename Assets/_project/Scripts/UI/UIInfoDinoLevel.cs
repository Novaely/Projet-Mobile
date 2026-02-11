using System;
using System.Collections;
using System.Collections.Generic;
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

    public event Func<SpawnManager.InfoDino> OnNextDino;
    public event Func<SpawnManager.InfoDino> OnPreviousDino;

    public void NextDino()
    {
        SpawnManager.InfoDino infoDino =  OnNextDino?.Invoke();

        _textDinoName.text = infoDino.label;
        _textDinoContraintePositive.text = infoDino.contraintePositive;
        _textDinoContrainteNegative.text = infoDino.contrainteNegative;
        _imagePreviewDino.sprite = infoDino.sprite;

    }

    public void PreviousDino()
    {
        SpawnManager.InfoDino infoDino = OnPreviousDino?.Invoke();

        _textDinoName.text = infoDino.label;
        _textDinoContraintePositive.text = infoDino.contraintePositive;
        _textDinoContrainteNegative.text = infoDino.contrainteNegative;
        _imagePreviewDino.sprite = infoDino.sprite;
    }

    public void CleanDino()
    {
        _textDinoName.text = "";
        _textDinoContraintePositive.text = "";
        _textDinoContrainteNegative.text = "";
        _imagePreviewDino.sprite = null;
    }
}