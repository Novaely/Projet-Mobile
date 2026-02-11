using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [SerializeField] Seat _spawnerSeat;
    [SerializeField] List<GameObject> _dinosPrefab;
    List<GameObject> _dinos = new();
    [SerializeField] Transform _dinosContainer;

    public event Action <Dino, Transform> OnSpawn;

    int _currentIdDino = -1;

    public class InfoDino
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

            _UIInfoDino.OnNextDino += NextDino;
            _UIInfoDino.OnPreviousDino += PrevDino;

            for (int i = 0; i < _dinosPrefab.Count; i++)
            {
                GameObject dino = Instantiate(_dinosPrefab[i], _spawnerSeat.transform.position, Quaternion.identity, _dinosContainer);

                if (i == 0)
                {
                    dino.SetActive(false);
                    Dino dinoDino = dino.GetComponent<Dino>();
                    OnSpawn?.Invoke(dinoDino, _spawnerSeat.transform);
                }

                _dinos.Add(dino);
            }

            _UIInfoDino.NextDino();
        }
    }

    private void Update()
    {
        if (_currentIdDino == -1) { return; }

        if (_dinos.Count == 1)
        {
            return;
        }

        if (_spawnerSeat != null && _spawnerSeat.occupant == null)
        {
            _dinos.RemoveAt(_currentIdDino);
            _UIInfoDino.NextDino();
        }
    }

    InfoDino NextDino()
    {
        if (_currentIdDino == -1)
        {
            for (int i = 1; i < _dinos.Count; i++)
            {
                _dinos[i].SetActive(false);
            }
        }
        if (_dinos.Count >= 1 && _currentIdDino != -1)
        {
            _dinos[_currentIdDino].SetActive(false);
        }

        _currentIdDino = (_currentIdDino + 1) % _dinos.Count;

        GameObject dino = _dinos[_currentIdDino];

        InfoDino infoDino = new InfoDino
        {
            label = dino.name,
            contraintePositive = "",
            contrainteNegative = "",
            sprite = dino.GetComponent<SpriteRenderer>().sprite,
        };

        _dinos[_currentIdDino].SetActive(true);

        Dino dinoDino = dino.GetComponent<Dino>();
        OnSpawn?.Invoke(dinoDino, _spawnerSeat.transform);

        return infoDino;
    }

    InfoDino PrevDino()
    {
        _dinos[_currentIdDino].SetActive(false);

        _currentIdDino = (_currentIdDino - 1) % _dinos.Count;

        GameObject dino = _dinos[_currentIdDino];

        InfoDino infoDino = new InfoDino
        {
            label = dino.name,
            contraintePositive = "",
            contrainteNegative = "",
            sprite = dino.GetComponent<SpriteRenderer>().sprite,
        };

        _dinos[_currentIdDino].SetActive(true);

        return infoDino;
    }
}