using Fusion;
using UnityEngine;
using System;

public class CollectableSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef eggPrefab = NetworkPrefabRef.Empty;

    private float _screenBoundaryX = 0.0f;
    private float _screenBoundaryY = 0.0f;

    public static Action SpawnNewCollectable;

    public void StartCollectableSpawn()
    {
        SpawnNewCollectable += SpawnCollectable;
        if (!Object.HasStateAuthority) return;

        _screenBoundaryX = Camera.main.orthographicSize * Camera.main.aspect;
        _screenBoundaryY = Camera.main.orthographicSize;

        for (int i = 0; i < 5; i++) {
            SpawnCollectable();
        }
    }

    private void SpawnCollectable()
    {
        Vector3 position;
        position.x = UnityEngine.Random.Range(-_screenBoundaryX, _screenBoundaryX);
        position.y = UnityEngine.Random.Range(-_screenBoundaryY, _screenBoundaryY);
        position.z = 0;

        position -= position.normalized * 0.2f;
        if (Runner != null) {
            Runner.Spawn(eggPrefab, position, Quaternion.identity);
        }
    }
}
