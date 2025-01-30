using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShowUserDetails : MonoBehaviour
{
    private UserData ud;
    private PlayerProfileModel playerProfile;
    private string avatarID;
    public TextMeshProUGUI usernameDisplay;
    public GameObject avatarDisplay;
    public Image background;
    public SpriteManager sm;



    // Start is called before the first frame update
    void Start()
    {
        ud = UserData.getInstance();
        playerProfile = ud.playerProfile;
       
        avatarID = ud.userData["avatarID"].Value;
        sm.setSelectedAvatar(avatarID);
    
        setUserData();
        setUserAvatar();
    }

    void setUserData(){
        usernameDisplay.text = "Welcome, " + playerProfile.DisplayName + "!";
        for(int i = 0; i < ud.statistics.Count; i++)
        {
            Debug.Log(ud.statistics[i]);
        }
       
        
    }

    void setUserAvatar() {
        avatarDisplay.GetComponent<Image>().sprite = sm.getSelectedAvatar().profileSprite;
        background.sprite = sm.getSelectedAvatar().backgroundSprite;
    }

    public void updateData(){
        setUserAvatar();
    }

    public void enterRoom(){
        SceneManager.LoadScene("RoomMenu");
    }
}
