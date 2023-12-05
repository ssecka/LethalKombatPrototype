using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputHandler : MonoBehaviour
{
    private Vector2 _moveInputVector = Vector2.zero;
    private bool _jumpWasPressed = false;
    private bool _isAllowedToAttack = true;
    private InputAttackType _inputAttackType = 0;

    public void AttackFinished()
    {
        Debug.Log("Attack finished.");
        _isAllowedToAttack = true;
    }

    public void BlockAttack()
    {
        Debug.Log("Attacks blocked.");
        _isAllowedToAttack = false;
    }


    #region General Movement Inputs

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInputVector = context.ReadValue<Vector2>();
        _moveInputVector.y = 0;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
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

    public void OnPunch(InputAction.CallbackContext context)
    {
        _inputAttackType = InputAttackType.Punch;
    }

    public void OnSideKick(InputAction.CallbackContext context)
    {
        _inputAttackType = InputAttackType.Sidekick;
    }

    public void OnHook(InputAction.CallbackContext context)
    {
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
        _inputAttackType = 0;

        return networkInputData;
    }
}