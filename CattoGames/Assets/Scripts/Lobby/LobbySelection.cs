using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbySelection : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown roomModeDropDown;
    [SerializeField] private TMP_Dropdown gameModeDropDown;
    [SerializeField] private Toggle privateRoomToggle;
    [SerializeField] private GameObject joinPrivateMenu;
    [SerializeField] private GameObject createRoomMenu;

    void Start() {
        joinPrivateMenu.SetActive(false);
        createRoomMenu.SetActive(false);
    }

    public void OnJoinModeChange() {
        string roomMode = roomModeDropDown.options[roomModeDropDown.value].text;
        switch(roomMode) {
            case "Join Random":
                joinPrivateMenu.SetActive(false);
                createRoomMenu.SetActive(false);
                break;
            case "Join Private":
                joinPrivateMenu.SetActive(true);
                createRoomMenu.SetActive(false);
                break;
            case "Create Lobby":
                joinPrivateMenu.SetActive(false);
                createRoomMenu.SetActive(true);
                break;
        }
    }
}
