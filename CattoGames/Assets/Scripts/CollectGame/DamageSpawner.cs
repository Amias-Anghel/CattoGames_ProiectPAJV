using UnityEngine;
using Fusion;

public class DamageSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef damagePrefab = NetworkPrefabRef.Empty;
    private float _screenBoundaryX = 0.0f;
    private float _screenBoundaryY = 0.0f;
    private TickTimer _spawnDelay;
    private float spawnDelayTime = 5.0f;

    public void StartDamageZoneSpawner() {
        if (!Object.HasStateAuthority) return;

       SetSpawnDelay();
        
        _screenBoundaryX = Camera.main.orthographicSize * Camera.main.aspect;
        _screenBoundaryY = Camera.main.orthographicSize;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        SpawnZones();
    }

    private void SetSpawnDelay() {
        _spawnDelay = TickTimer.CreateFromSeconds(Runner, spawnDelayTime);
        if (spawnDelayTime > 0.5f) {
            spawnDelayTime -= 0.1f;
        }
    }

    private void SpawnZones() {
        if(!_spawnDelay.Expired(Runner)) return;

        for(int i = 0; i < 5; i++) {
            SpawnDamageZone();
        }
        SetSpawnDelay();
    }

    private void SpawnDamageZone()
    {
        Vector3 position;
        position.x = Random.Range(-_screenBoundaryX, _screenBoundaryX);
        position.y = Random.Range(-_screenBoundaryY, _screenBoundaryY);
        position.z = 0;

        position -= position.normalized * 0.2f;

        Runner.Spawn(damagePrefab, position, Quaternion.identity);
    }
}
