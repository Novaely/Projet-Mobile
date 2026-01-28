using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [SerializeField] EmplacementController _spawner;

    [SerializeField] List<GameObject> _dinos;
    [SerializeField] Transform _dinosContainer;

    public event Action <DinoController, Transform> OnSpawn;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (_spawner.Storage == null)
        {
            if (_dinos.Count > 0)
            {
                GameObject item = Instantiate(_dinos[0], _spawner.transform.position, Quaternion.identity, _dinosContainer);
                DinoController dino = item.GetComponent<DinoController>();
                OnSpawn?.Invoke(dino, _spawner.transform);
                _dinos.RemoveAt(0);
            }
        }
    }
}
