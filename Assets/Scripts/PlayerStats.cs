using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    public HealthBarScript HealthBarScript;
    [SerializeField] private int maxhealth = 1000;
    [SerializeField] private int currenthealth;

    // Start is called before the first frame update
    void Start()
    {
        HealthBarScript ??= new();
        currenthealth = maxhealth;
        HealthBarScript.SetMaxHealth(maxhealth);

    }
    
    // Update is called once per frame
    void Update()
    {
        //Testing if HealthBars are getting updated(works)
        /*if (Input.GetKeyDown(KeyCode.M))
        {
            TakeDamage(20);
        }*/
    }
    
    
    public void TakeDamage(int dmgAmount)
    {
        currenthealth -= dmgAmount;
        if (currenthealth <= 0)
        {
            //throw new NotImplementedException("TODO: Implement Game End");
            GeneralFunctions.PrintDebugStatement("Im Deadge");
        }
        // Play Hit Animation
        
        
        HealthBarScript.SetHealth(currenthealth);

        GeneralFunctions.PrintDebugStatement("New Life: " + currenthealth);

    }
}
