using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLobbyListDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _playerOverviewEntryPrefab = null;
    private Dictionary<PlayerRef, PlayerLobby> 
        _playerLobbyList = new Dictionary<PlayerRef, PlayerLobby>();
    private Dictionary<PlayerRef, GameObject> 
        _playerListEntries = new Dictionary<PlayerRef, GameObject>();
    
    private Dictionary<PlayerRef, string> _playerNickNames = new Dictionary<PlayerRef, string>();
    private Dictionary<PlayerRef, string> _playerAvatars = new Dictionary<PlayerRef, string>();


    public void AddEntry(PlayerRef playerRef, PlayerLobby playerLobby)
    {
        if (_playerListEntries.ContainsKey(playerRef)) return;

        var entry = Instantiate(_playerOverviewEntryPrefab, this.transform);
        entry.transform.localScale = Vector3.one;
        playerLobby.SetText(entry.transform.GetChild(0).GetComponent<TMP_Text>());
        playerLobby.SetImage(entry.transform.GetChild(1).GetComponent<Image>());
        string nickName = string.Empty;
        string avatarID = string.Empty;

        _playerNickNames.Add(playerRef, nickName);
        _playerAvatars.Add(playerRef, avatarID);
        _playerListEntries.Add(playerRef, entry);
        _playerLobbyList.Add(playerRef, playerLobby);

        UpdateEntry(playerRef, playerLobby);
    }

    public void RemoveEntry(PlayerRef playerRef)
    {
        if (_playerListEntries.TryGetValue(playerRef, out var entry) == false) return;

        if (entry != null)
        {
            Destroy(entry.gameObject);
        }

        _playerNickNames.Remove(playerRef);
        _playerAvatars.Remove(playerRef);
        _playerListEntries.Remove(playerRef);
        _playerLobbyList.Remove(playerRef);
    }

    public void UpdateData(PlayerRef player, string nickName, string avatarID)
    {
        if (_playerLobbyList.TryGetValue(player, out var entry) == false) return;

        _playerNickNames[player] = nickName;
        _playerAvatars[player] = avatarID;
        UpdateEntry(player, entry);
    }


    private void UpdateEntry(PlayerRef player, PlayerLobby entry)
    {
        var nickName = _playerNickNames[player];
        var avatarID = _playerAvatars[player];

        entry.SetPlayerName($"{nickName}");
        entry.SetPlayerProfile(avatarID);
    }
}
