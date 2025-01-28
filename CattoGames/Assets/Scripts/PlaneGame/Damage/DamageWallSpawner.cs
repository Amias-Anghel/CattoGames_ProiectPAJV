using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class DamageWallSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef wall = NetworkPrefabRef.Empty;
    private TickTimer _spawnDelay;
    private float spawnDelayTime = 2.2f;
    private float xDelay = 20;

    private float _screenBoundaryX = 0.0f;
    private float _screenBoundaryY = 0.0f;

    public void StartWallSpawner() {
        if (!Object.HasStateAuthority) return;

        _spawnDelay = TickTimer.CreateFromSeconds(Runner, spawnDelayTime);

        _screenBoundaryX = Camera.main.orthographicSize * Camera.main.aspect / 2;
        _screenBoundaryY = Camera.main.orthographicSize / 2;

        Runner.Spawn(wall, new Vector2(_screenBoundaryX + xDelay, 0), Quaternion.identity);
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        if(!_spawnDelay.Expired(Runner)) return;
        _spawnDelay = TickTimer.CreateFromSeconds(Runner, spawnDelayTime);

        float yPosition = Random.Range(-_screenBoundaryY, _screenBoundaryY);
        Runner.Spawn(wall, new Vector2(_screenBoundaryX + xDelay, yPosition), Quaternion.identity);
    }
}
