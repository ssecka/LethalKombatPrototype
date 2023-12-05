using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputHandler : MonoBehaviour
{
    private Vector2 _moveInputVector = Vector2.zero;
    private bool _jumpWasPressed = false;
    
    
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInputVector = context.ReadValue<Vector2>();
        _moveInputVector.y = 0;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        _jumpWasPressed = true;
    }
    
    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new();
        networkInputData._movementInput = _moveInputVector;
        networkInputData._isJumpPressed = _jumpWasPressed;
        _jumpWasPressed = false;
        
        
        return networkInputData;
    }
}