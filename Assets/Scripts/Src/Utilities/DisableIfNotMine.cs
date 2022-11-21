using Fusion;
using game.assets.player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableIfNotMine : NetworkBehaviour
{
    public GameObject gameObjectToDisable;

    public override void Spawned()
    {
        if (!Object.HasInputAuthority)
        {
            gameObjectToDisable.SetActive(false);
        }
    }
}
