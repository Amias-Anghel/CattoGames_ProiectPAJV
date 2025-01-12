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

        // NetworkedEgg egg = _lagCompensatedHits[0].GameObject.GetComponent<NetworkedEgg>();

        // egg.CollectEgg(Object.InputAuthority);

        return true;
    }

    // Check damage zone collision using a lag compensated OverlapSphere
    private bool CheckDamageZoneCollision()
    {
        _lagCompensatedHits.Clear();

        int count = Runner.LagCompensation.OverlapSphere(_rigidbody.position, _playerRadius,
            Object.InputAuthority, _lagCompensatedHits, _damageCollisionLayer.value);

        if (count <= 0) return false;

        _lagCompensatedHits.SortDistance();

        // DamageZone zone = _lagCompensatedHits[0].GameObject.GetComponent<DamageZone>();

        // if(zone.GivesDamage()) {
        //     // remove life
        //     if (Runner.TryGetPlayerObject(Object.InputAuthority, out var playerNetworkObject)) {
        //         playerNetworkObject.GetComponent<PlayerDataNetworkedCollect>().TakeDamage();

        //         if (!playerNetworkObject.GetComponent<PlayerDataNetworkedCollect>().IsAlive()) {
        //             PlayerDied();
        //         }
        //     }
        // }

        return true;
    }

    private void PlayerDied() {
        gameStateController.CheckIfGameHasEnded();
        Runner.Despawn(Object);
    }
}
