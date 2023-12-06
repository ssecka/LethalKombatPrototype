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

    public override void Spawned()
    {
    }
    

    public override void Render()
    {

        UpdateAnimations();
        
        //StartAttackAnimation(_lastAnimationInput);
        // Setting _lastAnimationInput to 0 would cause issues.
        //_lastAnimationInput = 0;
    }

    private void UpdateAnimations()
    {
        if (_lastVisibleJab < _networkCharacterControllerPrototypeCustom.JabCount)
        {
            _networkAnimator.SetTrigger(JabID,true);
            _lastVisibleJab = _networkCharacterControllerPrototypeCustom.JabCount;
        }
    }
}