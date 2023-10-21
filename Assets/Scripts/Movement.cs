using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Movement : MonoBehaviour
{
    private const float MVSPEED = .105f;
    private const float JUMPPOWER = 250f;
    private const float GRAVITY_MULTI = 1.015f;

    private Animator _animator;

    private bool _canJump = true;

    [SerializeField] private Transform _transform;
    [SerializeField] private Rigidbody _rb;


    private Vector2 _movDir = Vector2.zero;

    private PlayerInput _playerControlls;

    private InputAction _move, _punch, _jump, _legSweep;

    private void Awake()
    {
        if (_transform == null) Debug.Log("transform not set!");
        _playerControlls = new();
    }


    private void OnEnable()
    {
        _move = _playerControlls.Player.Move;
        _move.Enable();

        _punch = _playerControlls.Player.Punch;
        _punch.Enable();
        _punch.performed += Punch;

        _legSweep = _playerControlls.Player.LegSweep;
        _legSweep.Enable();
        _legSweep.performed += LegSweep;

        _jump = _playerControlls.Player.Jump;
        _jump.Enable();
        _jump.performed += Jump;
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
        var newX = _transform.position.x + ((-_movDir.x) * MVSPEED);
        _transform.position = new(newX, ((Component)this).transform.position.y, 0);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!_canJump) return;
        _rb.AddForce(Vector3.up * JUMPPOWER);
        _canJump = false;
    }


    #region Attacks

    /// <summary>
    /// Hotkey: leftclick
    /// </summary>
    /// <param name="context"></param>
    private void Punch(InputAction.CallbackContext context)
    {
        GeneralFunctions.PrintDebugStatement("Punch");

        //TODO: Add animation for front punch
        _animator.SetTrigger("SideKick");

        var tForm = _transform;
        var distance = 2.5f;

        // TODO: ADJUST THE DIR FOR THE CURRENT POS.
        var dir = new Vector3(-1, 0, 0);
        var position = new Vector3(tForm.position.x + 0.75f, tForm.position.y + 1.25f, tForm.position.z);
        var ray = new Ray(position, dir);
        var endLine = new Vector3(tForm.position.x + (distance * dir.x), tForm.position.y + 1.25f, tForm.position.z);

        Debug.DrawLine(position, endLine, Color.red, 100f);
        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            if (hit.transform.gameObject.TryGetComponent(out PlayerStats otherPlayer))
            {
                otherPlayer.TakeDamage(100);
            }
            else
            {
                GeneralFunctions.PrintDebugStatement("I hitted :" + hit);
            }
        }
    }

    /// <summary>
    /// Hotkey: Rightclick
    /// </summary>
    /// <param name="context"></param>
    private void LegSweep(InputAction.CallbackContext context)
    {
        GeneralFunctions.PrintDebugStatement("LegSweep");


        _animator.SetTrigger("Kick");

        var tForm = _transform;
        var distance = 2.5f;

        // TODO: ADJUST THE DIR FOR THE CURRENT POS.
        var position = new Vector3(tForm.position.x + 0.75f, tForm.position.y + 0.5f, tForm.position.z);
        var dir = new Vector3(-position.x, position.y, position.z);
        var ray = new Ray(position, dir);

        var endLine = new Vector3(tForm.position.x + (distance * dir.x), tForm.position.y + 0.5f, tForm.position.z);

        Debug.DrawLine(position, endLine, Color.red, 100f);
        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            if (hit.transform.gameObject.TryGetComponent(out PlayerStats otherPlayer))
            {
                otherPlayer.TakeDamage(250);
            }
            else
            {
                GeneralFunctions.PrintDebugStatement("I hitted :" + hit);
            }
        }
    }

    #endregion
}

/*
        var position = new Vector3(tForm.position.x + 0.75f,tForm.position.y,tForm.position.z);
        var dir = new Vector3(position.x, position.y - 1, position.z);
        var ray = new Ray(position, dir);

*/