using System;
using System.Collections.Generic;
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

    [Networked]
    public long LastHitTimestamp { get; set; }  
    
    #region Jab

    [Networked, HideInInspector] public int JabCount { get; set; }

    public int InterpolatedJabCount => _jabCountInterpolator.Value;

    private Interpolator<int> _jabCountInterpolator;

    #endregion

    #region Kick

    [Networked, HideInInspector] public int KickCount { get; set; }

    public int InterpolatedKickCount => _kickCountInterpolator.Value;

    private Interpolator<int> _kickCountInterpolator;

    #endregion

    #region Hook

    [Networked, HideInInspector] public int HookCount { get; set; }

    public int InterpolatedHookCount => _hookCountInterpolator.Value;
    private Interpolator<int> _hookCountInterpolator;

    #endregion

    #region LowKick

    [Networked, HideInInspector] public int LowKickCount { get; set; }

    public int InterpolatedLowKickCount => _lowKickCountInterpolator.Value;

    private Interpolator<int> _lowKickCountInterpolator;

    #endregion

    #region FireBall

    [Networked, HideInInspector] public int FireBallCount { get; set; }

    public int InterpolatedFireBallCount => _fireBallCountInterpolator.Value;

    private Interpolator<int> _fireBallCountInterpolator;

    #endregion

    #region Jump

    [Networked, HideInInspector] public int JumpCount { get; set; }

    public int InterpolatedJumpCount => _jumpCountInterpolator.Value;
    private Interpolator<int> _jumpCountInterpolator;

    #endregion

    #region Block
    
    /*
     * Fusion cant handle bool States so we need to use int  (1 is true and 0 is false)
     * :)))))))))))))
     */
    
    
    [Networked, HideInInspector] public int BlockState { get; set; } 

    public int InterpolatedBlockState => _blockStateInterpolator.Value;
    private Interpolator<int> _blockStateInterpolator;

    #endregion

    #region Speed

    [Networked, HideInInspector] public float SpeedValue { get; set; }

    public float InterpolatedSpeed => _speedInterpolator.Value;
    private Interpolator<float> _speedInterpolator;

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
    private static readonly int LowKickID = Animator.StringToHash("LowKic");
    private static readonly int FireBallID = Animator.StringToHash("FireBall");
    private static readonly int JumpID = Animator.StringToHash("Jump");

    #endregion

    #region Character Specifications

    [Header("Character Controller Settings")]
    public float gravity = -20.0f;

    public float jumpImpulse = 8.0f;
    public float acceleration = 10.0f;
    public float braking = 10.0f;
    public float maxSpeed = 2.0f;
    public float rotationSpeed = 15.0f;

    #endregion

    public Transform leftHandAttackPoint,
        rightHandAttackPoint,
        leftLegAttackPoint,
        rightLegAttackPoint,
        fireBallAttackPoint;

    private long _lastAttackQueued = 0;
    public bool _canQueueAttack { get; set; } = true;
    private bool _attackAlreadyHit = false;
    private long _lastTimeCheck = 0;
    private InputAttackType _lastNetworkInput;

    private int _disableBlockCounter = 0;

    private List<LagCompensatedHit> _lagCompensatedHits = new List<LagCompensatedHit>();
    private PlayerRef _thrownByPlayerRef;
    Vector3 _currentActiveHitPoint = Vector3.zero;

    private const long IMMUNITY_DURATION = 50 * TimeSpan.TicksPerMillisecond;
    
    private bool _checkForHits;


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

        SpeedValue = 0;
        LastHitTimestamp = 0;
        
        _jabCountInterpolator = GetInterpolator<int>(nameof(JabCount));
        _hookCountInterpolator = GetInterpolator<int>(nameof(HookCount));
        _kickCountInterpolator = GetInterpolator<int>(nameof(KickCount));
        _jumpCountInterpolator = GetInterpolator<int>(nameof(JumpCount));
        _blockStateInterpolator = GetInterpolator<int>(nameof(BlockState));
        _lowKickCountInterpolator = GetInterpolator<int>(nameof(LowKickCount));
        _fireBallCountInterpolator = GetInterpolator<int>(nameof(FireBallCount));
        _speedInterpolator = GetInterpolator<float>(nameof(SpeedValue));
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.IsProxy) // Only run on the host
        {
            return;
        }


        if (GetInput(out NetworkInputData networkInputData))
        {
            var moveDir = networkInputData._movementInput;
            moveDir.Normalize();


            this.Move(moveDir);
            if (networkInputData._isJumpPressed)
            {
                this.Jump();
                networkInputData._isJumpPressed = false;
            }

            // Cache last input 
            if (networkInputData._InputAttackType != 0)
            {
                Debug.Log($"Changed last input from {_lastNetworkInput} to {networkInputData._InputAttackType}");
                _lastNetworkInput = networkInputData._InputAttackType;
            }

            QueueAttackAnimation(_lastNetworkInput);
        }

        if (!Object.HasStateAuthority) return;

        if (_checkForHits)
        {
            var hitCount = Runner.LagCompensation.OverlapSphere(
                _currentActiveHitPoint,
                0.25f,
                _thrownByPlayerRef,
                _lagCompensatedHits, options: HitOptions.IncludePhysX);


            print("Hits LagCompenstation: " + hitCount);
            for (int i = 0; i < hitCount; i++)
            {
                HPHandler hpHandler = _lagCompensatedHits[i].GameObject.transform.root
                    .GetComponentInChildren<HPHandler>();
                if (hpHandler == null) continue;

                var currentTime = DateTime.UtcNow.Ticks;
                if (LastHitTimestamp + IMMUNITY_DURATION > currentTime)
                {
                    // print(LastHitTimestamp);
                    // print(IMMUNITY_DURATION);
                    // print(currentTime);
                    break;
                }

                LastHitTimestamp = currentTime;
                
                hpHandler.OnHitTaken(250);
                _checkForHits = false;
                break;
            }
        }
    }


    private void QueueAttackAnimation(InputAttackType inputAttackType)
    {
        _lastNetworkInput = 0;
        //Test with this

        if (inputAttackType is InputAttackType.None)
        {
            _disableBlockCounter++;
            if (_disableBlockCounter > 15)
            {
                BlockState = 0;
            }

            return;
        }

        ;

        _disableBlockCounter = 0;
        if (inputAttackType is InputAttackType.Block)
        {
            BlockState = 1;
            Debug.Log($"BlockState changed to true.");
            return;
        }


        if (!_canQueueAttack) return;

        switch (inputAttackType)
        {
            case InputAttackType.Jab:
                JabCount++;
                Debug.Log($"Increased Jab from {JabCount - 1} to {JabCount}");
                break;
            case InputAttackType.Sidekick:
                KickCount++;
                Debug.Log($"Increased Kick from {KickCount - 1} to {KickCount}");
                break;
            case InputAttackType.Hook:
                HookCount++;
                Debug.Log($"Increased Hook from {HookCount - 1} to {HookCount}");
                break;
            case InputAttackType.Lowkick:
                LowKickCount++;
                Debug.Log($"Increased LowKick from {LowKickCount - 1} to {LowKickCount}");
                break;
            case InputAttackType.FireBall:
                FireBallCount++;
                Debug.Log($"Increased FireBall from {FireBallCount - 1} to {FireBallCount}");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(inputAttackType), inputAttackType, null);
        }

        BlockState = 0;
        Debug.Log($"Blocking State changed to false");
    }

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
        if (IsGrounded || ignoreGrounded)
        {
            Debug.Log("JUMPING");
            var newVel = Velocity;
            newVel.y += overrideImpulse ?? jumpImpulse;
            Velocity = newVel;
            JumpCount++;
            // _networkAnimator.SetTrigger(JumpID, true);
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

        SpeedValue = moveVelocity.x * (transform.eulerAngles.y >= 269 ? -1 : 1);


        Velocity = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;
        IsGrounded = Controller.isGrounded;
    }

    #endregion

    public void Rotate(float rotationY)
    {
        transform.Rotate(0, rotationY * Runner.DeltaTime * rotationSpeed, 0);
    }


    #region Animations

    #region Start and end

    public void AnimationStarted(InputAttackType attackType)
    {
        _canQueueAttack = false;
    }

    public void DeactivateHitBox()
    {
        _attackAlreadyHit = false;
        _checkForHits = false;
    }

    #endregion


    #region ActivateHitBox

    public void ActivateHitBox(InputAttackType inputAttackType)
    {
        #region Determine Physical HitArea and damage

        short damage = 0;
        switch (inputAttackType)
        {
            case InputAttackType.None:
                break;
            case InputAttackType.Block:
                break;
            case InputAttackType.Jab:
                _currentActiveHitPoint = leftHandAttackPoint.position;
                damage = 100;
                break;
            case InputAttackType.Sidekick:
                _currentActiveHitPoint = rightLegAttackPoint.position;
                damage = 200;
                break;
            case InputAttackType.Hook:
                _currentActiveHitPoint = rightHandAttackPoint.position;
                damage = 100;
                break;
            case InputAttackType.Lowkick:
                _currentActiveHitPoint = leftLegAttackPoint.position;
                damage = 100;
                break;
            case InputAttackType.FireBall:
                _currentActiveHitPoint = fireBallAttackPoint.position;
                damage = 100;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(inputAttackType), inputAttackType, null);
        }

        _checkForHits = true;

        #endregion
    }

    #endregion

    #endregion

    public void KnockOut()
    {
        // TODO: Despawn this entitiy and send restart request to host
    }
}