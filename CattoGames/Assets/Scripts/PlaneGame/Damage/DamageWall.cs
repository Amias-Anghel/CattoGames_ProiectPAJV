using UnityEngine;
using Fusion;

public class DamageWall : NetworkBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<PlayerControllerPlane>().Call_RPC_CollisionDetected(true); 
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<PlayerControllerPlane>().Call_RPC_CollisionDetected(false);
        }
    }
}
