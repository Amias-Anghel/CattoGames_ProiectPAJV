using UnityEngine;
using Fusion;

public class DamageAttack : NetworkBehaviour
{
    [Networked] private TickTimer _despawnTimer { get; set; }
    [Networked] private TickTimer _damageTimer { get; set; }
    [Networked] private NetworkBool expired { get; set; }
    [Networked] private NetworkBool givesDamage { get; set; }

    public override void Spawned()
    {
        expired = false;
        givesDamage = false;

        if (!Object.HasStateAuthority) return;

        _despawnTimer = TickTimer.CreateFromSeconds(Runner, 4f);
        _damageTimer = TickTimer.CreateFromSeconds(Runner, 2f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger area.");
            collision.GetComponent<PlayerCollectController>().Call_RPC_CollisionDetected(true); 
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player exited the trigger area.");
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
