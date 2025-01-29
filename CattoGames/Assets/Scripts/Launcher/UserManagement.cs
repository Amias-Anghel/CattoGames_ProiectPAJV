using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class UserManagement
{
    
//trebuie facute butoane in unityt 
// trebuie facut login cu email 
    public static void LoginWithEmailAddress(string email, string password) {
        UserData ud = UserData.getInstance();
        PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest() {
            Email = email,
            Password = password,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams() {
                GetPlayerProfile = true,
                GetUserData = true,
                GetUserAccountInfo = true
            }
        }, result => {
            Debug.LogFormat("LoginEmailAddress {0}", result.InfoResultPayload);
            ud.playerProfile = result.InfoResultPayload.PlayerProfile;
            ud.userData = result.InfoResultPayload.UserData;
            ud.playfabID = result.PlayFabId;
           // UserManagement.getLeaderboard();
           // UserManagement.getUserStatistics();
           // UserManagement.getPlayerReadOnlyData();
            ud.startGame();
        }, error => {
            Debug.LogFormat("LoginEmailAddress {0} {1}", error.ErrorMessage, error.Error);
        });
    }


    //optional
    public static void Register(string email, string password, string username) {
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest(){
            Email = email,
            Password = password,
            Username = username,
            DisplayName = username
        }, result => {
            Debug.LogFormat("Register {0}", result.PlayFabId);
            UpdatePlayerData("1429");
            LoginWithEmailAddress(email, password);
        }, error => {
            Debug.LogFormat("Register {0} {1}: {2}, {3}, {4}", error.ErrorMessage, error.Error, email, password, username);
        });
    }

    
    public static void UpdatePlayerData(string avatarID) {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest(){
            Data = new Dictionary<string, string>(){
                {"avatarID", avatarID}
            }
        }, result=> {
            Debug.LogFormat("Update Player Data");
        }, error => {
            Debug.LogFormat("Update Player Data {0} {1}", error.ErrorMessage, error.Error);
        });
    }


    public static void getUserStatistics() {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(){
            StatisticNames = new List<string> { "timesLoggedIn" }
        }, result=> {
            Debug.LogFormat("Get User Statistics" + result.Statistics.Count);
            UserData.getInstance().statistics = result.Statistics;
        }, error => {
            Debug.LogFormat("Get User Statistics {0} {1}", error.ErrorMessage, error.Error);
        });
    }
    
    public static void SavePlayerReadOnlyData(float timeActive) {
        if (string.IsNullOrEmpty(PlayFabSettings.staticPlayer.PlayFabId)) {
        Debug.LogError("Player is not logged in! Cannot save data.");
        return;
    }
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest(){
            FunctionName = "savePlayer",
            FunctionParameter = new {
                timeActive = timeActive
            },
            GeneratePlayStreamEvent = true
        }, result => {
            Debug.LogFormat("Read Only Data {0}", timeActive);
        }, error => {
            Debug.LogFormat("Read Only Data {0} {1}", error.ErrorMessage, error.Error);
        });
    }

    public static void getPlayerReadOnlyData(){
        PlayFabClientAPI.GetUserReadOnlyData(new GetUserDataRequest(){
            PlayFabId = UserData.getInstance().playfabID,
        },
        result => {
            foreach(var item in result.Data) {
                Debug.Log(item.Key + " " + item.Value);
            }
                Debug.Log("Time active: "+result.Data["timeActive"].Value);
                UserData.getInstance().timeActive = float.Parse(result.Data["timeActive"].Value);
        },
        error => {
            Debug.Log("Got error getting read-only user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public static void sendLeaderboard(int score) {
        var request = new UpdatePlayerStatisticsRequest{
            Statistics = new List<StatisticUpdate>{
                new StatisticUpdate{
                    StatisticName = "GameScore",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, 
        result=>{
            Debug.LogFormat("Leaderboard sent");
        }, error=> {
            Debug.LogFormat("Update Leaderboard {0} {1}", error.ErrorMessage, error.Error);

        });
    }

    public static void getLeaderboard() {
        var request = new GetLeaderboardRequest {
            StatisticName = "GameScore",
            StartPosition = 0,
            MaxResultsCount = 5
        };
        PlayFabClientAPI.GetLeaderboard(request, 
        result => {
            Debug.LogFormat("Leaderboard received " + result.Leaderboard.Count);
            UserData.getInstance().leaderboard = result;
        }, error => {
            Debug.LogFormat("Get Leaderboard {0} {1}", error.ErrorMessage, error.Error);
        });
    }
}
