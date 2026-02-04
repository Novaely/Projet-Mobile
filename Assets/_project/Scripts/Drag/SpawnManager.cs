using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [SerializeField] Seat _spawnerSeat; 
    [SerializeField] List<GameObject> _dinos;
    [SerializeField] Transform _dinosContainer;

    public event Action <Dino, Transform> OnSpawn;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        if (_dinos.Count == 0) return;
        if (_spawnerSeat != null && _spawnerSeat.occupant == null)
        {
            GameObject item = Instantiate(_dinos[0], _spawnerSeat.transform.position, Quaternion.identity, _dinosContainer);
            Dino dino = item.GetComponent<Dino>();
            OnSpawn?.Invoke(dino, _spawnerSeat.transform);
            _dinos.RemoveAt(0);
        }
    }
}