using System;
using UnityEngine;
using Fusion;

public class HPHandler : NetworkBehaviour
{
    private const short STARTING_HP = 1_000;
    [Networked(OnChanged = nameof(OnHPChanged))]
    private short HP { get; set; } // We only use short for performance
    
    [Networked(OnChanged = nameof(OnStateChanged)), HideInInspector]
    public bool isDead { get; set; }
    private bool isInit = false;
    
    

    private void Start()
    {
        HP = STARTING_HP;
        isDead = false;
    }

    // Only called by server
    public void OnHitTaken(short amount, bool isBlocking = false)
    {
        if (isDead) return; // -> we deadge

        HP -= amount;

        Debug.Log($"{transform.name} took damage, HP left: {HP}");

        if (HP <= 0)
        {
            isDead = true;
            
            Debug.Log($"{transform.name} is dead.");

        }
    }

    private static void OnHPChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"OnHPChanged value {changed.Behaviour.HP}");
    }

    private static void OnStateChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"OnStateChanged value {changed.Behaviour.isDead}");

    }
    
}