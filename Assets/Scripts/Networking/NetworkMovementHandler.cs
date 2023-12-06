using System;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkMovementHandler : NetworkBehaviour
{
    private NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;

    [Header("Animation")] public Animator Animator;

    private InputAttackType _lastAnimationInput = 0;
    
    #region AnimationIDs

    private static readonly int SideKickID = Animator.StringToHash("SideKick");
    private static readonly int HookID = Animator.StringToHash("Punch");
    private static readonly int JabID = Animator.StringToHash("Jab");
    private static readonly int FWalking = Animator.StringToHash("FWalking");
    private static readonly int BWalking = Animator.StringToHash("BWalking");
    private static readonly int BlockingID = Animator.StringToHash("Blocking");
    private static readonly int JumpID = Animator.StringToHash("Jump");

    #endregion

    private void Awake()
    {
        _networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.IsProxy) return;
        
        if (GetInput(out NetworkInputData networkInputData))
        {
            var moveDir = networkInputData._movementInput;
            moveDir.Normalize();


            _networkCharacterControllerPrototypeCustom.Move(moveDir);
            if (networkInputData._isJumpPressed)
                _networkCharacterControllerPrototypeCustom.Jump();

            _lastAnimationInput = networkInputData._InputAttackType;
        }
    }

    public override void Render()
    {
        StartAttackAnimation(_lastAnimationInput);
        //_lastAnimationInput = 0;
    }

    private void StartAttackAnimation(InputAttackType inputAttackType)
    {
        switch (inputAttackType)
        {
            case InputAttackType.None:
                Block(false);
                break;
            case InputAttackType.Block:
                Block(true);
                break;
            case InputAttackType.Jab:
                Jab();
                break;
            case InputAttackType.Sidekick:
                SideKick();
                break;
            case InputAttackType.Hook:
                Hook();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(inputAttackType), inputAttackType, null);
        }
    }

    #region Attack Patterns

    private void Jab()
    {
        Animator.SetTrigger(JabID);
    }

    private void SideKick()
    {
        Animator.SetTrigger(SideKickID);
    }

    private void Hook()
    {
        Animator.SetTrigger(HookID);
    }

    private void Block(bool val)
    {
        //TODO
    }

    #endregion
}