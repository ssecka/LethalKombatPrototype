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
    private const float MOVE_SPEED = .105f;
    private const float JUMP_POWER = 250f;
    private const float GRAVITY_MULTI = 1.015f;
    private const float ATTACK_TOLERANCE_RANGE = 0.2f;
    
    
    private Animator _animator;

    private bool _canJump = true;

    [SerializeField] private Transform _transform;
    [SerializeField] private Rigidbody _rb;
    
    // Keep Public so the Sphere can be seen in the editor
    public Transform _sideKickAttackPoint, _punchAttackPoint;

    private Vector2 _movDir = Vector2.zero;

    private PlayerInput _playerControlls;

    private InputAction _move, _punch, _jump, _sideKick, _block;

    private List<InputAction> _toBeBlocked;
    
    private static readonly int SideKickID = Animator.StringToHash("SideKick");
    private int _playerNumber = -1;
    private bool _punchSwitcher = true;
    private static readonly int PunchID = Animator.StringToHash("Punch");
    private static readonly int Jab = Animator.StringToHash("Jab");
    private float _lastXCord;
    private PlayerStats _playerStats;

    private bool _isMoving = false;
    private static readonly int FWalking = Animator.StringToHash("FWalking");
    private static readonly int BWalking = Animator.StringToHash("BWalking");
    private float _facingMultiplier = 0f;
    private static readonly int BlockingID = Animator.StringToHash("Blocking");

    private void Awake()
    {
        _toBeBlocked ??= new();
        if (_transform == null) Debug.Log("transform not set!");
        _playerControlls = new();
        
        if(TryGetComponent(out PlayerStats stats))
        {
            _playerNumber = stats._player;
            _playerStats = stats; // We need this to set blockingg
        }

        _facingMultiplier = _transform.rotation.y < 0 ? -1 : 0;
        
        _lastXCord = _transform.position.x;
    }


    private void OnEnable()
    {
        _toBeBlocked ??= new();

        _move = _playerControlls.Player.Move;
        _move.Enable();
        _toBeBlocked.Add(_move);
        
        
        _punch = _playerControlls.Player.Punch;
        _punch.Enable();
        _punch.performed += Punch;
        _toBeBlocked.Add(_punch);

        _sideKick = _playerControlls.Player.LegSweep;
        _sideKick.Enable();
        _sideKick.performed += SideKick;
        _toBeBlocked.Add(_sideKick);
        
        
        _jump = _playerControlls.Player.Jump;
        _jump.Enable();
        _jump.performed += Jump;
        _toBeBlocked.Add(_jump);
        
        
        _block = _playerControlls.Player.Block;
        _block.Enable();
        _block.started += _ => UpdateBlocking(true);
        _block.canceled += _ => UpdateBlocking(false);

        
    }



    private void OnDisable()
    {
        _move.Disable();
        _punch.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get the Animator component from your character.
        _animator = GetComponent<Animator>();
    }


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
        _movDir = _move.ReadValue<Vector2>();
        if (_rb.velocity.y < 0 && !_canJump)
        {
            var velocity = _rb.velocity;
            velocity = new(velocity.x, velocity.y * GRAVITY_MULTI, velocity.z);
            _rb.velocity = velocity;
        }


    }

    private void FixedUpdate()
    {
        
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

    private void Jump(InputAction.CallbackContext context)
    {
        if (!_canJump) return;
        _rb.AddForce(Vector3.up * JUMP_POWER);
        _canJump = false;
    }

    private void OnDrawGizmos()
    {
        if (_sideKickAttackPoint != null)
            Gizmos.DrawWireSphere(_sideKickAttackPoint.position,ATTACK_TOLERANCE_RANGE);
        if(_punchAttackPoint != null)
            Gizmos.DrawWireSphere(_punchAttackPoint.position,ATTACK_TOLERANCE_RANGE);
    }

    #region Attacks

    /// <summary>
    /// Hotkey: leftclick
    /// </summary>
    /// <param name="context"></param>
    private void Punch(InputAction.CallbackContext context)
    {
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
    private void SideKick(InputAction.CallbackContext context)
    {
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

    private void UpdateBlocking(bool state)
    {
        _animator.SetBool(BlockingID,_playerStats.IsBlocking = state);
        foreach (var v in _toBeBlocked)
        {
            if(state) v.Disable();
            else v.Enable();
        }
    }
    
    #endregion
}

/*
        var position = new Vector3(tForm.position.x + 0.75f,tForm.position.y,tForm.position.z);
        var dir = new Vector3(position.x, position.y - 1, position.z);
        var ray = new Ray(position, dir);

*/