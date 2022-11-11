using Fusion;
using game.assets;
using game.assets.player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : NetworkBehaviour
{
    public NetworkPrefabRef leaveBehind;
    public void destroy()
    {
        if (!Object.HasStateAuthority) return;

        if (leaveBehind != null && leaveBehind.IsValid)
        {
            var go = Runner.Spawn(leaveBehind, transform.position, transform.rotation);
            go.GetComponent<Destroy>().destroyAfterAMinute(); // Clean yoself up
        }
        Runner.Despawn(GetComponent<NetworkObject>());
    }

    public void destroyAfterAMinute()
    {
        Invoke("destroy", 60f);
    }

    public void destroyAfterTenSeconds()
    {
        Invoke("destroy", 10f);
    }
}
