using System;
using System.ComponentModel;
using DefaultNamespace;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[OrderBefore(typeof(NetworkTransform))]
[DisallowMultipleComponent]
// ReSharper disable once CheckNamespace
public class NetworkCharacterControllerPrototypeCustom : NetworkTransform
{
    
    #region Networked stuff

    private NetworkMecanimAnimator _networkAnimator;
    
    #region Jab
    [Networked, HideInInspector]
    public int     JabCount              { get; set; }
    
    public int     InterpolatedJabCount  => _jabCountInterpolator.Value;

    private Interpolator<int> _jabCountInterpolator;
    
    #endregion
    
    #region Kick
    [Networked, HideInInspector]
    
    public int     KickCount              { get; set; }
    
    public int     InterpolatedKickCount  => _kickCountInterpolator.Value;
    
    private Interpolator<int> _kickCountInterpolator;
    
    #endregion
    
    #region Hook
    [Networked, HideInInspector]
    public int     HookCount              { get; set; }
    
    public int     InterpolatedHookCount  => _hookCountInterpolator.Value;
    private Interpolator<int> _hookCountInterpolator;

    #endregion
    
    #region Jump
    
    [Networked, HideInInspector]
    public int     JumpCount              { get; set; }
    
    public int     InterpolatedJumpCount  => _jumpCountInterpolator.Value;
    private Interpolator<int> _jumpCountInterpolator;
    
    #endregion
    
    private InputAttackType _lastAnimationInput = 0;

    #endregion
    
    #region AnimationIDs

    private static readonly int SideKickID = Animator.StringToHash("SideKick");
    private static readonly int HookID = Animator.StringToHash("Punch");
    private static readonly int JabID = Animator.StringToHash("Jab");
    private static readonly int FWalking = Animator.StringToHash("FWalking");
    private static readonly int BWalking = Animator.StringToHash("BWalking");
    private static readonly int BlockingID = Animator.StringToHash("Blocking");
    private static readonly int JumpID = Animator.StringToHash("Jump");

    #endregion
    
    [Header("Character Controller Settings")]
    public float gravity = -20.0f;

    public float jumpImpulse = 8.0f;
    public float acceleration = 10.0f;
    public float braking = 10.0f;
    public float maxSpeed = 2.0f;
    public float rotationSpeed = 15.0f;
    public Transform leftHandAttackPoint, rightHandAttackPoint, leftLegAttackPoint, rightLegAttackPoint;

    private long _lastAttackQueued = 0;
    public bool isAllowedToAttack { get; set; } = true;
    private bool _attackAlreadyHit = false;
    
    [Networked] [HideInInspector] public bool IsGrounded { get; set; }

    [Networked] [HideInInspector] public Vector3 Velocity { get; set; }
    


    /// <summary>
    /// Sets the default teleport interpolation velocity to be the CC's current velocity.
    /// For more details on how this field is used, see <see cref="NetworkTransform.TeleportToPosition"/>.
    /// </summary>
    protected override Vector3 DefaultTeleportInterpolationVelocity => Velocity;

    /// <summary>
    /// Sets the default teleport interpolation angular velocity to be the CC's rotation speed on the Z axis.
    /// For more details on how this field is used, see <see cref="NetworkTransform.TeleportToRotation"/>.
    /// </summary>
    protected override Vector3 DefaultTeleportInterpolationAngularVelocity => new Vector3(0f, 0f, rotationSpeed);

    public CharacterController Controller { get; private set; }

    protected override void Awake()
    {
        _networkAnimator = GetComponentInChildren<NetworkMecanimAnimator>();
        base.Awake();
        CacheController();

    }

    public override void Spawned()
    {
        base.Spawned();
        CacheController();

        // Caveat: this is needed to initialize the Controller's state and avoid unwanted spikes in its perceived velocity
        Controller.Move(transform.position);

        _jabCountInterpolator = GetInterpolator<int>(nameof(JabCount));
        _hookCountInterpolator = GetInterpolator<int>(nameof(HookCount));
        _kickCountInterpolator = GetInterpolator<int>(nameof(KickCount));
        _jumpCountInterpolator = GetInterpolator<int>(nameof(JumpCount));
    }
    
    public override void FixedUpdateNetwork()
    {
        if (Object.IsProxy) // Only run on the host
        {
            Debug.Log("Im Proxy");
            return;
        }

        
        if (GetInput(out NetworkInputData networkInputData))
        {
            var moveDir = networkInputData._movementInput;
            moveDir.Normalize();


            this.Move(moveDir);
            if (networkInputData._isJumpPressed)
                this.Jump();

            StartAttackAnimation(networkInputData._InputAttackType);
        }
    }
    
    
    private void StartAttackAnimation(InputAttackType inputAttackType)
    {
        switch (inputAttackType)
        {
            case InputAttackType.None:
                Block(false);
                break;
            case InputAttackType.Block:
                Block(true);
                break;
            case InputAttackType.Jab:
                Jab();
                break;
            case InputAttackType.Sidekick:
                SideKick();
                break;
            case InputAttackType.Hook:
                Hook();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(inputAttackType), inputAttackType, null);
        }
    }

    #region Attack Patterns

    private void Jab()
    {
        JabCount++;
        _networkAnimator.SetTrigger(JabID);
    }

    private void SideKick()
    {
        KickCount++;
        _networkAnimator.SetTrigger(SideKickID);
    }

    private void Hook()
    {
        HookCount++;
        _networkAnimator.SetTrigger(HookID);
    }

    private void Block(bool val)
    {
        //TODO
    }

    #endregion

    #region Cached
    private void CacheController()
    {
        if (Controller == null)
        {
            Controller = GetComponent<CharacterController>();

            Assert.Check(Controller != null,
                $"An object with {nameof(NetworkCharacterControllerPrototype)} must also have a {nameof(CharacterController)} component.");
        }
    }

    protected override void CopyFromBufferToEngine()
    {
        // Trick: CC must be disabled before resetting the transform state
        Controller.enabled = false;

        // Pull base (NetworkTransform) state from networked data buffer
        base.CopyFromBufferToEngine();

        // Re-enable CC
        Controller.enabled = true;
    }

    #endregion
    
    #region Movement
    
    /// <summary>
    /// Basic implementation of a jump impulse (immediately integrates a vertical component to Velocity).
    /// <param name="ignoreGrounded">Jump even if not in a grounded state.</param>
    /// <param name="overrideImpulse">Optional field to override the jump impulse. If null, <see cref="jumpImpulse"/> is used.</param>
    /// </summary>
    public virtual void Jump(bool ignoreGrounded = false, float? overrideImpulse = null)
    {
        // JUMP
        Debug.Log("JUMPING");
        
        if (IsGrounded || ignoreGrounded)
        {
            var newVel = Velocity;
            newVel.y += overrideImpulse ?? jumpImpulse;
            Velocity = newVel;
        }
    }

    /// <summary>
    /// Basic implementation of a character controller's movement function based on an intended direction.
    /// <param name="direction">Intended movement direction, subject to movement query, acceleration and max speed values.</param>
    /// </summary>
    public virtual void Move(Vector3 direction)
    {
        var deltaTime = Runner.DeltaTime;
        var previousPos = transform.position;
        var moveVelocity = Velocity;

        direction = direction.normalized;

        
        if (IsGrounded && moveVelocity.y < 0)
        {
            moveVelocity.y = 0f;
        }

        direction.x *= -1;
        
        moveVelocity.y += gravity * Runner.DeltaTime;

        var horizontalVel = default(Vector3);
        horizontalVel.x = moveVelocity.x;
        horizontalVel.z = moveVelocity.z;

        if (direction == default)
        {
            horizontalVel = Vector3.Lerp(horizontalVel, default, braking * deltaTime);
        }
        else
        {
            horizontalVel = Vector3.ClampMagnitude(horizontalVel + direction * acceleration * deltaTime, maxSpeed);
        }

        moveVelocity.x = horizontalVel.x;
        moveVelocity.z = horizontalVel.z;

        Controller.Move(moveVelocity * deltaTime);

        Velocity = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;
        IsGrounded = Controller.isGrounded;
    }

    #endregion
    
    public void Rotate(float rotationY)
    {
        transform.Rotate(0, rotationY * Runner.DeltaTime * rotationSpeed, 0);
    }
    
    
    
    #region Animations

    #region Start
    
    public void AnimationStarted(EAttackType attackType)
    {
        isAllowedToAttack = false;
        //TODO: PLAY SOUND
    }

    
    #endregion
    
    #region ActivateHitBox
    
    public void ActivateHitBox(InputAttackType inputAttackType)
    {
        #region Determine Physical HitArea and damage
        
        Vector3 hitPoint = Vector3.zero;
        short damage = 0;
        switch (inputAttackType)
        {
            case InputAttackType.None:
                break;
            case InputAttackType.Block:
                break;
            case InputAttackType.Jab:
                hitPoint = leftHandAttackPoint.position;
                damage = 100;
                break;
            case InputAttackType.Sidekick:
                damage = 200;
                break;
            case InputAttackType.Hook:
                damage = 100;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(inputAttackType), inputAttackType, null);
        }
        
        #endregion
        
        var hitTargets = Physics.OverlapSphere(hitPoint, .2f);

        for (var index = 0; index < hitTargets.Length && !_attackAlreadyHit; index++)
        {
            var hitTarget = hitTargets[index];
            if (hitTarget.TryGetComponent(out HPHandler hpHandler) && _attackAlreadyHit)
            {
                _attackAlreadyHit = true;
                
                //Instantiate(hitEffect, _lefthandAttackPoint.position, Quaternion.identity);

                hpHandler.OnHitTaken(damage);

                //_fusionConnection.PlaySound(attackType, ref _soundEffects);
                //_hitFreezeSystem.Freeze();
                
                break;
            }
        }
    }
    
    #endregion

    #region Deactivate HitBox

    
    public void DeactivateHitBox() => _attackAlreadyHit = false;

    #endregion

    #endregion
}