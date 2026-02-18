using System;
using TMPro;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.UI;
using static SpawnManager;

public class UIInfoDinoLevel : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] Image _imageDiet;
    [SerializeField] private TextMeshProUGUI _textDinoName;
    [SerializeField] private TextMeshProUGUI _textDinoContraintePositive;
    [SerializeField] private TextMeshProUGUI _textDinoContrainteNegative;

    [Header("Navigation Buttons")]
    [SerializeField] private GameObject _buttonNextDino;
    [SerializeField] private GameObject _buttonPreviousDino;

    [Header("Visual Preview")]
    [SerializeField] private Image _imagePreviewDino;
    public RectTransform Spawn;

    [Header("Sprite Need")]
    [SerializeField] Sprite _spriteCarnivor;
    [SerializeField] Sprite _spriteHerbivor;

    public event Func<bool, DinoCharacteristic> OnNextDino;
    public event Func<DinoCharacteristic> OnPreviousDino;

    private void Start()
    {
        Spawn = _imagePreviewDino.GetComponent<RectTransform>();
    }

    public void NextDino(bool forced)
    {
        if (GameManager.Instance.GameState != GameManager.GameStates.Play) { return; }

        DinoCharacteristic infoDino = OnNextDino?.Invoke(forced);
        UpdateUI(infoDino,false);
    }

    public void PreviousDino()
    {
        if (GameManager.Instance.GameState != GameManager.GameStates.Play) { return; }

        DinoCharacteristic infoDino = OnPreviousDino?.Invoke();
        UpdateUI(infoDino,false);
    }

    public void PreciseDino(DinoCharacteristic infoDino,bool isPlace)
    {
        UpdateUI(infoDino,isPlace);
    }

    private void UpdateUI(DinoCharacteristic infoDino,bool isPlaced)
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
            if (isPlaced)
            {
                _imagePreviewDino.gameObject.SetActive(true);
            }
            else
            {
                _imagePreviewDino.gameObject.SetActive(false);
            }
        }

        if (_imageDiet != null)
        {
            _imageDiet.sprite = infoDino.diet == DietType.Carnivore ? _spriteCarnivor : _spriteHerbivor;
        }
    }
}