
using Fusion;
using UnityEngine;

public class PlayerLobbySpawn : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkPrefabRef _playerNetworkPrefab = NetworkPrefabRef.Empty;

    public override void Spawned()
    {
        
    }

    public void PlayerJoined(PlayerRef player)
    {
        SpawnPlayer(player);
    }

    public void PlayerLeft(PlayerRef player)
    {
        DespawnPlayer(player);
    }

    private void SpawnPlayer(PlayerRef player)
    {
        var playerObject = Runner.Spawn(_playerNetworkPrefab, Vector3.one, Quaternion.identity, player);
        playerObject.transform.localScale = Vector3.one;
        Runner.SetPlayerObject(player, playerObject);
    }

    private void DespawnPlayer(PlayerRef player)
    {
        if (Runner.TryGetPlayerObject(player, out var playerNetworkObject))
        {
            Runner.Despawn(playerNetworkObject);
        }

        Runner.SetPlayerObject(player, null);
    }
}
