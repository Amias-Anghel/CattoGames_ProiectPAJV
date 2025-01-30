using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using PlayFab.ClientModels;
using UnityEngine;

[System.Serializable]
public struct SpriteItem
{
    public string ID;
    public Sprite profileSprite;
    public Sprite avatarSprite;
    public Sprite planeSprite;
    public Sprite backgroundSprite;
}
public class SpriteManager : MonoBehaviour
{
    private static SpriteManager instance;
    public SpriteItem selectedAvatar;

    private SpriteManager()
    {
    }

    public static SpriteManager getInstance()
    {
        if (instance == null)
        {
            instance = new SpriteManager();
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

    public void setSelectedAvatar(string id){
        for(int i = 0; i < sprites.Length; i++) {
            if(sprites[i].ID.Equals(id)){
                this.selectedAvatar = sprites[i];
            }
        }
    }

    public SpriteItem getSelectedAvatar() {
        return this.selectedAvatar;
    }
    public SpriteItem[] sprites;

    public SpriteItem[] getSprites(){
        return sprites;
    }

    public string[] getIDS() {
        var ids =  sprites
                    .Select(item => item.ID );
        return ids.ToArray();
    }


    public SpriteItem getSprite(string index) {
        for(int i = 0; i < sprites.Length; i++) {
            if(sprites[i].ID.Equals(index)){
                return sprites[i];
            }
        }
        return sprites[0];
    }

    public SpriteItem getNext(string index) {
         for(int i = 0; i < sprites.Length - 1; i++) {
            if(sprites[i].ID.Equals(index)){
                return sprites[i + 1];
            }
        }
        return sprites[0];
    }
       
}
