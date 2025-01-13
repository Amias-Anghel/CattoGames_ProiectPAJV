using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHostOptions : NetworkBehaviour
{
    [SerializeField] private TMP_Text roomName;
    [SerializeField] Button startGameButton;
    [SerializeField] TMP_Dropdown gameModeDropDown;

    private ChangeDetector _changeDetector;

    [Networked]
    public NetworkString<_16> GAME { get; private set; }

    public override void Spawned() {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        roomName.text = $"RoomID: {Runner.SessionInfo.Name}";
        GAME = (string)Runner.SessionInfo.Properties["game"];
        gameModeDropDown.value = GAME.ToString().CompareTo("Treat Collector") == 0 ? 0 : 1;

        if (!Object.HasStateAuthority) {
            startGameButton.gameObject.SetActive(false);
            gameModeDropDown.interactable = false;
        } else {
            startGameButton.interactable = false;
        }
    }

    public override void FixedUpdateNetwork() {
        if (!Object.HasStateAuthority) return;

        if (Runner.ActivePlayers.Count() >= 2) {
            startGameButton.interactable = true;
        }
    }

    public void OnGameChange() {
        if (!Object.HasStateAuthority) return;
        
        switch(gameModeDropDown.value) {
            case 0: //"Treat Collector":
                GAME = "Treat Collector";
                break;
            case 1://"Plane Team":
                GAME = "Plane Team";
                break;
        }
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(GAME):
                    gameModeDropDown.value = GAME.ToString().CompareTo("Treat Collector") == 0 ? 0 : 1;
                    Dictionary<string, SessionProperty> modifiedProps = new Dictionary<string, SessionProperty>
                    {
                        { "game", GAME.ToString() }
                    };
                    Runner.SessionInfo.UpdateCustomProperties(modifiedProps);
                    break;
            }
        }
    }

    public async void StartGame() {
        if (Object.HasStateAuthority) {
            switch(GAME.ToString()) {
                case "Treat Collector":
                    await FindObjectOfType<NetworkRunner>().LoadScene("CollectGame");
                    break;
                case "Plane Team":
                    await FindObjectOfType<NetworkRunner>().LoadScene("PlaneGame");
                    break;
            }
        }
    }

    public void LeaveLobby() {
        Runner.Shutdown();
    }

}
