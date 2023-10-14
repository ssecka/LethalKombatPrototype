using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Movement : MonoBehaviour
{
    private const float _mvspeed = 5f;

    [SerializeField] private InputAction _playerControls;
    [SerializeField] private Rigidbody _rb;

    private Vector2 _movDir = Vector2.zero;
    
    private void OnEnable() => _playerControls.Enable();

    private void OnDisable() => _playerControls.Disable();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _movDir = _playerControls.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        var velocity = _rb.velocity;
        _rb.velocity = new(_movDir.x * _mvspeed,velocity.y,velocity.z);
    }
}
