using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using System.Diagnostics;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine.Animations;

public class GeneralFunctions : MonoBehaviour
{
    /// <summary>
    /// Used as Debug Print Statement. Is only called in debug builds.
    /// Also has diagnostic information regarding the class and method from which this is called
    /// </summary>
    /// <param name="statement">this is the debug message</param>
    public static void PrintDebugStatement(string statement)
    {
#if DEBUG
        statement ??= "N/A";
        var trace = new StackTrace();
        var frame = trace.GetFrame(1);
        var method = frame.GetMethod();
        var methodName = method.Name;
        var className = method.ReflectedType.Name;
        print(statement + " [ CLASS: " + className + " | METHOD: " + methodName + " ]");
#else
        return;
#endif
    }

    public static void PrintDebugStatement(object statement) => PrintDebugStatement(statement.ToString());

    public static void PlaySoundByEnum(EAttackType attackType, ref SoundEffects soundEffects)
    {
        //PrintDebugStatement("Playing sound: " + attackType.ToString());
        switch (attackType)
        {
            case EAttackType.Jab:
                soundEffects.PlayJabSound(false);
                break;
            case EAttackType.JabHit:
                soundEffects.PlayJabSound(true);
                break;
            case EAttackType.Kick:
                soundEffects.PlayKickSound(false);
                break;
            case EAttackType.KickHit:
                soundEffects.PlayKickSound(true);
                break;
            case EAttackType.Block:
                soundEffects.PlayBlockSound();
                break;
            default:
                throw new InvalidEnumArgumentException("Invalid Enum");
        }
    }
}