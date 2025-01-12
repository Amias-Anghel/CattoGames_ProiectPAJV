using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataNetworkedCollect : NetworkBehaviour
{
    [SerializeField] private Slider life = null;
    [SerializeField] private TMP_Text playerName;
    private PlayerOverviewPanel _overviewPanel = null;
    private ChangeDetector _changeDetector;

    [HideInInspector]
    [Networked]
    public NetworkString<_16> NickName { get; private set; }

    [HideInInspector]
    [Networked]
    public int Score { get; private set; }

    [HideInInspector]
    [Networked]
    public float Life { get; private set; }

    

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            var nickName = FindObjectOfType<PlayerData>().GetNickName();
            RpcSetNickName(nickName);
        }

        if (Object.HasStateAuthority)
        {
            Score = 0;
            Life = 1f;
            life.value = Life;
        }

        _overviewPanel = FindObjectOfType<PlayerOverviewPanel>();
        _overviewPanel.AddEntry(Object.InputAuthority, this);
    
        _overviewPanel.UpdateNickName(Object.InputAuthority, NickName.ToString());
        _overviewPanel.UpdateScore(Object.InputAuthority, Score);
        
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }
    
    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(NickName):
                    _overviewPanel.UpdateNickName(Object.InputAuthority, NickName.ToString());
                    break;
                case nameof(Score):
                    _overviewPanel.UpdateScore(Object.InputAuthority, Score);
                    break;
                case nameof(Life):
                    life.value = Life;
                    break;
            }
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        _overviewPanel.RemoveEntry(Object.InputAuthority);
    }

    public void AddToScore(int points)
    {
        Score += points;
    }

    public void TakeDamage() {
        Life -= Runner.DeltaTime;
    }

    public void UpdateDisplayName(string newDisplayName) {
        playerName.text = newDisplayName;
    }

    public bool IsAlive() {
        return Life > 0;
    }

    // RPC used to send player information to the Host
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetNickName(string nickName)
    {
        if (string.IsNullOrEmpty(nickName)) return;
        NickName = nickName;
    }
}
