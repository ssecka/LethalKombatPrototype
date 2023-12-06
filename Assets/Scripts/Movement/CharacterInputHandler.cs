using System;
using System.Security.Cryptography;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public struct NetworkInputData : INetworkInput
{
    public Vector2 _movementInput;
    public NetworkBool _isJumpPressed;
    public InputAttackType _InputAttackType;
}

/// <summary>
/// This class only recieves the inputs and sends it.
/// The checks happen in the other class!
/// </summary>
public class CharacterInputHandler : MonoBehaviour
{
    private Vector2 _moveInputVector = Vector2.zero;
    private bool _jumpWasPressed = false;
    private InputAttackType _inputAttackType = 0;

    private const long ATTACK_BLOCK_TIMER = 400;
    private long _lastBlocKUnixTime = 0;


    #region General Movement Inputs

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInputVector = context.ReadValue<Vector2>();
        _moveInputVector.y = 0;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        _jumpWasPressed = true;
    }

    #endregion

    #region Defensive Movement Inputs

    public void OnBlock(InputAction.CallbackContext context)
    {
        _inputAttackType = InputAttackType.Block;
    }

    #endregion

    #region Attack Based Inputs

    public void OnJab(InputAction.CallbackContext context)
    {
        // Only trigger on "keyDown"
        if (!context.started) return;
        _inputAttackType = InputAttackType.Jab;
    }

    public void OnSideKick(InputAction.CallbackContext context)
    {
        // Only trigger on "keyDown"

        if (!context.started) return;
        _inputAttackType = InputAttackType.Sidekick;
    }

    public void OnHook(InputAction.CallbackContext context)
    {
        // Only trigger on "keyDown"

        if (!context.started) return;
        _inputAttackType = InputAttackType.Hook;
    }

    #endregion

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new();
        networkInputData._movementInput = _moveInputVector;
        networkInputData._isJumpPressed = _jumpWasPressed;
        _jumpWasPressed = false;

        networkInputData._InputAttackType = _inputAttackType;
        _inputAttackType = InputAttackType.None;

        return networkInputData;
    }
}