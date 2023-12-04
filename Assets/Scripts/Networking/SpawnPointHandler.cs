using System.ComponentModel;
using UnityEngine;

public class SpawnPointHandler
{
    private static int  spwanCounter = 0;

    public static (Vector3,Quaternion) GetSpawnPoint()
    {
        Vector3 vector3;
        Quaternion quaternion;
        switch (spwanCounter)
        {
            case(0):
                vector3 = new(4.5f, 0.5f,0f);
                quaternion = Quaternion.Euler(0f, -90f, 0f);
                break;
            case(1):
                vector3 = new(-4.5f, 0.5f,0f);
                quaternion = Quaternion.Euler(0f, 90f, 0f);
                break;
            default:
                throw new InvalidEnumArgumentException("UNSUPPORTED PLAYER COUNT");
        }

        spwanCounter = (++spwanCounter % 2);

        return (vector3, quaternion);
    }
}
