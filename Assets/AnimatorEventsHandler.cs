using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

public class AnimatorEventsHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [FormerlySerializedAs("_toCall")] public NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;
    public void JabAnimationStarted() => networkCharacterControllerPrototypeCustom.AnimationStarted(EAttackType.Jab);
    
    public void JabActivateHitbox() => networkCharacterControllerPrototypeCustom.ActivateHitBox(InputAttackType.Jab);

    public void AttackFinished() => networkCharacterControllerPrototypeCustom.isAllowedToAttack = true;

    public void JabDeactivateHitbox() => networkCharacterControllerPrototypeCustom.DeactivateHitBox();

}
