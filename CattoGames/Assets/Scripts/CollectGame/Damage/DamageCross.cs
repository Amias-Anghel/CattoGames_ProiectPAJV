using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class DamageCross : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef damagePrefab = NetworkPrefabRef.Empty;
    private TickTimer _spawnDelay;
    private int index;
    private bool spawning;

    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;
        index = 0;
        spawning = true;
        _spawnDelay = TickTimer.CreateFromSeconds(Runner, 0.4f);
    }


    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        if (spawning) {
            SpawnZones();
        }
    }

    private void SetSpawnDelay() {
        _spawnDelay = TickTimer.CreateFromSeconds(Runner, 0.1f);
    }

    private void SpawnZones() {
        if(!_spawnDelay.Expired(Runner)) return;
        
        for (int i = 0; i < transform.childCount; i++) {
            SpawnDamageZone(transform.GetChild(i).GetChild(index).position);
        }

        index++;
        if (transform.GetChild(0).childCount <= index) {
            spawning = false;
            Runner.Despawn(Object);
        }

        SetSpawnDelay();
    }

    private void SpawnDamageZone(Vector3 position)
    {
        position -= position.normalized;
        Runner.Spawn(damagePrefab, position, Quaternion.identity);
    }
}
