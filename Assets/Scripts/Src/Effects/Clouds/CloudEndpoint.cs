using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CloudEndpoint : MonoBehaviour
{
    private List<GameObject> finishedClouds = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        finishedClouds.Add(other.gameObject);
    }

    public List<GameObject> AnyFinishedClouds()
    {
        return finishedClouds;
    }
}
