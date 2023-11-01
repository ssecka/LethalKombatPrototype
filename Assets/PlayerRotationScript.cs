using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotationScript : MonoBehaviour
{
    private Transform _playerOne, _playerTwo;
    private readonly Quaternion _faceRight = Quaternion.Euler(0, -90f, 0);
    private readonly Quaternion _faceLeft = Quaternion.Euler(0, 90f, 0);
    
    
    
    private PlayerRotationScript(){}
    
    public PlayerRotationScript(Transform playerOne, Transform playerTwo)
    {
        _playerOne = playerOne;
        _playerTwo = playerTwo;
    }
    
    public void UpdatePlayerRotation()
    {
        var xOne = _playerOne.position.x;
        var xTwo = _playerTwo.position.x;
        var faceRight = xOne > xTwo;
        _playerOne.rotation =  faceRight ? _faceRight : _faceLeft;
        _playerTwo.rotation =  !faceRight ? _faceRight : _faceLeft;
        
        
    }
    
    public static void StaticUpdatePlayerRotation(Transform t1, Transform t2)
    {
        var xOne = t1.position.x;
        var xTwo = t2.position.x;
        var faceRight = xOne > xTwo;
    
        Quaternion fr = Quaternion.Euler(0, -90f, 0);
        Quaternion fl = Quaternion.Euler(0, 90f, 0);
        
        t1.rotation =  faceRight ? fr : fl;
        t2.rotation =  !faceRight ? fr : fl;
        
        
    }
    
    
}
