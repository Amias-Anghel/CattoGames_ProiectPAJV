using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.LagCompensation;
using UnityEngine;


public class PlayerControllerPlane : NetworkBehaviour
{
    [SerializeField] private float _playerRadius = 1f;
    [SerializeField] private LayerMask _damageCollisionLayer;
    private Rigidbody2D _rigidbody = null;
    private List<LagCompensatedHit> _lagCompensatedHits = new List<LagCompensatedHit>();

    private GameStateControllerPlane gameStateController;
    private PlayerDataNetworkedPlane playerDataNetworkedPlane;

    [Networked] private NetworkBool _takeDamage { get; set; }

    public override void Spawned()
    {
        gameStateController = FindObjectOfType<GameStateControllerPlane>();
        playerDataNetworkedPlane = GetComponent<PlayerDataNetworkedPlane>();
        _rigidbody = GetComponent<Rigidbody2D>();
        if (Object.HasStateAuthority == false) return;
    }

    public override void FixedUpdateNetwork()
    {
        CheckDamageZoneCollision();
    }

    private void CheckDamageZoneCollision() {
        if (!Object.HasStateAuthority) return;

        if (_takeDamage) {
            if (Runner.TryGetPlayerObject(Object.InputAuthority, out var playerNetworkObject)) {
                playerNetworkObject.GetComponent<PlayerDataNetworkedPlane>().TakeDamage();

                if (!playerNetworkObject.GetComponent<PlayerDataNetworkedPlane>().Alive) {
                    PlayerDied();
                }
            }
        }

        return;
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

    public bool AcceptInput() {
        return playerDataNetworkedPlane.Alive && Object.IsValid;
    }

    private void PlayerDied() {
        gameStateController.CheckIfGameHasEnded();
        Runner.Despawn(Object);
    }
}

