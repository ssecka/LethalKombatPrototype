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

    public void BlockAttack()
    {
        networkCharacterControllerPrototypeCustom.isAllowedToAttack = true;
    }
    
    public void AttackFinished() => networkCharacterControllerPrototypeCustom.isAllowedToAttack = true;
}