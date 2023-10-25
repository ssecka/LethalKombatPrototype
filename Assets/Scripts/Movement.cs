using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Movement : MonoBehaviour
{
    #region Constants
    
    private const float MOVE_SPEED = .105f;
    private const float JUMP_POWER = 250f;
    private const float GRAVITY_MULTI = 1.015f;
    private const float ATTACK_TOLERANCE_RANGE = 0.2f;
    private const float ATTACK_COOLDOWN_TIME = 0.22f;

    #endregion
    
    #region Serializeable
    
    [SerializeField] private Transform _transform;
    [SerializeField] private Rigidbody _rb;
    public Transform _sideKickAttackPoint, _punchAttackPoint;

    #endregion
    
    private Animator _animator;
    
    private DateTimeOffset _lastAttackTime = DateTimeOffset.Now;
    private bool _canJump = true;

    #region AnimationIDs
    
    private static readonly int SideKickID = Animator.StringToHash("SideKick");
    private static readonly int PunchID = Animator.StringToHash("Punch");
    private static readonly int Jab = Animator.StringToHash("Jab");
    private static readonly int FWalking = Animator.StringToHash("FWalking");
    private static readonly int BWalking = Animator.StringToHash("BWalking");
    private static readonly int BlockingID = Animator.StringToHash("Blocking");
    private static readonly int JumpID = Animator.StringToHash("Jump");
    
    #endregion
    
    private Vector2 _movDir = Vector2.zero;
    private List<InputAction> _toBeBlocked;
    private int _playerNumber = -1;
    private bool _punchSwitcher = true;
    private float _lastXCord;
    private PlayerStats _playerStats;
    private float _facingMultiplier;
    private bool _isBlocking = false;
    

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
        _toBeBlocked ??= new();
        if (_transform == null) Debug.Log("transform not set!");
        
        if(TryGetComponent(out PlayerStats stats))
        {
            _playerNumber = stats._player;
            _playerStats = stats; // We need this to set blockingg
        }

        _facingMultiplier = _transform.rotation.y < 0 ? -1 : 0;
        
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
        if (_sideKickAttackPoint != null)
            Gizmos.DrawWireSphere(_sideKickAttackPoint.position,ATTACK_TOLERANCE_RANGE);
        if(_punchAttackPoint != null)
            Gizmos.DrawWireSphere(_punchAttackPoint.position,ATTACK_TOLERANCE_RANGE);
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
            _animator.SetBool(FWalking,false);
            _animator.SetBool(BWalking,false);
            return;
        }

        var hlp = _lastXCord - _transform.position.x;
        if (Math.Abs(hlp) < Math.Pow(10,-6))
        {
            _animator.SetBool(FWalking,false);
            _animator.SetBool(BWalking,false);
        }
        _lastXCord = _transform.position.x;
        
        var newX = _transform.position.x + ((_facingMultiplier * _movDir.x) * MOVE_SPEED);
        if (_movDir.x > 0)
        {
            _animator.SetBool(FWalking,true);
            _animator.SetBool(BWalking,false);
        }
        else if (_movDir.x < 0)
        {
            _animator.SetBool(FWalking,false);
            _animator.SetBool(BWalking,true);
        }
        

        _transform.position = new(newX, ((Component)this).transform.position.y, 0);
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

        _punchSwitcher = !_punchSwitcher;
        if (_punchSwitcher)
        {
            _animator.SetTrigger(Jab);
        }
        else
        {
            _animator.SetTrigger(PunchID);
        }

        // ReSharper disable once Unity.PreferNonAllocApi --> not needed in this usecase
        var hitTargets = Physics.OverlapSphere(_sideKickAttackPoint.position, ATTACK_TOLERANCE_RANGE);

        foreach (var hitTarget in hitTargets)
        {
            if (hitTarget.TryGetComponent(out PlayerStats otherPlayer) && (otherPlayer._player != _playerNumber) && 
                hitTarget.transform.gameObject.TryGetComponent(out Animator animator))
            {
                GeneralFunctions.PrintDebugStatement("We hit the other Player!");
                otherPlayer.TakeDamage(100,animator);
                break;
            }
            
            //GeneralFunctions.PrintDebugStatement("Target_Hit: " + hitTarget);
        }
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

        //TODO: Add animation for front punch
        _animator.SetTrigger(SideKickID);

        // ReSharper disable once Unity.PreferNonAllocApi --> not needed in this usecase
        var hitTargets = Physics.OverlapSphere(_sideKickAttackPoint.position, ATTACK_TOLERANCE_RANGE);

        foreach (var hitTarget in hitTargets)
        {
            if (hitTarget.TryGetComponent(out PlayerStats otherPlayer) && (otherPlayer._player != _playerNumber) && 
                hitTarget.transform.gameObject.TryGetComponent(out Animator animator))
            {
                GeneralFunctions.PrintDebugStatement("We hit the other Player!");
                otherPlayer.TakeDamage(100,animator);
                break;
            }
            
            //GeneralFunctions.PrintDebugStatement("Target_Hit: " + hitTarget);
        }
    }

    public void Block(InputAction.CallbackContext context)
    {
        GeneralFunctions.PrintDebugStatement("BREAKPOINT");
        var state = context.phase;
        if(state == InputActionPhase.Started) UpdateBlocking(true);
        else if(state == InputActionPhase.Canceled) UpdateBlocking(false);
    }

    private void UpdateBlocking(bool state)
    {
        _animator.SetBool(BlockingID,state);
        _animator.SetBool(FWalking,false);
        _animator.SetBool(BWalking,false);
        
        _isBlocking = state;
    }
    
    #endregion
}
