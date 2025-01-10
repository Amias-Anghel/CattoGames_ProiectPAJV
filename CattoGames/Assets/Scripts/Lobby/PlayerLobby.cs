using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class PlayerLobby : NetworkBehaviour
{
    [SerializeField] private TMP_Text text;

    private ChangeDetector _changeDetector;
    private PlayerLobbyListDisplay _playerLobbyDisplay;

    [HideInInspector] [Networked]
    public NetworkString<_16> PlayerName { get; private set; }

    public void SetPlayerName(string playerName) {
        text.text = playerName;
    }

    public void SetText(TMP_Text tMP_Text) {
        text = tMP_Text;
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            var nickName = FindObjectOfType<PlayerData>().GetNickName();
            RpcSetNickName(nickName);
        }

        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        _playerLobbyDisplay = FindObjectOfType<PlayerLobbyListDisplay>();
        _playerLobbyDisplay.AddEntry(Object.InputAuthority, this);
        _playerLobbyDisplay.UpdateNickName(Object.InputAuthority, PlayerName.ToString());
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        _playerLobbyDisplay.RemoveEntry(Object.InputAuthority);
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(PlayerName):
                    SetPlayerName(PlayerName.ToString());
                    break;
            }
        }
    }

    // RPC used to send player information to the Host
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetNickName(string playerName)
    {
        if (string.IsNullOrEmpty(playerName)) return;
        PlayerName = playerName;
    }
}
