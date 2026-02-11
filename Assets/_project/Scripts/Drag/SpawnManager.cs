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
                    Dino dinoDino = dino.GetComponent<Dino>();
                    OnSpawn?.Invoke(dinoDino, _spawnerSeat.transform);
                }
                else
                {
                    dino.SetActive(false);
                }

                _dinos.Add(dino);
            }

            _UIInfoDino.NextDino(false);
        }
    }

    private void Update()
    {
        if (_currentIdDino == -1) { return; }

        if (_dinos.Count == 0)
        {
            return;
        }

        if (_spawnerSeat != null && _spawnerSeat.occupant == null)
        {
            _dinos.RemoveAt(_currentIdDino);
            _UIInfoDino.NextDino(true);
        }
    }

    InfoDino NextDino(bool forced)
    {
        if (_dinos.Count == 0) { return null; }

        if (!forced && _dinos.Count >= 3 && _currentIdDino != -1)
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
        if (_dinos.Count == 0) { return null; }

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