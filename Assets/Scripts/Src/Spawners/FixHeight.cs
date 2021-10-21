using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static game.assets.utilities.GameUtils;

public class FixHeight : MonoBehaviour
{
    void Start()
    {
        Vector3 currentPosition = transform.position;
        float bestHeight = getTerrainHeight(currentPosition);
        transform.position = new Vector3(currentPosition.x, bestHeight, currentPosition.z);
    }
}
