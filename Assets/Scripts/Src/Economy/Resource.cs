using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.utilities.resources;
using game.assets.utilities;
using UnityEngine.Events;

namespace game.assets.economy {
    [RequireComponent(typeof(GameObjectSearcher))]
    public class Resource : MonoBehaviour
    {
        [Tooltip("Maximum workers assignable to resource")]
        public int maxWorkers = 3;

        [Tooltip("Yield for workers when extracting")]
        public ResourceSet yield;

        public List<Worker> workers = new List<Worker>();

        private Depositor upstream;

        private GameObjectSearcher searcher;

        public UnityEvent<int> workerCountChanged = new UnityEvent<int>();

        private void Start()
        {
            searcher = GetComponent<GameObjectSearcher>();
        }

        public bool addWorker(Worker worker)
        {
            upstream = closestDepositor();
            if (workers.Count < maxWorkers)
            {
                worker.clearAssignment();
                workers.Add(worker);
                worker.assignResourceTile(this);
                worker.startCollectingResources(getNode(), yield);
                workerCountChanged.Invoke(workers.Count);
                return true;
            }
            return false;
        }

        public void removeWorker(Worker worker)
        {
            workers.Remove(worker);
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
                if (depositor.IsMine())
                {
                    float dist = Vector3.Distance(depositor.transform.position, currentPos);
                    if (dist < minDist)
                    {
                        tMin = depositor.transform;
                        minDist = dist;
                    }
                }
            }
            return tMin.GetComponent<Depositor>();
        }
    }
}
