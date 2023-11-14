using System;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;
// ReSharper disable InconsistentNaming

public class Movement : MonoBehaviour
{
    #region Constants

    private const float MOVE_SPEED = .105f;
    private const float JUMP_POWER = 420f;
    private const float GRAVITY_MULTI = 1.015f;
    private const float ATTACK_TOLERANCE_RANGE = 0.2f;
    private const float ATTACK_COOLDOWN_TIME = 0.22f;


    private readonly Quaternion _faceRight = Quaternion.Euler(0, -90f, 0);
    private readonly Quaternion _faceLeft = Quaternion.Euler(0, 90f, 0);

    #endregion

    #region Serializeable

    [SerializeField] private Transform _transform;
    [SerializeField] private Rigidbody _rb;
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


    private HitFreezeSystem _hitFreezeSystem;
    [SerializeField] private SoundEffects _soundEffects;
    private Vector2 _movDir = Vector2.zero;
    private List<InputAction> _toBeBlocked;
    private int _playerNumber = -1;
    private float _lastXCord;
    private PlayerStats _playerStats;
    private float _facingMultiplier;
    private bool _isBlocking = false;
    private Transform _otherPlayer;
    
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

        var facingRight = Math.Abs(_transform.rotation.y - (-90)) < 0.000001f;
        
        //TODO: Validate Move Dir
        
        if (facingRight && _movDir.x > 0 || !facingRight && _movDir.x < 0)
        {
            _animator.SetBool(FWalking, true);
            _animator.SetBool(BWalking, false);
        }
        else if (facingRight && _movDir.x < 0 || !facingRight && _movDir.x > 0)
        {
            _animator.SetBool(FWalking, false);
            _animator.SetBool(BWalking, true);
        }


        _transform.position = new(newX, ((Component)this).transform.position.y, 0); 
        UpdatePlayerRotation();

    }


    public void UpdatePlayerRotation()
    {
        
        // !Physics.Raycast(this.transform.position,new(1,0,0),10f)
        // Physics.Raycast(transform.position,new(-1,0,0),10f))

        if (Physics.Raycast(new(transform.position.x - 0.25f,transform.position.y,transform.position.z), new(-1, 0, 0), out RaycastHit hit, 10f)
            && this.transform.rotation.y > 0)
        {
            if(hit.transform.TryGetComponent<PlayerStats>(out PlayerStats tmp))
            {
                this.transform.rotation = Quaternion.Euler(0, -90f, 0);
            }
        }
        if (Physics.Raycast(new(transform.position.x + 0.25f,transform.position.y,transform.position.z), new(1, 0, 0), out RaycastHit hit2, 10f)
                 && this.transform.rotation.y < 0)
        {
            if(hit2.transform.TryGetComponent<PlayerStats>(out PlayerStats tmp))
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

    /// <summary>
    /// Used as cooldown between Attacks
    /// </summary>
    /// <returns>true if allowed, false if not</returns>
    private bool IsNextAttackAllowed()
    {
        var curTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var allowedTime = _lastAttackTime.AddSeconds(ATTACK_COOLDOWN_TIME).ToUnixTimeSeconds();
        if (curTime > allowedTime)
        {
            _lastAttackTime = DateTimeOffset.UtcNow;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Hotkey: leftclick
    /// </summary>
    /// <param name="context"></param>
    public void Punch(InputAction.CallbackContext context)
    {
        //Blocking --> Dont allow other action meanwhile.
        if (_isBlocking) return;

        if (!IsNextAttackAllowed()) return;
        GeneralFunctions.PrintDebugStatement("Punch");
        _animator.SetTrigger(PunchID);
    }

    /// <summary>
    /// Hotkey: L
    /// </summary>
    /// <param name="context"></param>
    public void Hook(InputAction.CallbackContext context)
    {
        //Blocking --> Dont allow other action meanwhile.
        if (_isBlocking) return;

        if (!IsNextAttackAllowed()) return;
        GeneralFunctions.PrintDebugStatement("Hook");
        _animator.SetTrigger(HookID);
    }

    /// <summary>
    /// Hotkey: Rightclick
    /// </summary>
    /// <param name="context"></param>
    public void SideKick(InputAction.CallbackContext context)
    {
        //Blocking --> Dont allow other action meanwhile.
        if (_isBlocking) return;

        if (!IsNextAttackAllowed()) return;
        GeneralFunctions.PrintDebugStatement("SideKick");
        _animator.SetTrigger(SideKickID);
    }

    public void Block(InputAction.CallbackContext context)
    {
        GeneralFunctions.PrintDebugStatement("BREAKPOINT");
        var state = context.phase;
        if (state == InputActionPhase.Started) UpdateBlocking(true);
        else if (state == InputActionPhase.Canceled) UpdateBlocking(false);
    }

    private void UpdateBlocking(bool state)
    {
        _animator.SetBool(BlockingID, state);
        _animator.SetBool(FWalking, false);
        _animator.SetBool(BWalking, false);

        _isBlocking = state;
    }
    
    public GameObject hitEffect;
    public GameObject blockEffect;
    public AudioSource punchSound;
    public AudioSource kickSound;
    
    public void JabActivateHitbox()
    {
        var hitTargets = Physics.OverlapSphere(_lefthandAttackPoint.position, ATTACK_TOLERANCE_RANGE);
        var attackType = EAttackType.Jab;

        foreach (var hitTarget in hitTargets)
        {
            if (hitTarget.TryGetComponent(out PlayerStats otherPlayer) && (otherPlayer.GetTeam() != _playerNumber) &&
                hitTarget.transform.gameObject.TryGetComponent(out Animator animator))
            {
                if (_jabAlreadyHit) continue;
                Instantiate(hitEffect, _lefthandAttackPoint.position, Quaternion.identity);
                punchSound.Play();
                GeneralFunctions.PrintDebugStatement("We hit the other Player!");
                
                otherPlayer.TakeDamage(30, animator, ref attackType);
                
                _jabAlreadyHit = true;
                _hitFreezeSystem.Freeze();
                break;
            }
        }
        GeneralFunctions.PlaySoundByEnum(attackType, in _soundEffects);
    }

    public void JabDeactivateHitbox()
    {
        _jabAlreadyHit = false;
    }

    public void HookActivateHitbox()
    {
        var hitTargets = Physics.OverlapSphere(_righthandAttackPoint.position, ATTACK_TOLERANCE_RANGE);
        var attackType = EAttackType.Jab;
        foreach (var hitTarget in hitTargets)
        {
            if (hitTarget.TryGetComponent(out PlayerStats otherPlayer) && (otherPlayer.GetTeam() != _playerNumber) &&
                hitTarget.transform.gameObject.TryGetComponent(out Animator animator))
            {
                if (_hookAlreadyHit) continue;
                Instantiate(hitEffect, _righthandAttackPoint.position, Quaternion.identity);
                punchSound.Play();
                GeneralFunctions.PrintDebugStatement("We hit the other Player!");
                
                otherPlayer.TakeDamage(40, animator, ref attackType);
                
                _hookAlreadyHit = true;
                _hitFreezeSystem.Freeze();
                
                break;
            }
        }
        GeneralFunctions.PlaySoundByEnum(attackType, in _soundEffects);

    }

    public void HookDeactivateHitbox()
    {
        _hookAlreadyHit = false;
    }

    public void SideKickActivateHitbox()
    {
        var hitTargets = Physics.OverlapSphere(_rightlegAttackPoint.position, ATTACK_TOLERANCE_RANGE);
        var attackType = EAttackType.Kick;
        foreach (var hitTarget in hitTargets)
        {
            if (hitTarget.TryGetComponent(out PlayerStats otherPlayer) && (otherPlayer.GetTeam() != _playerNumber) &&
                hitTarget.transform.gameObject.TryGetComponent(out Animator animator))
            {
                if (_sideKickAlreadyHit) continue;
                Instantiate(hitEffect, _rightlegAttackPoint.position, Quaternion.identity);
                kickSound.Play();
                GeneralFunctions.PrintDebugStatement("We hit the other Player!");
                
                otherPlayer.TakeDamage(80, animator, ref attackType);
                
                _sideKickAlreadyHit = true;
                _hitFreezeSystem.Freeze();
                break;
            }
        }
        GeneralFunctions.PrintDebugStatement(attackType.ToString());
        GeneralFunctions.PlaySoundByEnum(attackType, in _soundEffects);

    }

    public void SideKickDeactivateHitbox()
    {
        _sideKickAlreadyHit = false;
    }

    #endregion
}