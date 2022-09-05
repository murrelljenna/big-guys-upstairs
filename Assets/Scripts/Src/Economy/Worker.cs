using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.utilities.resources;
using game.assets.ai;
using game.assets.utilities;
using UnityEngine.Events;

namespace game.assets.economy {
    [RequireComponent(typeof(Movement))]
    public class Worker : MonoBehaviour
    {
        [Tooltip("Invoked each time worker grabs resources from a resource node")]
        public UnityEvent getFromResource;

        [Tooltip("Invoked each time worker drops off resources at a Town")]
        public UnityEvent dropOffResource;

        public int maxInventory = 5;
        public ResourceSet inventory = new ResourceSet();

        Movement movement;

        private Health building;

        /* Getting resources */
        private Resource resource;
        private GameObject node;
        private ResourceSet yield;
        private bool assigned = false;

        private const float COLLECT_RANGE = 0.3f;
        private const float COLLECT_RATE = 2.5f;

        private bool collectingResources;

        private void Start() {
            movement = GetComponent<Movement>();
        }

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
            movement.goTo(node.transform.position);
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

            Vector3 destination = resource.upstream.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position);
            Depositor depot = resource.upstream.GetComponent<Depositor>();        

            movement.goTo(destination);
            yield return new WaitUntil (() => gameObject.isInRangeOf(destination, COLLECT_RANGE));
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

/*        public void setBuildingTarget(Building building) {
            StartCoroutine(orderToBuild(building));
        }

        public IEnumerator orderToBuild(Building building) {
            Vector3 destination = building.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position);
            this.building = building;
            move(destination);
            yield return new WaitUntil (() => isInRange(destination, this.COLLECT_RANGE));
            InvokeRepeating("build", 0f, buildRate);
        }

        private void build() {
            if (building.underConstruction) {
                building.build(buildPer);
                if (animator != null) {
                    animator.SetTrigger("attack");
                }
            } else {
                CancelInvoke("build");

                GameObject building = game.assets.utilities.ObjFinder.findNearbyConstruction(this.transform.position, this.owner);
                if (building != null)
                {
                    print(building.name);
                    print(building.GetComponent<Building>().underConstruction);
                    setBuildingTarget(building.GetComponent<Building>());
                }
            }
        }*/
    }
}