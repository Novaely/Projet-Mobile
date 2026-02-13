using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    DragManager _dragManager;

    [SerializeField] Seat _spawnerSeat;
    [SerializeField] List<GameObject> _dinosPrefab;
    List<InfoDino> _dinos = new();
    [SerializeField] Transform _dinosContainer;

    public event Action <Dino, Transform> OnSpawn;

    int _currentIdDino = -1;

    int _nbDinoPlaced = 0;

    bool _isInfoDinoAlreadyPlaced;

    [Serializable]
    public class InfoDino
    {
        public int id;
        public GameObject dino;
        public DinoCharacteristic characteristic;
        public bool isPlace;
    }

    [Serializable]
    public class DinoCharacteristic
    {
        public string label;
        public string contraintePositive;
        public string contrainteNegative;
        public Sprite sprite;
    }

    UIInfoDinoLevel _UIInfoDino;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(WaitStart());

        IEnumerator WaitStart()
        {
            yield return new WaitForEndOfFrame();
            _UIInfoDino = FindFirstObjectByType<UIInfoDinoLevel>();
            _dragManager = FindFirstObjectByType<DragManager>();

            _UIInfoDino.OnNextDino += NextDino;
            _UIInfoDino.OnPreviousDino += PrevDino;

            _dragManager.OnDinoClicked += DinoClicked; 

            for (int i = 0; i < _dinosPrefab.Count; i++)
            {
                GameObject dino = Instantiate(_dinosPrefab[i], _spawnerSeat.transform.position, Quaternion.identity, _dinosContainer);

                Dino dinoScript = dino.GetComponent<Dino>();

                InfoDino info = new InfoDino
                {
                    id = i,
                    dino = dino,
                    characteristic = new DinoCharacteristic
                    {
                        label = (dinoScript.profile != null) ? dinoScript.profile.speciesName : dino.name,
                        contraintePositive = (dinoScript.profile != null) ? dinoScript.profile.positiveCondition : "Aucune",
                        contrainteNegative = (dinoScript.profile != null) ? dinoScript.profile.negativeCondition : "Aucune",
                        sprite = dinoScript.passiveSprite
                    },
                    isPlace = false,
                };

                if (i == 0)
                {
                    Dino dinoDino = dino.GetComponent<Dino>();
                    OnSpawn?.Invoke(dinoDino, _spawnerSeat.transform);
                }
                else
                {
                    dino.SetActive(false);
                }

                _dinos.Add(info);
            }

            _UIInfoDino.NextDino(false);
        }
    }

    private void Update()
    {
        if (_currentIdDino == -1) { return; }

        if (_dinos.Count == _nbDinoPlaced)
        {
            return;
        }

        if (_spawnerSeat != null && _spawnerSeat.occupant == null)
        {
            _nbDinoPlaced++;
            _dinos[_currentIdDino].isPlace = true;
            if (_dinos.Count == _nbDinoPlaced)
            {
                _UIInfoDino.PreciseDino(_dinos[_currentIdDino].characteristic, _dinos[_currentIdDino].isPlace);
            }
            else
            {
                _UIInfoDino.NextDino(true);
            }
        }
    }

    DinoCharacteristic NextDino(bool forced)
    {
        if (_dinos.Count == _nbDinoPlaced) { return null; }

        if (!forced && _dinos.Count >= 1 && _currentIdDino != -1)
        {
            _dinos[_currentIdDino].dino.SetActive(false);
        }

        if (!_isInfoDinoAlreadyPlaced)
        {
            do
            {
                _currentIdDino = (_currentIdDino + 1) % _dinos.Count;
            } while (_dinos[_currentIdDino].isPlace);
        }
        else
        {
            _isInfoDinoAlreadyPlaced = false;
        }

        InfoDino infoDino = _dinos[_currentIdDino];

        Dino dinoScript = infoDino.dino.GetComponent<Dino>();

        _dinos[_currentIdDino].dino.SetActive(true);

        Dino dinoDino = infoDino.dino.GetComponent<Dino>();
        OnSpawn?.Invoke(dinoDino, _spawnerSeat.transform);

        return infoDino.characteristic;
    }

    DinoCharacteristic PrevDino()
    {
        if (_dinos.Count == _nbDinoPlaced) { return null; }

        _dinos[_currentIdDino].dino.SetActive(false);

        if(!_isInfoDinoAlreadyPlaced)
        {
            do
            {
                _currentIdDino--;

                if (_currentIdDino == -1)
                {
                    _currentIdDino = _dinos.Count - 1;
                }
            } while (_dinos[_currentIdDino].isPlace);
        }
        else
        {
            _isInfoDinoAlreadyPlaced = false;
        }

        InfoDino infoDino = _dinos[_currentIdDino];

        Dino dinoScript = infoDino.dino.GetComponent<Dino>();

        _dinos[_currentIdDino].dino.SetActive(true);

        Dino dinoDino = infoDino.dino.GetComponent<Dino>();
        OnSpawn?.Invoke(dinoDino, _spawnerSeat.transform);

        return infoDino.characteristic;
    }

    void DinoClicked(Dino dino)
    {
        Debug.Log("dino click");

        foreach (var tryDino in _dinos)
        {
            if (tryDino.dino == dino.gameObject)
            {
                Debug.Log("dino trouvé");
                if (tryDino.isPlace == true)
                {
                    _UIInfoDino.PreciseDino(tryDino.characteristic,tryDino.isPlace);
                    _isInfoDinoAlreadyPlaced = true;
                }
                return;
            }
        }
    }
}