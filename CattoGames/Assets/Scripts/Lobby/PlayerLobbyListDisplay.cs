using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class PlayerLobbyListDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _playerOverviewEntryPrefab = null;
    private Dictionary<PlayerRef, PlayerLobby> 
        _playerLobbyList = new Dictionary<PlayerRef, PlayerLobby>();
    private Dictionary<PlayerRef, GameObject> 
        _playerListEntries = new Dictionary<PlayerRef, GameObject>();
    
    private Dictionary<PlayerRef, string> _playerNickNames = new Dictionary<PlayerRef, string>();

    public void AddEntry(PlayerRef playerRef, PlayerLobby playerLobby)
    {
        if (_playerListEntries.ContainsKey(playerRef)) return;

        var entry = Instantiate(_playerOverviewEntryPrefab, this.transform);
        entry.transform.localScale = Vector3.one;
        playerLobby.SetText(entry.transform.GetChild(0).GetComponent<TMP_Text>());
        string nickName = string.Empty;

        _playerNickNames.Add(playerRef, nickName);
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
        _playerListEntries.Remove(playerRef);
        _playerLobbyList.Remove(playerRef);
    }

    public void UpdateNickName(PlayerRef player, string nickName)
    {
        if (_playerLobbyList.TryGetValue(player, out var entry) == false) return;

        _playerNickNames[player] = nickName;
        UpdateEntry(player, entry);
    }

    private void UpdateEntry(PlayerRef player, PlayerLobby entry)
    {
        var nickName = _playerNickNames[player];

        entry.SetPlayerName($"{nickName}");
    }
}
