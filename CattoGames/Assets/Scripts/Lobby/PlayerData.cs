
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private string _nickName = null;
    private string avatarID = null;
    private UserData ud;

    private void Start()
    {
        var count = FindObjectsOfType<PlayerData>().Length;
        if (count > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Awake() {
        ud = UserData.getInstance();
        _nickName = ud.playerProfile.DisplayName;
        avatarID = SpriteManager.getInstance().getSelectedAvatar().ID;
        Debug.Log(avatarID);
    }

    // call with log in player name
    public void SetNickName(string nickName)
    {
        _nickName = nickName;
    }

    public string GetNickName()
    {
        
        if (string.IsNullOrWhiteSpace(_nickName))
        {
            _nickName = GetRandomNickName();
        }

        return _nickName;
    }

    public string getAvatarID() {
        return avatarID;
    }

    public static string GetRandomNickName()
    {
        var rngPlayerNumber = Random.Range(0, 9999);
        return $"Player {rngPlayerNumber.ToString("0000")}";
    }
}
