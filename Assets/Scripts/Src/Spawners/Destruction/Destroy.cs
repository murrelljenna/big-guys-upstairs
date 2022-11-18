﻿using Fusion;
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
        Invoke("addToDeletePool", 1f);
        this.gameObject.SetActive(false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SpawnLocalGameObject(Vector3 pos, Quaternion rot)
    {
        Debug.Log("AA - RPC");
        if (leaveBehind != null)
        {
            Debug.Log("AA - Instantiating?");
            var go = GameObject.Instantiate(leaveBehind, pos, rot);
            go.GetComponent<UnitColourController>().SetColourToPlayer(player);
            go.GetComponent<Destroy>().destroyAfterAMinute(); // Clean yoself up
        }
    }

    public void destroyAfterAMinute()
    {
        Invoke("destroy", 60f);
    }

    public void destroyAfterTenSeconds()
    {
        Invoke("destroy", 10f);
    }

    private void addToDeletePool()
    {
        if (Object.HasStateAuthority)
        {
            Runner.Despawn(GetComponent<NetworkObject>());
        }
    }
}
