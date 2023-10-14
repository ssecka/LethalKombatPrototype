using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Movement : MonoBehaviour
{
    private const float _mvspeed = 5f;
    private const float _jmpHight = 1f;

    [SerializeField] private Rigidbody _rb;

    private Vector2 _movDir = Vector2.zero;

    private PlayerControlls _playerControlls;
    
    private InputAction _move;
    private InputAction _fire;

    private void Awake()
    {
        _playerControlls = new();
    }

    private void OnEnable()
    {
        _move = _playerControlls.Player.Move;
        _move.Enable();

        _fire = _playerControlls.Player.Fire;
        _fire.Enable();
        _fire.performed += Fire;
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

    // Update is called once per frame
    void Update()
    {
        _movDir = _move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        var velocity = _rb.velocity;
        _rb.velocity = new((-_movDir.x) * _mvspeed,velocity.y,velocity.z);
    }

    private void Fire(InputAction.CallbackContext context)
    {
        Debug.Log("FIRED");
    }
}
