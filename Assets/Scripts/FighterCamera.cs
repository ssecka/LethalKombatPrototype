using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
// ReSharper disable Unity.NoNullPropagation

public class FighterCamera : MonoBehaviour
{

    private const string PLAYER_ONE_TAG_SEARCH_NAME = "Player1";
    private const string PLAYER_TWO_TAG_SEARCH_NAME = "Player2";
    
    private Transform[] _playerTransforms;
    public float YOffset = 1.5f;
    public float MinDistance = 5f;
    private float _xMin, _xMax, _yMin, _yMax;
    private PlayerRotationScript _playerRotationScript;
    [SerializeField] private GameObject Player1;
    [SerializeField] private GameObject Player2;



    // Start is called before the first frame update

    void Awake()
    {
        _playerTransforms ??= new Transform[2];
    }

    public void SpawnInPlayer2(Transform t1, Transform t2)
    {
        _playerTransforms ??= new Transform[2];
        _playerTransforms[0] = t1;
        _playerTransforms[1] = t2;

        _playerRotationScript ??= new(_playerTransforms[0], _playerTransforms[1]);


    }

    private void Update()
    {

        Player1 = GameObject.FindGameObjectWithTag(PLAYER_ONE_TAG_SEARCH_NAME);
        Player2 = GameObject.FindGameObjectWithTag(PLAYER_TWO_TAG_SEARCH_NAME);
        
        // falls errors den expensive != null operator usen!
        
        if(Player1?.transform is not null ) _playerTransforms[0] = Player1.transform;
        if(Player2?.transform is not null ) _playerTransforms[1] = Player2.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(_playerTransforms?[0] is null || 
           _playerTransforms[1] is null ||
           _playerTransforms.Length == 0)
            return;
        
        _xMin = _xMax = _playerTransforms[0].position.x;
        _yMin = _yMax = _playerTransforms[0].position.y;
        
        // Calculate the lowest and highest X and Y
        int i = 1, len = _playerTransforms.Length;
        do
        {
            var curX = _playerTransforms[i].position.x;
            var curY = _playerTransforms[i].position.y;
            
            _xMin = Math.Min(_xMin, curX);
            _xMax = Math.Max(_xMax, curX);
            
            _yMin = Math.Min(_yMin, curY);
            _yMax = Math.Max(_yMax, curY);
            
        } while (++i < len);

        
        #region old
        // for (int i = 1; i < _playerTransforms.Length; i++)
        // {
        //     // if (_playerTransforms[i].position.x < _xMin)
        //     //     _xMin = _playerTransforms[i].position.x;
        //     //
        //     // if (_playerTransforms[i].position.x > _xMax)
        //     //     _xMax = _playerTransforms[i].position.x;
        //     //
        //     // if (_playerTransforms[i].position.y < _yMin)
        //     //     _yMin = _playerTransforms[i].position.y;
        //     //
        //     // if (_playerTransforms[i].position.y > _yMax)
        //     //     _yMax = _playerTransforms[i].position.y;
        // }
        #endregion
        
        float xMiddle = (_xMin + _xMax) / 2;
        float yMiddle = (_yMin + _yMax) / 2;

        float distance = Math.Clamp(_xMax - _xMin,MinDistance,200f);
        
        transform.position = new Vector3(xMiddle, yMiddle + YOffset, distance);
    }
}
