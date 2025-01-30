using Fusion;

public class Collectable : NetworkBehaviour
{
    [Networked] private NetworkBool wasCollected { get; set; }

    public override void Spawned()
    {
        wasCollected = false;
    }

    public void Collect(PlayerRef player) {
        if (Object == null) return;
        if (!Object.HasStateAuthority) return;
        if (wasCollected) return;

        wasCollected = true;
        if (Runner.TryGetPlayerObject(player, out var playerNetworkObject)) {
            playerNetworkObject.GetComponent<PlayerDataNetworkedCollect>().AddToScore(1);
        }
    }

    public override void Render()
    {
        if (wasCollected)
        {
            CollectableSpawner.SpawnNewCollectable?.Invoke();
            Runner.Despawn(Object);
        }
    }
}
