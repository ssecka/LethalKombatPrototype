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

    private int _lastVisibleJump, _lastVisibleJab, _lastVisibleKick, _lastVisibleHook;
    
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

    public override void Render()
    {

        UpdateAnimations();

    }

    public override void FixedUpdateNetwork()
    {
    }

    private void UpdateAnimations()
    {
        // Safe some typing...
        var controller = _networkCharacterControllerPrototypeCustom;
        var useInterpolation = true; //testing...
        
        
        var jabCount =  useInterpolation ?  controller.InterpolatedJabCount : controller.JabCount;
        var hookCount = useInterpolation ?  controller.InterpolatedHookCount : controller.HookCount;
        var kickCount = useInterpolation ?  controller.InterpolatedKickCount : controller.KickCount;
        var jumpCount = useInterpolation ?  controller.InterpolatedJumpCount : controller.JumpCount;
        
        if (_lastVisibleJab < jabCount)
        {
            _networkAnimator.SetTrigger(JabID,true);
            _lastVisibleJab = controller.JabCount;
        }

        if (_lastVisibleHook < hookCount)
        {
            _networkAnimator.SetTrigger(HookID,true);
            _lastVisibleHook = controller.HookCount;
        }

        if (_lastVisibleKick < kickCount)
        {
            _networkAnimator.SetTrigger(SideKickID, true);
            _lastVisibleKick = controller.KickCount;
        }

        if (_lastVisibleJump < jumpCount)
        {
            controller.Jump();
            //_networkAnimator.SetTrigger(JumpID, true);
            _lastVisibleJump = controller.JumpCount;
        }
    }
}