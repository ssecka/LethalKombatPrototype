using System.ComponentModel;
using UnityEngine;

public class SpawnPointHandler
{
    private static int  spwanCounter = 0;

    public static (Vector2,Quaternion) GetSpawnPoint()
    {
        Vector2 vector2;
        Quaternion quaternion;
        switch (spwanCounter)
        {
            case(0):
                vector2 = new(4.5f, 0.5f);
                quaternion = Quaternion.Euler(0f, -90f, 0f);
                break;
            case(1):
                vector2 = new(-4.5f, 0.5f);
                quaternion = Quaternion.Euler(0f, 90f, 0f);
                break;
            default:
                throw new InvalidEnumArgumentException("UNSUPPORTED PLAYER COUNT");
        }

        spwanCounter = (++spwanCounter % 2);

        return (vector2, quaternion);
    }
}
