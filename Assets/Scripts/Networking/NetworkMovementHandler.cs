using Fusion;
using UnityEngine;

public class NetworkMovementHandler : NetworkBehaviour
{
    private NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;

    private void Awake()
    {
        _networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            var moveDir = networkInputData._movementInput;
            moveDir.Normalize();


            _networkCharacterControllerPrototypeCustom.Move(moveDir);
            if (networkInputData._isJumpPressed)
                _networkCharacterControllerPrototypeCustom.Jump();

            _networkCharacterControllerPrototypeCustom.Attack(networkInputData._InputAttackType);
        }
    }
}