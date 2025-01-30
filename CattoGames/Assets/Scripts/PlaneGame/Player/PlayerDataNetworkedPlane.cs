using Fusion;
using TMPro;
using UnityEngine;


public class PlayerDataNetworkedPlane : NetworkBehaviour
{    
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private SpriteRenderer visual;
    private ChangeDetector _changeDetector;

    [HideInInspector]
    [Networked]
    public NetworkString<_16> NickName { get; private set; }

    [HideInInspector]
    [Networked]
    public NetworkBool Alive { get; private set; }

    [HideInInspector] [Networked]
    public NetworkString<_16> AvatarId { get; private set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            var nickName = FindObjectOfType<PlayerData>().GetNickName();
            RpcSetNickName(nickName);

            var avatarId = FindObjectOfType<PlayerData>().getAvatarID();
            RpcSetAvatarId(avatarId);
        }

        if (Object.HasStateAuthority)
        {
            Alive = true;
        }

        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        UpdateDisplayName(NickName.ToString());
        UpdatePlayerVisual(AvatarId.ToString());
    }
    
    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(NickName):
                    UpdateDisplayName(NickName.ToString());
                    break;
                case nameof(AvatarId):
                    UpdatePlayerVisual(AvatarId.ToString());
                    break;
            }
        }
    }

    public void TakeDamage() {
        Alive = false;
    }

    public void UpdateDisplayName(string newDisplayName) {
        playerName.text = newDisplayName;
    }

    public void UpdatePlayerVisual(string avatarId) {
        visual.sprite = SpriteManager.getInstance().getSprite(avatarId).planeSprite;
    }


    // RPC used to send player information to the Host
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetNickName(string nickName)
    {
        if (string.IsNullOrEmpty(nickName)) return;
        NickName = nickName;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetAvatarId(string avatarId)
    {
        if (string.IsNullOrEmpty(avatarId)) return;
        AvatarId = avatarId;
    }
}
