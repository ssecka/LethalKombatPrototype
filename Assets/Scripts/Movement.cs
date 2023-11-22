using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;

// ReSharper disable InconsistentNaming

public class Movement : MonoBehaviour
{
    #region Constants

    private const float MOVE_SPEED = .105f;
    private const float JUMP_POWER = 600f;
    private const float GRAVITY_MULTI = 1.015f;
    private const float ATTACK_TOLERANCE_RANGE = 0.2f;
    private const float RIGHT_QUAT = -0.7075f;
    private const float ATTACK_COOLDOWN_TIME = 0.075f;


    private readonly Quaternion _faceRight = Quaternion.Euler(0, -90f, 0);
    private readonly Quaternion _faceLeft = Quaternion.Euler(0, 90f, 0);
    public bool facingRight;

    #endregion

    #region Serializeable

    [SerializeField] private Transform _transform;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private SoundEffects _soundEffects;
    public Transform _lefthandAttackPoint, _righthandAttackPoint, _leftlegAttackPoint, _rightlegAttackPoint;

    #endregion

    private Animator _animator;

    private DateTimeOffset _lastAttackTime = DateTimeOffset.Now;
    private bool _canJump = true;

    #region AnimationIDs

    private static readonly int SideKickID = Animator.StringToHash("SideKick");
    private static readonly int HookID = Animator.StringToHash("Punch");
    private static readonly int PunchID = Animator.StringToHash("Jab");
    private static readonly int FWalking = Animator.StringToHash("FWalking");
    private static readonly int BWalking = Animator.StringToHash("BWalking");
    private static readonly int BlockingID = Animator.StringToHash("Blocking");
    private static readonly int JumpID = Animator.StringToHash("Jump");

    #endregion

    public GameObject hitEffect;
    private HitFreezeSystem _hitFreezeSystem;
    private Vector2 _movDir = Vector2.zero;
    private List<InputAction> _toBeBlocked;
    private int _playerNumber = -1;
    private float _lastXCord;
    private PlayerStats _playerStats;
    private float _facingMultiplier;
    public bool _isBlocking = false;
    public static FusionConnection _fusionConnection;
    private bool _attackAllowed = true;

    private bool _jabAlreadyHit, _hookAlreadyHit, _sideKickAlreadyHit;


    #region Startup

    #region On/Off

    private void OnEnable()
    {
        _toBeBlocked ??= new();
    }

    private void OnDisable()
    {
    }

    #endregion

    private void Awake()
    {

        
        
        _hitFreezeSystem ??= GameObject.Find("PlayerEnvironmentSystem").GetComponent<HitFreezeSystem>();
        _soundEffects = GetComponent<SoundEffects>();
        _toBeBlocked ??= new();
        if (_transform == null) Debug.Log("transform not set!");

        if (TryGetComponent(out PlayerStats stats))
        {
            _playerNumber = stats.GetTeam();
            _playerStats = stats; // We need this to set blockingg
        }
        

        _lastXCord = _transform.position.x;
    }


    // Start is called before the first frame update
    private void Start()
    {
        // Get the Animator component from your character.
        _animator = GetComponent<Animator>();
    }

    #endregion

    /// <summary>
    /// Used to draw the hit detection spheres
    /// </summary>
    private void OnDrawGizmos()
    {
        if (_lefthandAttackPoint != null)
            Gizmos.DrawWireSphere(_lefthandAttackPoint.position, ATTACK_TOLERANCE_RANGE);
        if (_righthandAttackPoint != null)
            Gizmos.DrawWireSphere(_righthandAttackPoint.position, ATTACK_TOLERANCE_RANGE);
        if (_leftlegAttackPoint != null)
            Gizmos.DrawWireCube(_leftlegAttackPoint.position, new Vector3(.23f, .5f, .23f));
        if (_rightlegAttackPoint != null)
            Gizmos.DrawWireCube(_rightlegAttackPoint.position, new Vector3(.23f, .5f, .23f));
    }

    /// <summary>
    /// Used for the hit detection
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ground _))
        {
            _canJump = true;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (_rb.velocity.y < 0 && !_canJump)
        {
            var velocity = _rb.velocity;
            velocity = new(velocity.x, velocity.y * GRAVITY_MULTI, velocity.z);
            _rb.velocity = velocity;
        }
    }

    private void FixedUpdate()
    {
        if (_isBlocking)
        {
            _animator.SetBool(FWalking, false);
            _animator.SetBool(BWalking, false);
            return;
        }

        var hlp = _lastXCord - _transform.position.x;
        if (Math.Abs(hlp) < Math.Pow(10, -6))
        {
            _animator.SetBool(FWalking, false);
            _animator.SetBool(BWalking, false);
        }

        _lastXCord = _transform.position.x;
        var newX = _transform.position.x + ((-1 * _movDir.x) * MOVE_SPEED);


        facingRight = Math.Abs(_transform.localEulerAngles.y - 270) < 0.00001f;

        switch (facingRight)
        {
            case true when _movDir.x > 0:
            case false when _movDir.x < 0:
                _animator.SetBool(FWalking, true);
                _animator.SetBool(BWalking, false);
                break;
            case true when _movDir.x < 0:
            case false when _movDir.x > 0:
                _animator.SetBool(FWalking, false);
                _animator.SetBool(BWalking, true);
                break;
        }


        _transform.position = new(newX, ((Component)this).transform.position.y, 0);
        UpdatePlayerRotation();
    }


    private void UpdatePlayerRotation()
    {
        // !Physics.Raycast(this.transform.position,new(1,0,0),10f)
        // Physics.Raycast(transform.position,new(-1,0,0),10f))

        if (Physics.Raycast(new(transform.position.x - 0.25f, transform.position.y, transform.position.z),
                new(-1, 0, 0), out RaycastHit hit, 10f)
            && this.transform.rotation.y > 0)
        {
            if (hit.transform.TryGetComponent<PlayerStats>(out PlayerStats tmp))
            {
                this.transform.rotation = Quaternion.Euler(0, -90f, 0);
            }
        }

        if (Physics.Raycast(new(transform.position.x + 0.25f, transform.position.y, transform.position.z), new(1, 0, 0),
                out RaycastHit hit2, 10f)
            && this.transform.rotation.y < 0)
        {
            if (hit2.transform.TryGetComponent<PlayerStats>(out PlayerStats tmp))
            {
                this.transform.rotation = Quaternion.Euler(0, 90f, 0);
            }
        }
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        _movDir = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // Dont allow this action while blocking
        if (_isBlocking) return;
        if (!_canJump) return;

        _animator.SetTrigger(JumpID);
        _rb.AddForce(Vector3.up * JUMP_POWER);
        _canJump = false;
    }


    #region Attacks

    #region Cooldown-System

    /// <summary>
    /// Used as cooldown between Attacks
    /// </summary>
    /// <returns>true if allowed, false if not</returns>
    private bool IsNextAttackAllowed() => _attackAllowed;

    public void AttackFinished()
    {
        var task = new Task(() =>
        {
            // delay for cooldown time, then enable.
            // 1E3 = 1.000
            Task.Delay((int)(ATTACK_COOLDOWN_TIME * 1E3));
            _attackAllowed = true;
        });
        task.Start();
    }

    #endregion

    #region Jab (aka Punch)

    /// <summary>
    /// Hotkey: leftclick
    /// </summary>
    /// <param name="context"></param>
    public void Jab(InputAction.CallbackContext context)
    {
        //Blocking --> Dont allow other action meanwhile.
        if (_isBlocking) return;

        if (!IsNextAttackAllowed()) return;
        _attackAllowed = false;

        GeneralFunctions.PrintDebugStatement("Jab");
        _animator.SetTrigger(PunchID);
    }

    public void JabAnimationStarted() => _fusionConnection.PlaySound(EAttackType.Jab, ref _soundEffects);

    public void JabActivateHitbox()
    {
        var hitTargets = Physics.OverlapSphere(_lefthandAttackPoint.position, ATTACK_TOLERANCE_RANGE);
        var attackType = EAttackType.JabHit;

        GeneralFunctions.PrintDebugStatement("Jab");

        for (var index = 0; index < hitTargets.Length && !_jabAlreadyHit; index++)
        {
            var hitTarget = hitTargets[index];
            if (hitTarget.TryGetComponent(out PlayerStats otherPlayer) && (otherPlayer.GetTeam() != _playerNumber) &&
                hitTarget.transform.gameObject.TryGetComponent(out Animator animator) && !_jabAlreadyHit)
            {
                _jabAlreadyHit = true;
                Instantiate(hitEffect, _lefthandAttackPoint.position, Quaternion.identity);

                otherPlayer.TakeDamage(30, animator, ref attackType, 2f);

                GeneralFunctions.PrintDebugStatement("We hit the other Player!");

                _fusionConnection.PlaySound(attackType, ref _soundEffects);

                _hitFreezeSystem.Freeze();
                break;
            }
        }
    }

    public void JabDeactivateHitbox()
    {
        _jabAlreadyHit = false;
    }

    #endregion

    #region Hook

    /// <summary>
    /// Hotkey: L
    /// </summary>
    /// <param name="context"></param>
    public void Hook(InputAction.CallbackContext context)
    {
        //Blocking --> Dont allow other action meanwhile.
        if (_isBlocking) return;

        if (!IsNextAttackAllowed()) return;
        _attackAllowed = false;
        
        
        GeneralFunctions.PrintDebugStatement("Hook");
        _animator.SetTrigger(HookID);
    }

    public void HookAnimationStarted() => _fusionConnection.PlaySound(EAttackType.Jab, ref _soundEffects);


    public void HookActivateHitbox()
    {
        var hitTargets = Physics.OverlapSphere(_righthandAttackPoint.position, ATTACK_TOLERANCE_RANGE);
        var attackType = EAttackType.JabHit;

        for (var index = 0; index < hitTargets.Length && !_hookAlreadyHit; index++)
        {
            var hitTarget = hitTargets[index];
            if (hitTarget.TryGetComponent(out PlayerStats otherPlayer) && (otherPlayer.GetTeam() != _playerNumber) &&
                hitTarget.transform.gameObject.TryGetComponent(out Animator animator))
            {
                _hookAlreadyHit = true;
                Instantiate(hitEffect, _righthandAttackPoint.position, Quaternion.identity);
                GeneralFunctions.PrintDebugStatement("We hit the other Player!");

                otherPlayer.TakeDamage(40, animator, ref attackType, 2f);

                _fusionConnection.PlaySound(attackType, ref _soundEffects);

                _hitFreezeSystem.Freeze();

                break;
            }
        }
    }

    public void HookDeactivateHitbox()
    {
        _hookAlreadyHit = false;
    }

    #endregion

    #region SideKick

    /// <summary>
    /// Hotkey: Rightclick
    /// </summary>
    /// <param name="context"></param>
    public void SideKick(InputAction.CallbackContext context)
    {
        //Blocking --> Dont allow other action meanwhile.
        if (_isBlocking) return;

        if (!IsNextAttackAllowed()) return;
        _attackAllowed = false;
        
        GeneralFunctions.PrintDebugStatement("SideKick");
        _animator.SetTrigger(SideKickID);
    }

    public void SideKickAnimationStarted()
    {
        _fusionConnection.PlaySound(EAttackType.Kick, ref _soundEffects);
    }

    public void SideKickActivateHitbox()
    {
        var hitTargets = Physics.OverlapSphere(_rightlegAttackPoint.position, ATTACK_TOLERANCE_RANGE);
        var attackType = EAttackType.KickHit;

        for (var index = 0; index < hitTargets.Length && !_sideKickAlreadyHit; index++)
        {
            var hitTarget = hitTargets[index];
            if (hitTarget.TryGetComponent(out PlayerStats otherPlayer) && (otherPlayer.GetTeam() != _playerNumber) &&
                hitTarget.transform.gameObject.TryGetComponent(out Animator animator))
            {
                _sideKickAlreadyHit = true;
                Instantiate(hitEffect, _rightlegAttackPoint.position, Quaternion.identity);
                GeneralFunctions.PrintDebugStatement("We hit the other Player!");

                otherPlayer.TakeDamage(80, animator, ref attackType, 4f);

                _fusionConnection.PlaySound(attackType, ref _soundEffects);

                _hitFreezeSystem.Freeze();
                break;
            }
        }
    }

    public void SideKickDeactivateHitbox()
    {
        _sideKickAlreadyHit = false;
    }

    #endregion

    #region Blocking

    public void Block(InputAction.CallbackContext context)
    {
        var state = context.phase;
        if (state == InputActionPhase.Started) UpdateBlocking(true);
        else if (state == InputActionPhase.Canceled) UpdateBlocking(false);
    }

    private void UpdateBlocking(bool state)
    {
        _animator.SetBool(BlockingID, state);
        _animator.SetBool(FWalking, false);
        _animator.SetBool(BWalking, false);

        //Settings the state in this class AND for the corresponding playerStats class.
        _isBlocking = state;
        _playerStats.IsBlocking = state;
    }

    #endregion

    #endregion
}