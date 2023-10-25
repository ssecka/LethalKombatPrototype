using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManagerScript : MonoBehaviour
{
    // Start is called before the first frame update

    private static InputManagerScript _instance;

    public static InputManagerScript Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = new(); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayerInputManager.instance.JoinPlayer(0);
        PlayerInputManager.instance.JoinPlayer(1);
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        GeneralFunctions.PrintDebugStatement("Player joined the game");
    }    
    
    private void OnPlayerLeft(PlayerInput playerInput)
    {
        
    }
}
