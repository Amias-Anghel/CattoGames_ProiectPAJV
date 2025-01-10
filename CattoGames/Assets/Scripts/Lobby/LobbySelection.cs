using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;
using System;
using WebSocketSharp;

public class LobbySelection : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private TMP_Dropdown roomModeDropDown = null;
    [SerializeField] private TMP_Dropdown gameModeDropDown = null;
    [SerializeField] private GameObject joinPrivateMenu = null;
    [SerializeField] private GameObject createRoomMenu = null;
    [SerializeField] private TMP_InputField joinRoomName = null;
    [SerializeField] private TMP_InputField createRoomName = null;
    [SerializeField] private Toggle privateRoomToggle = null;
    [SerializeField] private TMP_Text playerNameDisplay = null;

    private int gameModeOption;
    private string gameChoice;

    private NetworkRunner runner;
    [SerializeField] private NetworkRunner networkRunnerPrefab = null;
    [SerializeField] private PlayerData playerDataPrefab = null;
    private List<SessionInfo> publicSessions = new List<SessionInfo>();

    void Start() {
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
        if (publicSessions.Count > 0)
        {
            // Attempt to join the first available public session
            var sessionToJoin = publicSessions[0];
            await runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Client,
                SessionName = sessionToJoin.Name,
            });
        }
        else
        {
            // No public sessions available, create a new one
            CreateSession(GetRandomRoomName(), false);
        }
    }

    public static string GetRandomRoomName()
    {
        var rngPlayerNumber = UnityEngine.Random.Range(0, 9999);
        return $"Room{rngPlayerNumber.ToString("0000")}";
    }

    public async void JoinPrivateSession(string sessionName)
    {
        if (sessionName.IsNullOrEmpty()) return;

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = sessionName,
        });
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        publicSessions = new List<SessionInfo>();

        foreach (var session in sessionList)
        {
            int maxPlayers = 3;
            string game = "Plane Team";

            bool isPrivate = session.Properties.ContainsKey("isPrivate") && (bool)session.Properties["isPrivate"];
            if (session.Properties.ContainsKey("maxPlayers")) {
                maxPlayers = session.Properties["maxPlayers"];
            }

            if (session.Properties.ContainsKey("game")) {
                game = session.Properties["game"];
            }

            if (!isPrivate && session.PlayerCount < maxPlayers && game.ToString().CompareTo(gameChoice) == 0)
            {
                publicSessions.Add(session);
            }
        }
    }

    public async void CreateSession(string sessionName, bool isPrivate)
    {
        if (sessionName.IsNullOrEmpty()) return;

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = sessionName,
            
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                { "isPrivate", isPrivate },
                { "maxPlayers", 3 },
                { "game", gameChoice}
            },
        });

        if (runner.IsServer) {
            await runner.LoadScene("Lobby");
        }
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
}
