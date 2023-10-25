using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    private GameManager _instance;
    public GameManager Instance{ get{return _instance;} }

    [SerializeField] private GameObject _spawnP1;
    [SerializeField] private GameObject _spawnP2;

    [SerializeField] private InputAction _joinAction;
    
    private void Awake()
    {
        _instance ??= new();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnPlayerJoined(PlayerInput input)
    {
        
    }
    
    void OnPlayerLeft(PlayerInput input)
    {
        
    }
    
}
