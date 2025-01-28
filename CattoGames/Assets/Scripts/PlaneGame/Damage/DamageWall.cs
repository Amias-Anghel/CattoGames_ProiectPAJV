using UnityEngine;
using Fusion;


public class DamageWall : NetworkBehaviour
{
    private Rigidbody2D _rigidbody;
    [SerializeField] float _movementSpeed = 20f;

    [Networked] private float _screenBoundaryX { get; set; }

    public override void Spawned()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        if (Object.HasStateAuthority) {
            _screenBoundaryX = Camera.main.orthographicSize * Camera.main.aspect;
        }
    }

    public override void FixedUpdateNetwork() {
        if (!Object.HasStateAuthority) return;

        _rigidbody.MovePosition(_rigidbody.position + _movementSpeed * Vector2.left * Runner.DeltaTime);

        if (CheckExitBounds()) {
            Runner.Despawn(Object);
        }
    }

    private bool CheckExitBounds()
    {
        var position = _rigidbody.position;

        if (Mathf.Abs(position.x) < _screenBoundaryX + 10) return false;

        return true;
    }

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
