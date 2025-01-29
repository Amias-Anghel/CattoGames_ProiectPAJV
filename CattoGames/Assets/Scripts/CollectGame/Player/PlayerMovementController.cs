using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField] private float _movementSpeed = 20.0f;

    // Local Runtime references
    private Rigidbody2D _rigidbody = null; 
    private PlayerCollectController _playerpController = null;
    private GameObject playerVisuals = null;
    private Animator playerAnimator = null;
    private float visualsScale_x;

    private ChangeDetector _changeDetector;

    // networked
    [Networked] private bool facingLeft {get;set;}
    [Networked] private bool moving {get;set;}

    [Networked] private float _screenBoundaryX { get; set; }
    [Networked] private float _screenBoundaryY { get; set; }

    public override void Spawned()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerpController = GetComponent<PlayerCollectController>();
        playerVisuals = transform.GetChild(0).gameObject;
        playerAnimator = playerVisuals.transform.GetChild(0).GetComponent<Animator>();
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        visualsScale_x = playerVisuals.transform.localScale.x;

        if (Object.HasStateAuthority) {
            facingLeft = false;
            _screenBoundaryX = Camera.main.orthographicSize * Camera.main.aspect;
            _screenBoundaryY = Camera.main.orthographicSize;
        }
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(facingLeft):
                    ApplyFlip();
                    break;
                case nameof(moving):
                    ApplyAnimation();
                    break;
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!_playerpController.AcceptInput) return;

        if (GetInput(out PlayerInput input))
        {
            Move(input);
        }

        CheckExitBounds();
    }

    private void Move(PlayerInput input)
    {
        _rigidbody.MovePosition(_rigidbody.position + _movementSpeed * input.movement * Runner.DeltaTime);
        if (input.movement != Vector2.zero) {
            facingLeft = input.movement.x <= 0;
        }

        moving = input.movement != Vector2.zero;
    }


    private void ApplyFlip() {
        Debug.Log(facingLeft);
        Vector3 currentScale = playerVisuals.transform.localScale;
        currentScale.x = facingLeft ? -visualsScale_x : visualsScale_x;
        playerVisuals.transform.localScale = currentScale;
    }

    private void ApplyAnimation() {
        playerAnimator.SetBool("move", moving);
    }

    private void CheckExitBounds()
    {
        var position = _rigidbody.position;

        if (Mathf.Abs(position.x) < _screenBoundaryX && Mathf.Abs(position.y) < _screenBoundaryY) return;

        if (Mathf.Abs(position.x) > _screenBoundaryX)
        {
            position = new Vector3(-Mathf.Sign(position.x) * _screenBoundaryX, position.y, 0);
        }

        if (Mathf.Abs(position.y) > _screenBoundaryY)
        {
            position = new Vector3(position.x, -Mathf.Sign(position.y) * _screenBoundaryY, 0);
        }

        position -= position.normalized * 0.1f;
        GetComponent<NetworkRigidbody2D>().Teleport(position);
    }
}
