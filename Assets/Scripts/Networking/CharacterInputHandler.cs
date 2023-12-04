using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputHandler : MonoBehaviour
{
    Vector3 _moveInputVector = Vector3.zero;    
    
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInputVector = context.ReadValue<Vector2>();
        _moveInputVector.y = 0;
    }
    
    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new();
        networkInputData._movementInput = _moveInputVector;
        
        return networkInputData;
    }
}