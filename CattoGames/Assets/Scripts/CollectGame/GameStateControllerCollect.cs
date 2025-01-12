using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameStateControllerCollect : NetworkBehaviour
{
    enum GameState
    {
        Starting,
        Running,
        Ending
    }

    [SerializeField] private TMP_Text endScreenText;

    [Networked] private GameState _gameState { get; set; }

    [Networked] private NetworkBehaviourId _winner { get; set; }
    [Networked] private TickTimer _timer { get; set; }
    [Networked] private TickTimer _timerStart { get; set; }

    private List<NetworkBehaviourId> _playerDataNetworkedIds = new List<NetworkBehaviourId>();

    public override void Spawned()
    {
        endScreenText.text = string.Empty;

        if (_gameState != GameState.Starting)
        {
            foreach (var player in Runner.ActivePlayers)
            {
                if (Runner.TryGetPlayerObject(player, out var playerObject) == false) continue;
                TrackNewPlayer(playerObject.GetComponent<PlayerDataNetworkedCollect>().Id);
            }
        }

        Runner.SetIsSimulated(Object, true);

        if (!Object.HasStateAuthority) return;
        _timerStart = TickTimer.CreateFromSeconds(Runner, 4f);
        _gameState = GameState.Starting;
    }

    public override void FixedUpdateNetwork()
    {
        switch (_gameState)
        {
            case GameState.Starting:
                UpdateStartingDisplay();
                break;
            case GameState.Running:
                break;
            case GameState.Ending:
                UpdateEndingDisplay();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void StartGame() {
        if (!Object.HasStateAuthority) return;
        _gameState = GameState.Starting;
    }

    private void UpdateStartingDisplay()
    {
        endScreenText.text = $"Starting game in {Mathf.RoundToInt(_timerStart.RemainingTime(Runner) ?? 0)}..";

        if (!_timerStart.ExpiredOrNotRunning(Runner)) return;
        endScreenText.text = string.Empty;

        if (!Object.HasStateAuthority) return;

        FindObjectOfType<PlayerSpawnerCollect>().StartPlayerSpawner(this);
        FindObjectOfType<CollectableSpawner>().StartCollectableSpawn();
        FindObjectOfType<DamageSpawner>().StartDamageZoneSpawner();

        _gameState = GameState.Running;
    }

    private void UpdateEndingDisplay()
    {
        if (Runner.TryFindBehaviour(_winner, out PlayerDataNetworkedCollect playerData)) {
            endScreenText.text = $"{playerData.NickName} won with {playerData.Score} points.\nDisconecting...";
        } else {
            endScreenText.text  ="Disconecting...";
        }

        if (_timer.ExpiredOrNotRunning(Runner) == false) return;
 
        Runner.Shutdown();
    }

    // Called from the ShipController when it hits an asteroid
    public void CheckIfGameHasEnded()
    {
        if (Object.HasStateAuthority == false) return;

        int playersAlive = 0;

        for (int i = 0; i < _playerDataNetworkedIds.Count; i++)
        {
            if (Runner.TryFindBehaviour(_playerDataNetworkedIds[i],
                    out PlayerDataNetworkedCollect playerDataNetworkedComponent) == false)
            {
                _playerDataNetworkedIds.RemoveAt(i);
                i--;
                continue;
            }

            if (playerDataNetworkedComponent.Life > 0) playersAlive++;
        }

        // If more than 1 player is left alive, the game continues.
        // If only 1 player is left, the game ends immediately.
        if (playersAlive > 1 || (Runner.ActivePlayers.Count() == 1 && playersAlive == 1)) return;

        foreach (var playerDataNetworkedId in _playerDataNetworkedIds)
        {
            if (Runner.TryFindBehaviour(playerDataNetworkedId,
                    out PlayerDataNetworkedCollect playerDataNetworkedComponent) ==
                false) continue;

            if (playerDataNetworkedComponent.Life <= 0) continue;

            _winner = playerDataNetworkedId;
        }

        GameHasEnded();
    }

    private void GameHasEnded()
    {
        _timer = TickTimer.CreateFromSeconds(Runner, 2f);
        _gameState = GameState.Ending;
    }

    public void TrackNewPlayer(NetworkBehaviourId playerDataNetworkedId)
    {
        _playerDataNetworkedIds.Add(playerDataNetworkedId);
    }
}
