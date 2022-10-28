using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Vector3 location()
    {
        return transform.TransformPoint(Vector3.zero);
    }

    public static PlayerSpawner[] GetAll()
    {
        return FindObjectsOfType<PlayerSpawner>();
    }

    public static int SpawnerCount()
    {
        return FindObjectsOfType<PlayerSpawner>().Length;
    }
}
