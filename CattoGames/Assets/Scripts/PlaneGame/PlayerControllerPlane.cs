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

        _lagCompensatedHits.Clear();

        int count = Runner.LagCompensation.OverlapSphere(_rigidbody.position, _playerRadius,
            Object.InputAuthority, _lagCompensatedHits, _damageCollisionLayer.value);

        if (count <= 0) return;

        PlayerDied();

        return;
    }

    public bool AcceptInput() {
        return playerDataNetworkedPlane.Alive && Object.IsValid;
    }

    private void PlayerDied() {
        gameStateController.CheckIfGameHasEnded();
        Runner.Despawn(Object);
    }
}

