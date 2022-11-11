using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRigSpawner : SimulationBehaviour, ISpawned
{
    public GameObject playerRigPrefab;
    public void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Camera cam = Instantiate(playerRigPrefab, transform).GetComponent<Camera>();
            cam.depth = float.MaxValue;
        }
        /*if (!Object.HasStateAuthority)
        {
            transform?.Find("PlayerRigFacade")?.gameObject?.SetActive(false);
        }*/
    }
}
