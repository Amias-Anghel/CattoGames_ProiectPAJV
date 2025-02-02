using Fusion;
using UnityEngine;

public class PlayerSpawnerPlane : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkPrefabRef _playerNetworkPrefab = NetworkPrefabRef.Empty;

    private bool _gameIsReady = false;
    private GameStateControllerPlane _gameStateController = null;
    private GameObject[] _spawnPoints = null;

    public override void Spawned()
    {
        if (Object.HasStateAuthority == false) return;
        _spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }

    public void StartPlayerSpawner(GameStateControllerPlane gameStateController)
    {
        _gameIsReady = true;
        _gameStateController = gameStateController;
        foreach (var player in Runner.ActivePlayers)
        {
            SpawnPlayer(player);
        }
    }

    private void SpawnPlayer(PlayerRef player)
    {
        int index = player.PlayerId % _spawnPoints.Length;
        var spawnPosition = _spawnPoints[index].transform.position;

        var playerObject = Runner.Spawn(_playerNetworkPrefab, spawnPosition, Quaternion.identity, player);
        Runner.SetPlayerObject(player, playerObject);

        _gameStateController.TrackNewPlayer(playerObject.GetComponent<PlayerDataNetworkedPlane>().Id);
    }

    private void DespawnPlayer(PlayerRef player)
    {
        if (Runner.TryGetPlayerObject(player, out var playerNetworkObject))
        {
            Runner.Despawn(playerNetworkObject);
        }

        Runner.SetPlayerObject(player, null);
    }
    
    public void PlayerJoined(PlayerRef player)
    {
        if (_gameIsReady == false) return;
        SpawnPlayer(player);
    }

    public void PlayerLeft(PlayerRef player)
    {
        DespawnPlayer(player);
    }
}
