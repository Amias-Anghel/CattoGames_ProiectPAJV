using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeAvatar : MonoBehaviour
{
    public Image avatarView;
    public Image profileView;
    public Image planeView;
    private string avatarID;
    private UserData ud;
    private SpriteManager sm;
    // Start is called before the first frame update
    void Start()
    {
        ud = UserData.getInstance();
        sm = SpriteManager.getInstance();
        this.avatarID = sm.getSelectedAvatar().ID;
        avatarView.sprite = sm.getSelectedAvatar().avatarSprite;
        profileView.sprite = sm.getSelectedAvatar().profileSprite;
        planeView.sprite = sm.getSelectedAvatar().planeSprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            SpriteItem si = sm.getNext(avatarID); 
            avatarView.sprite = si.avatarSprite;
            profileView.sprite = si.profileSprite;
            planeView.sprite = si.planeSprite;
            this.avatarID = si.ID;
        }
    }
    public void selectAvatar() {
        UserManagement.UpdatePlayerData(this.avatarID);
        sm.setSelectedAvatar(this.avatarID);
    }
}
