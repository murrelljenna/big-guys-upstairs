using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.utilities.resources;
using game.assets.ai;
using game.assets.utilities;
using UnityEngine.Events;
using static game.assets.utilities.GameUtils;

namespace game.assets.economy {
    [RequireComponent(typeof(Movement))]
    public class Worker : MonoBehaviour
    {
        [Tooltip("Invoked each time worker grabs resources from a resource node")]
        public UnityEvent getFromResource;

        [Tooltip("Invoked each time worker drops off resources at a Town")]
        public UnityEvent dropOffResource;
        [Tooltip("Invoked each time worker swings their hammer to construct something")]
        public UnityEvent buildTick;

        public int maxInventory = 5;
        public ResourceSet inventory = new ResourceSet();

        Movement movement;

        private Construction construction;

        /* Getting resources */
        public Resource resource;
        private GameObject node;
        private ResourceSet yield;
        private bool assigned = false;

        private const float COLLECT_RANGE = 0.3f;
        private const float DEPOSIT_RANGE = 0.6f;
        private const float COLLECT_RATE = 2.5f;
        private const float BUILD_RATE = 2.5f;
        private const int BUILD_AMT = 2;

        private bool collectingResources = false;
        public bool currentlyBuilding = false;

        private void Start() {
            movement = GetComponent<Movement>();
            movement.newMoveOrdered.AddListener(cancelOrders);

            //InvokeRepeating("buildNearestBuilding", 2f, 2f); 
        }

        public bool isCollectingResources() { return collectingResources; }
        public bool isCurrentlyBuilding() { return currentlyBuilding; }

        public void startCollectingResources(GameObject node, ResourceSet yield)  {
            collectingResources = true;
            this.node = node;
            this.yield = yield;
            if (inventory.anyValOver(maxInventory))
            {
                StartCoroutine(returnToDeposit());
            }
            else
            {
                StartCoroutine(collectResources(node, yield));
            }
        }

        private IEnumerator collectResources(GameObject node, ResourceSet yield) {
            movement.goToSilently(node.transform.position);
            yield return new WaitUntil (() => gameObject.isInRangeOf(node, COLLECT_RANGE));
            movement.stop();

            InvokeRepeating("getResource", 0f, COLLECT_RATE);
        }

        public void assignResourceTile(Resource resource)
        {
            this.assigned = true;
            this.resource = resource;
        }

        private void getResource() {
            movement.faceTowards(node.transform.position);
            getFromResource.Invoke();

            this.inventory = this.inventory + yield;

            if (this.inventory.anyValOver(maxInventory)) {
                CancelInvoke("getResource");
                StartCoroutine(returnToDeposit());
            }
        }

        private IEnumerator returnToDeposit() {

            Vector3 destination = resource.getUpstream().GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position);
            Depositor depot = resource.getUpstream().GetComponent<Depositor>();        

            movement.goToSilently(destination);

            yield return new WaitUntil (() => gameObject.isInRangeOf(destination, DEPOSIT_RANGE));

            dropOffResource.Invoke();
            depot.deposit(inventory);
            inventory.setEmpty();
            startCollectingResources(node, resource.yield);
        }

        public void assignResource(Resource resource) {        
            this.resource = resource;
        }

        public void clearAssignment() {
            CancelInvoke("getResource");
            
            if (resource != null) {
                resource.removeWorker(this);
            }
            this.resource = null;
            this.assigned = false;
            this.collectingResources = false;
        }

        public List<Worker> getFellowWorkers() {
            return this.resource.workers;
        }

        public void setBuildingTarget(Construction construction) {
            cancelOrders();
            currentlyBuilding = true;
            StartCoroutine(orderToBuild(construction));
        }

        public IEnumerator orderToBuild(Construction construction) {
            Vector3 destination = construction.GetComponent<Collider>().ClosestPointOnBounds(gameObject.transform.position);
            this.construction = construction;
            movement.goToSilently(destination);
            yield return new WaitUntil (() => gameObject.isInRangeOf(destination, COLLECT_RANGE));
            movement.stop();
            InvokeRepeating("build", 0f, BUILD_RATE);
        }

        private void build() {
            if (construction != null)
            {
                construction.build(BUILD_AMT);
                buildTick.Invoke();
            }
            else
            {
                clearBuilding();
                buildNearestBuilding();
            }
        }

        public void cancelOrders()
        {
            clearBuilding();
            clearAssignment();
        }

        private void clearBuilding()
        {
            CancelInvoke("build");
            currentlyBuilding = false;
        }

        private void buildNearestBuilding()
        {
            if (resource != null || currentlyBuilding)
            {
                return;
            }
            GameObject[] nearby = findGameObjectsInRange(transform.position, 10f);
            Construction[] thingsICanBuild = nearby.GetComponents<Construction>();
            for (int i = 0; i < thingsICanBuild.Length; i++)
            {
                if (thingsICanBuild[i].IsEnemyOf(this))
                {
                    continue;
                }

                setBuildingTarget(thingsICanBuild[i]);
            }

            return;
        }
    }
}