using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManagerScript : MonoBehaviour
{

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void OnPlayerJoined(PlayerInput playerInput)
    {
        if(GameObject.Find("Main Camera").TryGetComponent(out FighterCamera camera))
            camera.SpawnInPlayer2();
    }

}
