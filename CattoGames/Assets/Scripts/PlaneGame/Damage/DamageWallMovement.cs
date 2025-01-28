using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;

public class DamageWallMovement : NetworkBehaviour
{
    private NetworkRigidbody2D _rigidbody;
    [SerializeField] float _movementSpeed = 50f;

    [Networked] private float _screenBoundaryX { get; set; }


    public override void Spawned()
    {
        _rigidbody = GetComponent<NetworkRigidbody2D>();

        if (Object.HasStateAuthority) {
            _screenBoundaryX = Camera.main.orthographicSize * Camera.main.aspect;
        }
    }

    public override void FixedUpdateNetwork() {
        if (!Object.HasStateAuthority) return;

        _rigidbody.Rigidbody.MovePosition(_rigidbody.Rigidbody.position + _movementSpeed * Vector2.left * Runner.DeltaTime);

        if (CheckExitBounds()) {
            Runner.Despawn(Object);
        }
    }

    private bool CheckExitBounds()
    {
        var position = _rigidbody.transform.position;

        if (Mathf.Abs(position.x) < _screenBoundaryX + 20) return false;

        return true;
    }
}
