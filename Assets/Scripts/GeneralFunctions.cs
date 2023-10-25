using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
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
        print(statement + " [ CLASS: " + className + " | METHOD: " + methodName +" ]");
#else
        return;
#endif
    }


    public static void PrintDebugStatement(int statement) => PrintDebugStatement(statement.ToString());
}