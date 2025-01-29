using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using TreeEditor;

public class PlayerLobby : NetworkBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image profile;

    private ChangeDetector _changeDetector;
    private PlayerLobbyListDisplay _playerLobbyDisplay;

    [HideInInspector] [Networked]
    public NetworkString<_16> PlayerName { get; private set; }

    [HideInInspector] [Networked]
    public NetworkString<_16> PlayerProfile { get; private set; }

    public void SetPlayerName(string playerName) {
        text.text = playerName;
    }

    public void SetPlayerProfile(string playerProfile) {
        profile.sprite = SpriteManager.getInstance().getSprite(playerProfile).profileSprite;
    }

    public void SetText(TMP_Text tMP_Text) {
        text = tMP_Text;
    }

    public void SetImage(Image image) {
        profile = image;
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            var nickName = FindObjectOfType<PlayerData>().GetNickName();
            var avatarID = FindObjectOfType<PlayerData>().getAvatarID();

            RpcSetData(nickName, avatarID);
        }

        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        _playerLobbyDisplay = FindObjectOfType<PlayerLobbyListDisplay>();
        _playerLobbyDisplay.AddEntry(Object.InputAuthority, this);
        _playerLobbyDisplay.UpdateData(Object.InputAuthority, PlayerName.ToString(), PlayerProfile.ToString());

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
    private void RpcSetData(string playerName, string avatarID)
    {
        if (string.IsNullOrEmpty(playerName)) return;
        if (string.IsNullOrEmpty(avatarID)) return;
       
        PlayerName = playerName;
        PlayerProfile = avatarID;
    }
}
