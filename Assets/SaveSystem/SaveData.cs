using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float[] position;

    public SaveData(Player player)
    {
        position = Position(player.transform);
    }

    public SaveData()
    {
        return;
    }

    private float[] Position(Transform transform)
    {
        float[] position = new float[3];
        position[0] = transform.position.x;
        position[1] = transform.position.y;
        position[2] = transform.position.z;
        return position;
    }
}
