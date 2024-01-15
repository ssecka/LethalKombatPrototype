using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

public class AnimatorEventsHandler : MonoBehaviour
{
    public NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;

    #region Jab

    public void JabAnimationStarted() => networkCharacterControllerPrototypeCustom.AnimationStarted(InputAttackType.Jab);

    public void JabActivateHitbox() => networkCharacterControllerPrototypeCustom.ActivateHitBox(InputAttackType.Jab);


    public void JabDeactivateHitbox() => networkCharacterControllerPrototypeCustom.DeactivateHitBox();

    #endregion

    #region SideKick

    public void SideKickAnimationStarted() => networkCharacterControllerPrototypeCustom.AnimationStarted(InputAttackType.Sidekick);

    public void SideKickActivateHitbox() => networkCharacterControllerPrototypeCustom.ActivateHitBox(InputAttackType.Sidekick);


    public void SideKickDeactivateHitbox() => networkCharacterControllerPrototypeCustom.DeactivateHitBox();

    #endregion
    
    #region Hook
    
    public void HookAnimationStarted() => networkCharacterControllerPrototypeCustom.AnimationStarted(InputAttackType.Hook);

    public void HookActivateHitbox() => networkCharacterControllerPrototypeCustom.ActivateHitBox(InputAttackType.Hook);


    public void HookDeactivateHitbox() => networkCharacterControllerPrototypeCustom.DeactivateHitBox();
    
    #endregion
    
    #region LowKick
    
    public void LowKickAnimationStarted() => networkCharacterControllerPrototypeCustom.AnimationStarted(InputAttackType.Lowkick);

    public void LowKickActivateHitbox() => networkCharacterControllerPrototypeCustom.ActivateHitBox(InputAttackType.Lowkick);


    public void LowKickDeActivateHitbox() => networkCharacterControllerPrototypeCustom.DeactivateHitBox();
    
    #endregion
    
    #region FireBall

    public void SpawnFireBall() => networkCharacterControllerPrototypeCustom.LaunchFireball();

    public void FireBallDamage() => networkCharacterControllerPrototypeCustom.ActivateHitBox(InputAttackType.FireBall);
    
    
    #endregion
    
    

    public void BlockAttack()
    {
        networkCharacterControllerPrototypeCustom._canQueueAttack = true;
    }
    
    public void AttackFinished() => networkCharacterControllerPrototypeCustom._canQueueAttack = true;

    public void KnockOut_RemovePlayer()
    {
        networkCharacterControllerPrototypeCustom.KnockOut();
    }
}