using System;
using UnityEngine;
using Fusion;

public class HPHandler : NetworkBehaviour
{
    private const short STARTING_HP = 1_000;
    public short _playerNum;
    [Networked(OnChanged = nameof(OnHPChanged))]
    
    private short HP { get; set; } // We only use short for performance
    private short Round { get; set; }
    
    [Networked(OnChanged = nameof(OnStateChanged)), HideInInspector]
    public bool isDead { get; set; }
    private bool isInit = false;

    public HealthBarScript healthBarScript;
    
    

    private void Start()
    {
        healthBarScript = GetComponent<HealthBarScript>();
        healthBarScript.SetMaxHealth(STARTING_HP,transform.name);
        Round = 0;
        HP = STARTING_HP;
        isDead = false;
    }

    // Only called by server
    public void OnHitTaken(short amount, bool isBlocking = false)
    {
        if (isDead) return; // -> we deadge

        if (isBlocking)
        {
            amount /= 10;
            print("BLOCKED!");
        }
        
        HP -= amount;

        Debug.Log($"{transform.name} took damage, HP left: {HP}");

        if (HP <= 0)
        {
            Round++;
            isDead = true;
            
            Debug.Log($"{transform.name} is dead.");

        }
    }

    private static void OnHPChanged(Changed<HPHandler> changed)
    {
        //healthbBarScript.SetHealth(changed.Behaviour.HP, transform.name);
        Debug.Log($"OnHPChanged value {changed.Behaviour.HP}");
        changed.Behaviour.healthBarScript.SetHealth(changed.Behaviour.HP, changed.Behaviour.transform.name);
        if (changed.Behaviour.HP <= 0)
        {
            changed.Behaviour.healthBarScript.SetRound(changed.Behaviour.Round,changed.Behaviour.transform.name);
            changed.Behaviour.healthBarScript.SetMaxHealth(1000, changed.Behaviour.transform.name);
        }

    }

    private static void OnStateChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"OnStateChanged value {changed.Behaviour.isDead}");

    }
    
}