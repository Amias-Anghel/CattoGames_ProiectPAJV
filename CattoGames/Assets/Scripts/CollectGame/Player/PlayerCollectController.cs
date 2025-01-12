using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.LagCompensation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCollectController : NetworkBehaviour
{
    [SerializeField] private float _playerRadius = 2f;
    [SerializeField] private LayerMask _damageCollisionLayer;
    [SerializeField] private LayerMask _collectCollisionLayer;

    private Rigidbody2D _rigidbody = null;
    private List<LagCompensatedHit> _lagCompensatedHits = new List<LagCompensatedHit>();
    public bool AcceptInput => _isAlive && Object.IsValid;

    private GameStateControllerCollect gameStateController;

    [Networked] private NetworkBool _isAlive { get; set; }
    [Networked] private NetworkBool _takeDamage { get; set; }

    public override void Spawned()
    {
        gameStateController = FindObjectOfType<GameStateControllerCollect>();
        _rigidbody = GetComponent<Rigidbody2D>();
        if (Object.HasStateAuthority == false) return;
        _isAlive = true;
    }

    public override void FixedUpdateNetwork()
    {
        CheckCollidedWithEgg();
        CheckDamageZoneCollision();
    }

    private bool CheckCollidedWithEgg()
    {
        _lagCompensatedHits.Clear();

        int count = Runner.LagCompensation.OverlapSphere(_rigidbody.position, _playerRadius,
            Object.InputAuthority, _lagCompensatedHits, _collectCollisionLayer.value);

        if (count <= 0) return false;

        _lagCompensatedHits.SortDistance();

        Collectable collectable = _lagCompensatedHits[0].GameObject.GetComponent<Collectable>();

        collectable.Collect(Object.InputAuthority);

        return true;
    }

    public void Call_RPC_CollisionDetected(bool takingDamage)
    {
        if (Object != null && Object.HasInputAuthority) {
            RPC_CollisionDetected(takingDamage);
        }
    }


    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_CollisionDetected(bool takingDamage)
    {
        _takeDamage = takingDamage;
        Debug.Log("Collision detected with player across network.");
    }

    private void CheckDamageZoneCollision() {
        if (!Object.HasStateAuthority) return;

        if (_takeDamage) {
            if (Runner.TryGetPlayerObject(Object.InputAuthority, out var playerNetworkObject)) {
                playerNetworkObject.GetComponent<PlayerDataNetworkedCollect>().TakeDamage();

                if (!playerNetworkObject.GetComponent<PlayerDataNetworkedCollect>().IsAlive()) {
                    PlayerDied();
                }
            }
        }
    }

    private void PlayerDied() {
        gameStateController.CheckIfGameHasEnded();
        Runner.Despawn(Object);
    }
}
