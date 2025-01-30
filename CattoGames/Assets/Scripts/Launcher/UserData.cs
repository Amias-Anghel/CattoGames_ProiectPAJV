
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserData : MonoBehaviour
{
    public string playfabID;
    private static UserData instance;
    public PlayerProfileModel playerProfile = new PlayerProfileModel();
    public Dictionary<string, UserDataRecord> userData;
    public GetLeaderboardResult leaderboard; 
    public List<StatisticValue> statistics;
    public float timeActive;
    public float timeInCurrentGame;
    private UserData()
    {
    }

    public static UserData getInstance()
    {
        if (instance == null)
        {
            instance = new UserData();
        }
        return instance;
    }

    private void Awake() {
        DontDestroyOnLoad(this);
       if(instance != null && instance != this) {
            Destroy(this);
       } else {
        instance = this;
       }
    }

    private void OnDisable() {
        float time = timeActive + timeInCurrentGame;
        UserManagement.SavePlayerReadOnlyData(time);
        Debug.Log(time + " --- " + timeActive);
    }
    public void startGame() {
        
        SceneManager.LoadScene("MainScreen");
    }
}
