using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.utilities.resources;
using game.assets.utilities;
using UnityEngine.Events;
using game.assets.ai;
using game.assets.player;
using Fusion;
using static IsAUtils;

namespace game.assets.economy {
    [RequireComponent(typeof(GameObjectSearcher))]
    [RequireComponent(typeof(Ownership))]
    public class Resource : NetworkBehaviour
    {
        [Tooltip("Maximum workers assignable to resource")]
        public int maxWorkers = 3;

        [Tooltip("Yield for workers when extracting")]
        public ResourceSet yield;

        public Ownership ownership;

        public List<Worker> workers = new List<Worker>();

        private Depositor upstream;

        private GameObjectSearcher searcher;

        public UnityEvent<int> workerCountChanged = new UnityEvent<int>();

        private void Start()
        {
            searcher = GetComponent<GameObjectSearcher>();
            ownership = GetComponent<Ownership>();

            Instantiation.ObjectSpawned.AddListener((NetworkObject netObj) => netObj.gameObject.IfIsA(
                IsATown,
                (GameObject go) =>
                {
                    if (go.GetComponent<Ownership>().owner == ownership.owner)
                    {
                        upstream = closestDepositor();
                    }

                    return go;
                })
            );
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_FireWorkerEvents(int count)
        {
            workerCountChanged.Invoke(count);
        }

        public bool addWorker(Worker worker)
        {
            if (!Object.HasStateAuthority)
            {
                return false;
            }

            if (!ownership.isOwnedByOrNeutral(worker.GetComponent<Ownership>().owner)) {
                return false;
            }

            setOwner(worker);

            upstream = closestDepositor();

            if (workers.Count < maxWorkers)
            {
                worker.clearAssignment();
                workers.Add(worker);
                worker.assignResourceTile(this);
                worker.startCollectingResources(getNode(), yield);
                RPC_FireWorkerEvents(workers.Count);

                upstream?.GetComponent<Health>()?.onZeroHP.AddListener((Health _) => worker.cancelOrders());
                return true;
            }
            return false;
        }

        private void setOwner(Worker worker)
        {
            var workerOwnership = worker.GetComponent<Ownership>();

            ownership.setOwner(workerOwnership.owner);
            
        }

        public void removeWorker(Worker worker)
        {
            workers.Remove(worker);
            RPC_FireWorkerEvents(workers.Count);

            if (workers.Count == 0)
            {
                ownership.clearOwner();
            }
        }

        public virtual GameObject getNode()
        {
            int index = UnityEngine.Random.Range(0, searcher.actors.Count);
            return searcher.actors[index];
        }

        public Depositor getUpstream()
        {
            if (upstream == null) {
                upstream = closestDepositor();
            }

            return upstream;
        }

        private Depositor closestDepositor()
        {

            Depositor[] depositors = GameObject.FindObjectsOfType<Depositor>();
            Transform tMin = null;
            float minDist = Mathf.Infinity;
            Vector3 currentPos = transform.position;
            foreach (Depositor depositor in depositors)
            {
                if (depositor.BelongsTo(ownership.owner))
                {
                    float dist = Vector3.Distance(depositor.transform.position, currentPos);
                    if (dist < minDist)
                    {
                        tMin = depositor.transform;
                        minDist = dist;
                    }
                }
            }
            Debug.Log("WRK - " + tMin.gameObject.name + " is this resource's closest place to deposit resources");
            return tMin?.GetComponent<Depositor>();
        }
    }
}
