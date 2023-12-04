using System.ComponentModel;
using UnityEngine;

public class SpawnPointHandler
{
    private static int  spawnCounter = 0;

    public static (Vector3,Quaternion) GetSpawnPoint()
    {
        Vector3 vector3;
        Quaternion quaternion;
        
        // Vector wird warum auch immer mit 2 Multipliziert, deshhalb werden alle werte durch 2 geteilt
        
        switch (spawnCounter++)
        {
            case 0:
                vector3 = new(4.5f / 2f,0,0f);
                quaternion = Quaternion.Euler(0f, 90f, 0f);
                break;
            case(1):
                vector3 = new(-4.5f / 2f, 0,0f);
                quaternion = Quaternion.Euler(0f, -90f, 0f);
                break;
            default:
                throw new InvalidEnumArgumentException("UNSUPPORTED PLAYER COUNT");
        }

        Debug.Log(spawnCounter);
        spawnCounter %= 2;

        return (vector3, quaternion);
    }
}
