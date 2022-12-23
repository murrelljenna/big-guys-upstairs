using Fusion;
using game.assets;
using game.assets.player;
using game.assets.ui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ownership))]
public class Destroy : NetworkBehaviour
{
    public GameObject leaveBehind;

    Player player;

    public void destroy()
    {
        if (Object == null)
        {
            return;
        }
        player = GetComponent<Ownership>()?.owner;
        if (!Object.HasStateAuthority) return;
        RPC_SpawnLocalGameObject(transform.position, transform.rotation);
        this.transform.position = new Vector3(420, -1337, 6969);
        Invoke("addToDeletePool", 0.5f);
        this.gameObject.SetActive(false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SpawnLocalGameObject(Vector3 pos, Quaternion rot)
    {
        if (leaveBehind != null)
        {
            var go = GameObject.Instantiate(leaveBehind, pos, rot);
            go?.GetComponent<UnitColourController>()?.SetColourToPlayer(player); // Hack so that unit deaths are all coloured correctly
            go.GetComponent<Destroy>().cleanupAfter30Seconds(); // Clean yoself up
        }
    }

    public void cleanupLocally()
    {
        Destroy(this.gameObject);
    }

    public void destroyAfterAMinute()
    {
        Invoke("destroy", 60f);
    }

    public void destroyAfterTenSeconds()
    {
        Invoke("destroy", 10f);
    }

    public void cleanupAfter30Seconds()
    {
        Invoke("cleanupLocally", 30f);
    }

    private void addToDeletePool()
    {
        if (Object != null && Object.HasStateAuthority)
        {
            Instantiation.Despawn(Runner, GetComponent<NetworkObject>());
        }
    }
}
