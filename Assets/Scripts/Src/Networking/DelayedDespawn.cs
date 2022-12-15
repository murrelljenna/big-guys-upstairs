using Fusion;
using game.assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDespawn : NetworkBehaviour
{
    public List<GameObject> gos = new List<GameObject>();

    public static DelayedDespawn AsDevCube(List<GameObject> gos)
    {
        GameObject devCube = new GameObject();
        DelayedDespawn despawner = devCube.AddComponent<DelayedDespawn>();
        despawner.gos = gos;
        return despawner;
    }

    public void DestroyGos()
    {
        Invoke("DestroyAllGos", 0.2f);
    }

    private void DestroyAllGos()
    {
        gos.ForEach(neighbour =>
        {
            NetworkedGameManager.Get().Despawn(neighbour.GetComponent<NetworkObject>()); // Can I do this? We'll find out
        });
    }
}
