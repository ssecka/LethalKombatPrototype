﻿using System;
using UnityEngine;
using Fusion;

public class HPHandler : NetworkBehaviour
{
    private const short STARTING_HP = 1_000;
    [Networked(OnChanged = nameof(OnHPChanged))]
    
    private short HP { get; set; } // We only use short for performance

    [Networked]
    public short Round { get; set; } = 0;
    
    [Networked(OnChanged = nameof(OnStateChanged)), HideInInspector]
    public bool isDead { get; set; }

    [Networked(OnChanged = nameof(TriggerResetHP))]
    public int ResetHpTrigger { get; set; }
    
    private bool isInit = false;

    public HealthBarScript healthBarScript;
    public GameObject hitEffect;
    public GameObject blockEffect;
    public GameObject hitPoint;

    private static byte _cntHlp = 0;
    private Animator _animator;

    
    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        healthBarScript = GetComponent<HealthBarScript>();
        healthBarScript.SetMaxHealth(STARTING_HP, transform.name);
        HP = STARTING_HP;
        isDead = false;
    }

    // Only called by server
    public bool OnHitTaken(short amount, bool isBlocking = false)
    {
        if (isDead) return true; // -> we deadge
        
        _animator.SetTrigger("Hit");
       Runner.Spawn(hitEffect, hitPoint.gameObject.transform.position);

       if (isBlocking)
        {
            amount /= 10;
            print("BLOCKED!");
            Runner.Spawn(blockEffect, hitPoint.transform.position);
            
            int yRotation = Convert.ToInt32(transform.eulerAngles.y);
            if (yRotation == 90)
            {
                transform.Translate(Vector3.forward * .5f);
            }
            else
            {
                transform.Translate(Vector3.back * .5f);
            }
        }
        
        HP -= amount;

        Debug.Log($"{transform.name} took damage, HP left: {HP}");

        if (HP <= 0)
        {
            Round++;
            isDead = true;
            Debug.Log($"{transform.name} is dead.");
            
        }

        return isDead;
    }

    public void ResetHp()
    {
        HP = STARTING_HP;
        isDead = false;
    }

    private static void OnHPChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"OnHPChanged value {changed.Behaviour.HP}");
        changed.Behaviour.healthBarScript.SetHealth(changed.Behaviour.HP, changed.Behaviour.transform.name);
    }

    private static void OnStateChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"OnStateChanged value {changed.Behaviour.isDead}");
    }

    private static void TriggerResetHP(Changed<HPHandler> changed)
    {
        changed.Behaviour.healthBarScript.SetRound(changed.Behaviour.Round,changed.Behaviour.transform.name);
        changed.Behaviour.healthBarScript.ResetHealthBarHost(STARTING_HP); 
        changed.Behaviour.healthBarScript.ResetHealthBarClient(STARTING_HP);
    }
}