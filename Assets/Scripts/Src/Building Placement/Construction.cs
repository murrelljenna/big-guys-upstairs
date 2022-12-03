using Fusion;
using game.assets;
using game.assets.ai;
using game.assets.economy;
using game.assets.player;
using game.assets.utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Ownership))]
public class Construction : NetworkBehaviour
{
    [Tooltip("Prefab created when construction finished")]
    public NetworkPrefabRef onceBuilt;

    [Tooltip("Invoked when construction finished")]
    public UnityEvent built;

    private Ownership ownership;

    private Health health;
    public override void Spawned()
    {
        health = GetComponent<Health>();
        ownership = GetComponent<Ownership>();

        if (!Object.HasStateAuthority)
        {
            return;
        }

        health.onMaxHP.AddListener(finish);

        // Fetch all nearby workers and have them build me if they're not doing anything important

        GameObject[] nearby = GameUtils.findGameObjectsInRange(transform.position, 10f);
        var workers = nearby.GetComponents<Worker>();

        for (int i = 0; i < workers.Length; i++)
        {
            var worker = workers[i];
            if (worker.currentlyBuilding || worker.resource != null || !worker.BelongsTo(ownership.owner))
            {
                continue;
            }

            worker.setBuildingTarget(this);
        }

        RPC_SetToFirstModel();
    }

    public void build(int amt)
    {
        health.raiseHP(amt);

        if (health.HP > (health.maxHP / 2))
        {
            if (Object.HasStateAuthority)
                RPC_SetToSecondModel();
        }
    }

    private void finish() {
        Runner.Spawn(onceBuilt, transform.position, transform.rotation, Object.InputAuthority, (runner, o) => o.SetAsPlayer(GetComponent<Ownership>().owner));
        built.Invoke();
        Destroy(this);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_SetToFirstModel()
    {
        setToFirstModel();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_SetToSecondModel()
    {
        setToSecondModel();
    }

    private void setToFirstModel()
    {
        transform.Find("Construction_0").gameObject.SetActive(true);
        transform.Find("Construction_1").gameObject.SetActive(false);
    }

    private void setToSecondModel()
    {
        transform.Find("Construction_0").gameObject.SetActive(false);
        transform.Find("Construction_1").gameObject.SetActive(true);
    }
}
