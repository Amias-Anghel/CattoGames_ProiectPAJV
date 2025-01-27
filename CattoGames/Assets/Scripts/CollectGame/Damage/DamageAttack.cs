using UnityEngine;
using Fusion;

public class DamageAttack : NetworkBehaviour
{
    [Networked] private TickTimer _despawnTimer { get; set; }
    [Networked] private NetworkBool expired { get; set; }

    public override void Spawned()
    {
        expired = false;

        if (!Object.HasStateAuthority) return;

        _despawnTimer = TickTimer.CreateFromSeconds(Runner, 4f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<PlayerCollectController>().Call_RPC_CollisionDetected(true); 
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<PlayerCollectController>().Call_RPC_CollisionDetected(false);
        }
    }
    
    public override void FixedUpdateNetwork()
    {
       CheckZoneExpiration();
    }

    private void CheckZoneExpiration() {
        if(Object == null) return;
        if(!Object.HasStateAuthority) return;
        if(expired) return;

        if(!_despawnTimer.Expired(Runner)) return;
        expired = true;
    }

    public override void Render()
    {
        if (expired)
        {
           Runner.Despawn(Object);
        }
    }
}
