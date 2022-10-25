using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRigSpawner : SimulationBehaviour, ISpawned
{
    public GameObject playerRigPrefab;
    public void Spawned()
    {
        Camera cam = Instantiate(playerRigPrefab, transform).GetComponent<Camera>();
        if (Object.HasInputAuthority)
        {
            cam.depth = float.MaxValue;
        }

        if (!Object.HasStateAuthority)
        {
            transform.Find("PlayerRigFacade").gameObject.SetActive(false);
        }
    }
}
