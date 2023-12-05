﻿using System;
using UnityEngine;
using Fusion;

public class HPHandler : NetworkBehaviour
{
    private const short STARTHING_HP = 1_000;
    
    [Networked(OnChanged = nameof(OnHPChanged))]
    private short HP { get; set; } // We only use short for performance
    
    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    private bool isInit = false;


    private void Start()
    {
        HP = STARTHING_HP;
        isDead = false;
    }

    // Only called by server
    public void OnHitTaken(short amount, bool isBlocking = false)
    {
        if (isDead) return; // -> we deadge

        HP -= amount;

        Debug.Log($"{Time.time} {transform.name} took damage, HP left: {HP}");

        if (HP <= 0)
        {
            isDead = true;
            
            Debug.Log($"{Time.time} {transform.name} is dead.");

        }
    }

    private static void OnHPChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.HP}");
    }

    private static void OnStateChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"{Time.time} OnStateChanged value {changed.Behaviour.isDead}");

    }
    
}