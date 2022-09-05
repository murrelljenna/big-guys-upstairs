using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using game.assets.utilities.resources;

public enum UnitState {
    Idle,
    Building,
    Collecting,
    Fighting
}

public class Militia : Unit
{
    private ResourceTile resourceTile;
    public ResourceSet inventory = new ResourceSet();
    private int maxInventory = 5;

    // Resource collection stuff
    private GameObject node;
    private ResourceSet yield;
    public bool assigned;
    private float collectRate = 2.5f;

    private bool collectingResources;

    public UnitState unitState;
    private Building building;

    private int buildPer = 20;
    private float buildRate = 1.5f;

    // Start is called before the first frame update
    void Start() {
        this.prefabName = "Militia";
        this.movable = true;
        this.responseRange = 3f;
        this.woodCost = 1;
        this.foodCost = 5;

        this.atk = 1;
        this.hp = 5;
        this.lastHP = this.hp;
        this.rng = 0.3f;
        this.attackRate = 1.2f;

        base.Start();
    }

    public void startCollectingResources(GameObject node, ResourceSet yield)  {
        collectingResources = true;
        this.node = node;
        this.yield = yield;
        StartCoroutine(collectResources(node, yield));
    }

    private IEnumerator collectResources(GameObject node, ResourceSet yield) {
        setDestination(node.transform.position);
        yield return new WaitUntil (() => isInRange(node));
        stopMovement();

        InvokeRepeating("getResource", 0f, collectRate);
    }

    public virtual void getResource() {
        resetIdle();

        this.faceTarget(node.transform);
        if (animator != null) {
            animator.SetTrigger("attack");
        }

        this.inventory = this.inventory + yield;

        if (this.inventory.anyValOver(maxInventory)) {
            CancelInvoke("getResource");
            StartCoroutine(returnToDeposit());
        }
    }

    private IEnumerator returnToDeposit() {

        Vector3 destination = resourceTile.upstream.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position);
        Depositor depot = resourceTile.upstream.GetComponent<Depositor>();        

        setDestination(destination);
        yield return new WaitUntil (() => isInRange(destination, this.collectRange));
        depot.deposit(inventory);
        inventory.setEmpty();
        collectingResources = false;
    }

    public void assignResourceTile(ResourceTile resourceTile) {        this.assigned = true;
        this.resourceTile = resourceTile;
    }

    void Update() {
        if (assigned && !collectingResources) {
            startCollectingResources(node, yield);
        } 

        base.Update();
    }

    public override void onSelect() {
        AudioSource[] sources = this.transform.Find("SelectionSounds").GetComponents<AudioSource>();
        AudioSource source = sources[UnityEngine.Random.Range(0, sources.Length)];
        AudioSource.PlayClipAtPoint(source.clip, this.transform.position);

        base.onSelect();
    }

    public override void move(Vector3 destination) {
        if (assigned) {
            clearAssignment();
        }
        base.move(destination);
    }

    public void clearAssignment() {
        CancelInvoke("getResource");
        CancelInvoke("build");
        if (resourceTile != null) {
            this.resourceTile.removeWorker(this);
        }
        this.resourceTile = null;
        this.assigned = false;
        this.collectingResources = false;
    }

    public List<Militia> getFellowWorkers() {
        return this.resourceTile.workers;
    }

    public void setBuildingTarget(Building building) {
        StartCoroutine(orderToBuild(building));
    }

    public IEnumerator orderToBuild(Building building) {
        Vector3 destination = building.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position);
        this.building = building;
        move(destination);
        yield return new WaitUntil (() => isInRange(destination, this.collectRange));
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
        }
    }
}
