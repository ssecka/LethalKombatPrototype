using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Movement : MonoBehaviour
{
    private const float MVSPEED = .105f;
    private const float JUMPPOWER = 250f;
    private const float GRAVITY_MULTI = 1.015f;

    
    private bool _canJump = true;
    
    [SerializeField] private Transform _transform;
    [SerializeField] private Rigidbody _rb;
    
    
    private Vector2 _movDir = Vector2.zero;

    private PlayerInput _playerControlls;

    private InputAction _move, _fire, _jump;

    private void Awake()
    {
        if(_transform == null) Debug.Log("transform not set!");
        _playerControlls = new();
    }

    private void OnEnable()
    {
        _move = _playerControlls.Player.Move;
        _move.Enable();
        
        _fire = _playerControlls.Player.Fire;
        _fire.Enable();
        _fire.performed += Fire;
        
        _jump = _playerControlls.Player.Jump;
        _jump.Enable();
        _jump.performed += Jump;
    }

    private void OnDisable()
    {
        _move.Disable();
        _fire.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
            velocity = new(velocity.x,velocity.y*GRAVITY_MULTI,velocity.z);
            _rb.velocity = velocity;
        }
    }

    private void FixedUpdate()
    {
        var newX = _transform.position.x + ((-_movDir.x) * MVSPEED);
        _transform.position = new(newX,((Component)this).transform.position.y,0);
    }

    private void Fire(InputAction.CallbackContext context)
    {
        Debug.Log("FIRED");
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!_canJump) return;
        _rb.AddForce(Vector3.up * JUMPPOWER);
        _canJump = false;
    }
}
