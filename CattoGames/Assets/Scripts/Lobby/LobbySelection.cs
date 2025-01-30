using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;
using System;
using WebSocketSharp;

public class LobbySelection : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown roomModeDropDown = null;
    [SerializeField] private TMP_Dropdown gameModeDropDown = null;
    [SerializeField] private GameObject joinPrivateMenu = null;
    [SerializeField] private GameObject createRoomMenu = null;
    [SerializeField] private TMP_InputField joinRoomName = null;
    [SerializeField] private TMP_InputField createRoomName = null;
    [SerializeField] private Toggle privateRoomToggle = null;
    [SerializeField] private TMP_Text playerNameDisplay = null;
    [SerializeField] private Image playerBackgroundDisplay = null;
    [SerializeField] private Slider playerCountSlider = null;
    [SerializeField] private TMP_Text playerCountText = null;

    private int gameModeOption;
    private string gameChoice;

    private NetworkRunner runner;
    [SerializeField] private NetworkRunner networkRunnerPrefab = null;
    [SerializeField] private PlayerData playerDataPrefab = null;

    void Start() {
        playerCountSlider.value = 2;
        playerCountText.text = "Player Limit: 2";

        joinPrivateMenu.SetActive(false);
        createRoomMenu.SetActive(false);

        gameModeOption = roomModeDropDown.value;
        gameChoice = "Treat Collector";

        runner = FindObjectOfType<NetworkRunner>();
        if (runner == null)
        {
            runner = Instantiate(networkRunnerPrefab);
        }

        SetPlayerData();
    }

    private void SetPlayerData() {
        var playerData = FindObjectOfType<PlayerData>();
        if (playerData == null)
        {
            playerData = Instantiate(playerDataPrefab);
        }

        playerNameDisplay.text = $"Joining as {playerData.GetNickName()}";
        playerBackgroundDisplay.sprite = SpriteManager.getInstance().getSelectedAvatar().backgroundSprite;
    }
    
    public void OnGameChange() {
        switch(gameModeDropDown.value) {
            case 0: //"Treat Collector":
                gameChoice = "Treat Collector";
                break;
            case 1://"Plane Team":
                gameChoice = "Plane Team";
                break;
        }
    }

    public void OnPlayerCountChange() {
        playerCountText.text = $"Players Limit: {playerCountSlider.value}";
    }

    public void OnJoinModeChange() {
        gameModeOption = roomModeDropDown.value;
        
        switch(gameModeOption) {
            case 0: //"Join Random":
                joinPrivateMenu.SetActive(false);
                createRoomMenu.SetActive(false);
                break;
            case 1://"Join Private":
                joinPrivateMenu.SetActive(true);
                createRoomMenu.SetActive(false);
                break;
            case 2: //"Create Lobby":
                joinPrivateMenu.SetActive(false);
                createRoomMenu.SetActive(true);
                break;
        }
    }

    public void EnterLobby() {
        
        switch(gameModeOption) {
            case 0: //"Join Random":
                JoinRandomPublicSession();
                break;
            case 1://"Join Private":
                JoinPrivateSession(joinRoomName.text);
                break;
            case 2: //"Create Lobby":
                CreateSession(createRoomName.text, privateRoomToggle.isOn);
                break;
        }
    }

    public async void JoinRandomPublicSession()
    {
        // Attempt to join the first available public session
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            MatchmakingMode = Fusion.Photon.Realtime.MatchmakingMode.RandomMatching,

            SessionProperties = new Dictionary<string, SessionProperty>
            {
                { "game", gameChoice},
                { "isPrivate", false}
            },
        });

        if (runner.IsServer) {
            await runner.LoadScene("Lobby");
        }
    }

    public static string GetRandomRoomName()
    {
        var rngPlayerNumber = UnityEngine.Random.Range(0, 9999);
        return $"Room{rngPlayerNumber.ToString("0000")}";
    }

    public async void JoinPrivateSession(string sessionName)
    {
        if (sessionName.IsNullOrEmpty()) {
            JoinRandomPublicSession();
        }

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = sessionName,
        });
    }

    public async void CreateSession(string sessionName, bool isPrivate)
    {
        if (sessionName.IsNullOrEmpty()) {
            sessionName = GetRandomRoomName();
        }

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = sessionName,
            PlayerCount = (int)playerCountSlider.value,
            
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                { "game", gameChoice},
                { "isPrivate", privateRoomToggle.isOn}
            },
        });

        if (runner.IsServer) {
            await runner.LoadScene("Lobby");
        }
    }
}
