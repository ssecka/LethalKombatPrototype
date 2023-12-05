using DefaultNamespace;
using Fusion;
using UnityEngine;

//TODO: optimize for bits lateron --> fater networking
public struct NetworkInputData : INetworkInput
{
    public Vector2 _movementInput;
    public NetworkBool _isJumpPressed;
    public InputAttackType _InputAttackType;
}