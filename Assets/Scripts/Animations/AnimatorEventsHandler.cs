using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

public class AnimatorEventsHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;

    #region Jab

    public void JabAnimationStarted() => networkCharacterControllerPrototypeCustom.AnimationStarted(EAttackType.Jab);

    public void JabActivateHitbox() => networkCharacterControllerPrototypeCustom.ActivateHitBox(InputAttackType.Jab);


    public void JabDeactivateHitbox() => networkCharacterControllerPrototypeCustom.DeactivateHitBox();

    #endregion

    #region SideKick

    public void SideKickAnimationStarted() => networkCharacterControllerPrototypeCustom.AnimationStarted(EAttackType.Kick);

    public void SideKickActivateHitbox() => networkCharacterControllerPrototypeCustom.ActivateHitBox(InputAttackType.Sidekick);


    public void SideKickDeactivateHitbox() => networkCharacterControllerPrototypeCustom.DeactivateHitBox();

    #endregion
    
    #region Hook
    
    public void HookAnimationStarted() => networkCharacterControllerPrototypeCustom.AnimationStarted(EAttackType.Hook);

    public void HookActivateHitbox() => networkCharacterControllerPrototypeCustom.ActivateHitBox(InputAttackType.Hook);


    public void HookDeactivateHitbox() => networkCharacterControllerPrototypeCustom.DeactivateHitBox();
    
    #endregion

    public void AttackFinished() => networkCharacterControllerPrototypeCustom.isAllowedToAttack = true;
}