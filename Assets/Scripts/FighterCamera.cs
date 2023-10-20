using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterCamera : MonoBehaviour
{

    public Transform[] playerTransforms;
    public float yOffset = 2.0f;
    public float minDistance = 7.5f;
    private float xMin, xMax, yMin, yMax;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(playerTransforms.Length == 0)
            return;
        xMin = xMax = playerTransforms[0].position.x;
        yMin = yMax = playerTransforms[0].position.y;
        for (int i = 1; i < playerTransforms.Length; i++)
        {
            if (playerTransforms[i].position.x < xMin)
                xMin = playerTransforms[i].position.x;
            
            if (playerTransforms[i].position.x > xMax)
                xMax = playerTransforms[i].position.x;
            
            if (playerTransforms[i].position.y < yMin)
                yMin = playerTransforms[i].position.y;
            
            if (playerTransforms[i].position.y > yMax)
                yMax = playerTransforms[i].position.y;
        }

        float xMiddle = (xMin + xMax) / 2;
        float yMiddle = (yMin + yMax) / 2;

        float distance = xMax - xMin;
        if (distance < minDistance)
            distance = minDistance;
        transform.position = new Vector3(xMiddle, yMiddle + yOffset, distance);

    }
}
