using Fusion;
using UnityEngine;

public class NetworkMovementHandler : NetworkBehaviour
{
    private NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;

    private void Awake()
    {
        _networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame --> locally
    void Update()
    {
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            var moveDir = networkInputData._movementInput;
            moveDir.Normalize();
            
            
            _networkCharacterControllerPrototypeCustom.Move(moveDir);
            if(networkInputData._isJumpPressed)
                _networkCharacterControllerPrototypeCustom.Jump();
        }
    }
}