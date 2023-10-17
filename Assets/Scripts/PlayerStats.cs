using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Start is called before the first frame update

    private float _hp = 1000;
    
    void Start()
    {
        
    }
    
    
    public void TakeDamage(int dmgAmount)
    {
        var remaing = _hp - dmgAmount;
        if (remaing <= 0)
        {
            //throw new NotImplementedException("TODO: Implement Game End");
            Debug.Log("Im Deadge");
        }
        // Play Hit Animation
        _hp = remaing;

        Debug.Log("New Life: " + _hp);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
