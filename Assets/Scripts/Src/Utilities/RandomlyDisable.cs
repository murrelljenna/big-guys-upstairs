using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomlyDisable : MonoBehaviour
{
    public GameObject gameObjectToDisable;

    public void Start()
    {
        if (gameObjectToDisable == null && Random.RandomRange(1, 10) < 4)
        {
            return;
        }

        ;
    }
}
