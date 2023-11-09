using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FighterCamera : MonoBehaviour
{

    private Transform[] _playerTransforms;
    public float yOffset = 1.5f;
    public float minDistance = 4f;
    private float xMin, xMax, yMin, yMax;
    private PlayerRotationScript _playerRotationScript;
    [FormerlySerializedAs("Player1")] public GameObject player1;
    [FormerlySerializedAs("Player2")] public GameObject player2;



    // Start is called before the first frame update

    void Awake()
    {
        _playerTransforms ??= new Transform[2];
    }

    public void SpawnInPlayer2(Transform t1, Transform t2)
    {
        _playerTransforms[0] = t1;
        _playerTransforms[1] = t2;

        _playerRotationScript ??= new(_playerTransforms[0], _playerTransforms[1]);


    }

    private void Update()
    {
        player1 = GameObject.FindGameObjectWithTag("Player1");
        player2 = GameObject.FindGameObjectWithTag("Player2");
        if(player1?.transform != null ) _playerTransforms[0] = player1.transform;
        if(player2?.transform != null ) _playerTransforms[1] = player2.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(_playerTransforms == null || 
           _playerTransforms[0] is null || 
           _playerTransforms[1] is null ||
           _playerTransforms.Length == 0)
            return;
        
        xMin = xMax = _playerTransforms[0].position.x;
        yMin = yMax = _playerTransforms[0].position.y;
        for (int i = 1; i < _playerTransforms.Length; i++)
        {
            if (_playerTransforms[i].position.x < xMin)
                xMin = _playerTransforms[i].position.x;
            
            if (_playerTransforms[i].position.x > xMax)
                xMax = _playerTransforms[i].position.x;
            
            if (_playerTransforms[i].position.y < yMin)
                yMin = _playerTransforms[i].position.y;
            
            if (_playerTransforms[i].position.y > yMax)
                yMax = _playerTransforms[i].position.y;
        }

        float xMiddle = (xMin + xMax) / 2;
        float yMiddle = (yMin + yMax) / 2;

        float distance = xMax - xMin;
        if (distance < minDistance)
            distance = minDistance;
        transform.position = new Vector3(xMiddle, yMiddle + yOffset, distance);
    }
}
