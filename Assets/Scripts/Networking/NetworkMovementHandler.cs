using System;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkMovementHandler : NetworkBehaviour
{
    private NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;

    private NetworkMecanimAnimator _networkAnimator;

    private InputAttackType _lastAnimationInput = 0;

    private bool _proxyTiggered, _hostTriggered;
    
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
        _networkAnimator = GetComponentInChildren<NetworkMecanimAnimator>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.IsProxy)
        {
            _proxyTiggered = true;
            return;
        }

        _hostTriggered = true;
        
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
        // Setting _lastAnimationInput to 0 would cause issues.
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
        _networkAnimator.SetTrigger(JabID);
    }

    private void SideKick()
    {
        _networkAnimator.SetTrigger(SideKickID);
    }

    private void Hook()
    {
        _networkAnimator.SetTrigger(HookID);
    }

    private void Block(bool val)
    {
        //TODO
    }

    #endregion
}