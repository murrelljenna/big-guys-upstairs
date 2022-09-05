using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
using Photon.Realtime;
using UnityEditor;

public class selection : MonoBehaviour
{
    public Camera cam;
    [SerializeField] List<GameObject> selected = new List<GameObject>();
    private game.assets.Player player;

    /* Wall placement vars */

    private Vector3 firstPoint;
    private Vector3 lastPoint;
    private bool firstPointPlaced = false;
    private Vector3 destination;

    /* Terrain masks */

    private int terrainMask = 1 << 11;
    private int resourceMask = 1 << 9;
    private int attackableMask = (1 << 9) | (1 << 10) | (1 << 12) | (1 << 14) | (1 << 16) | (1 << 18);
    private int allMask;
    private int unitMask = 1 << 12;
    private int idleGroupingMask = 1 << 20;

    private LineRenderer lineRen;
    private List<Vector3> grid;

    private const float QUICK_SELECT_RANGE = 5f;
    private const float DOUBLE_CLICK_TIME = 0.2f;

    private float lastClickTime;

    private GameObject CommandMenu;
    private commandUIController controller;

    private GameObject viewedInfo;
    // Start is called before the first frame update
    void Start()
    {
        allMask = terrainMask | attackableMask;  // For terrain and attackable objects.
        player = this.transform.parent.parent.parent.parent.gameObject.GetComponent<game.assets.Player>();
        


        lineRen = this.GetComponent<LineRenderer>();
        this.gameObject.GetComponent<Renderer>().material.color = player.playerColor;
    }

    void OnEnable() {
        if (CommandMenu != null) {
            CommandMenu.SetActive(true);
        } else {
            CommandMenu = GameObject.Find("Command_UI");
            controller = CommandMenu.GetComponent<commandUIController>();
            CommandMenu.SetActive(false);
        }
    }

    void OnDisable() {
        CommandMenu.SetActive(false);
        clearSelection();
    }

    void Update()
    {
        RaycastHit hit;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitMask)) {
            // If looking at a tower.

            GuardTower tower = hit.collider.gameObject.GetComponent<GuardTower>();
            if (tower != null) {
                viewedInfo = tower.gameObject.transform.Find("Info").gameObject;
                viewedInfo.SetActive(true);

                tower.interactionOptions(player);
            } else {
                if (viewedInfo != null) {
                    viewedInfo.SetActive(false);
                }
            }

            if (Input.GetMouseButtonDown(0)) {
                firstPointPlaced = false;
                if (hit.collider.gameObject.GetComponent<ownership>().owner == player.playerID) {
                    float lastClick = Time.time - lastClickTime;
                    Militia militia = hit.collider.gameObject.GetComponent<Militia>();

                    if (lastClick <= DOUBLE_CLICK_TIME) {
                        if (militia != null && militia.assigned) {
                            selectFellowWorkers(militia);
                        } else {
                            selectUnitsInRadius(hit.point);
                        }
                    } else {
                        if (selected.Count > 0 && selected.Exists(unit => unit.GetInstanceID() == hit.collider.gameObject.GetInstanceID())) {
                            deselectUnit(hit.collider.gameObject);
                        } else {
                            selectUnit(hit.collider.gameObject);
                        }
                    }

                    lastClickTime = Time.time;
                }
            }
        } else {
            if (viewedInfo != null) {
                viewedInfo.SetActive(false);
            }
        }

        // When looking and an idleGrouping notification
        ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, idleGroupingMask) && Input.GetKeyDown(KeyCode.E)) {
            IdleGrouping idleGrouping = hit.collider.gameObject.GetComponent<IdleGrouping>();
            List<Unit> idleUnits = idleGrouping.getGrouping();
            idleGrouping.clickButton();
            for (int i = 0; i < idleUnits.Count; i++) {
                if (idleUnits[i].isSelected == false) {
                    selectUnit(idleUnits[i].gameObject);
                }
            }
        }

        // Order units to move to a position

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, allMask)) {
            if (Input.GetMouseButtonDown(1)) {
                destination = hit.point;

                if (attackableMask == (attackableMask | (1 << hit.collider.gameObject.layer)) 
                    && hit.collider.gameObject.GetComponent<ownership>().owned == true 
                    && hit.collider.gameObject.GetComponent<ownership>().owner != player.playerID) {

                    StartCoroutine(sendAttackOrders(hit.collider.gameObject.GetComponent<Attackable>().id));
                } else if (terrainMask == (terrainMask | (1 << hit.collider.gameObject.layer))) {
                    StartCoroutine(placeUnits(destination, new Queue<GameObject>(selected)));
                } else if (resourceMask == (resourceMask | (1 << hit.collider.gameObject.layer))) {
                    if (hit.collider.gameObject.GetComponent<ownership>().owner == this.player.playerID) {
                        StartCoroutine(assignUnits(hit.collider.gameObject.GetComponent<ResourceTile>(), selected));
                    }
                } else if (hit.collider.gameObject.GetComponent<Building>() != null && hit.collider.gameObject.GetComponent<ownership>().owner == this.player.playerID && hit.collider.gameObject.GetComponent<Building>()) {
                    StartCoroutine(assignUnitsToBuild(hit.collider.gameObject.GetComponent<Building>(), selected));
                }
            }
        }

        // Group select with left-click hold

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainMask)) {
            if (Input.GetMouseButtonDown(0)) {
                firstPoint = hit.point;
                firstPointPlaced = true;
                return;
            }

            if (!Input.GetMouseButton(0)) {
                lineRen.positionCount = 0;
                return;
            }

            lastPoint = hit.point;

            float topX = firstPoint.x > lastPoint.x ? firstPoint.x : lastPoint.x;
            float topZ = firstPoint.z > lastPoint.z ? firstPoint.z : lastPoint.z;
            float bottomX = firstPoint.x < lastPoint.x ? firstPoint.x : lastPoint.x;
            float bottomZ = firstPoint.z < lastPoint.z ? firstPoint.z : lastPoint.z;

            lineRen.positionCount = 5;

            Vector3[] lineRenPositions = new Vector3[5];
            lineRenPositions[0] = firstPoint;
            lineRenPositions[1] = new Vector3(firstPoint.x, firstPoint.y, lastPoint.z);
            lineRenPositions[2] = new Vector3(lastPoint.x, firstPoint.y, lastPoint.z);
            lineRenPositions[3] = new Vector3(lastPoint.x, firstPoint.y, firstPoint.z);
            lineRenPositions[4] = firstPoint;

            lineRen.SetPositions(lineRenPositions);

            Vector3 center = new Vector3(bottomX + (topX-bottomX)/2, 0, bottomZ + (topZ-bottomZ)/2);
            Vector3 halfExtent = new Vector3(topX - bottomX, 20, topZ-bottomZ);
            Vector3 something = new Vector3(cam.transform.position.x, 0, cam.transform.position.z);

            Rect rectangle = new Rect(topX, topZ, bottomX - topX, bottomZ - topZ);
            Collider[] selectedUnits = Physics.OverlapBox(center, halfExtent/2, Quaternion.identity, unitMask);

            foreach(Collider unit in selectedUnits) {
                if (unit.gameObject.GetComponent<ownership>().owner == player.playerID && unit.gameObject.GetComponent<Unit>().isSelected == false) {
                    selectUnit(unit.gameObject);
                }
            }

            firstPointPlaced = false;
        }
    }

    private IEnumerator assignUnitsToBuild(Building building, List<GameObject> units) {
        List<GameObject> assignedUnits = new List<GameObject>(units);

        foreach (GameObject unit in assignedUnits) {
            
            Militia worker = unit.GetComponent<Militia>();

            if (worker != null) {
                int runCount = 0;

                //bool added = resourceTile.addWorker(worker); // True if worker was successfully added (there was an available slot) false otherwise.
                worker.setBuildingTarget(building);
                deselectUnit(unit);

                runCount++;
                if (runCount % 2 == 0) {
                    yield return null;
                }
            }
        }
    }

    private IEnumerator assignUnits(ResourceTile resourceTile, List<GameObject> units) {
        List<GameObject> assignedUnits = new List<GameObject>(units);

        foreach (GameObject unit in assignedUnits) {
            
            Militia worker = unit.GetComponent<Militia>();

            if (worker != null) {
                int runCount = 0;

                bool added = resourceTile.addWorker(worker); // True if worker was successfully added (there was an available slot) false otherwise.
                if (added) {
                    deselectUnit(unit);
                }

                runCount++;
                if (runCount % 2 == 0) {
                    yield return null;
                }
            }
        }
    }


    private IEnumerator placeUnits(Vector3 center, Queue<GameObject> units, float unitSize = 0.2f, float gapSize = 0.2f) {
        Queue<Vector3> points = new Queue<Vector3>();
        Queue<Vector3> taken = new Queue<Vector3>();
        points.Enqueue(center);

        float branchSize = unitSize/2f + gapSize;
        Vector3[] positionMods = new Vector3[8] {
            new Vector3(0, 0, branchSize),
            new Vector3(branchSize, 0, branchSize),
            new Vector3(branchSize, 0, 0),
            new Vector3(branchSize, 0, -branchSize),
            new Vector3(0, 0, -branchSize),
            new Vector3(-branchSize, 0, -branchSize),
            new Vector3(-branchSize, 0, 0),
            new Vector3(-branchSize, 0, branchSize)
        };

        int runCount = 0;

        while (units.Count > 0) {
            GameObject unitObj = units.Dequeue();

            if (unitObj == null) {
                continue;
            }

            Unit unit = unitObj.GetComponent<Unit>();
            Vector3 destination = points.Dequeue();

            if (unit != null && unit.gameObject.GetComponent<NavMeshAgent>() != null) {
                unit.gameObject.GetComponent<Unit>().move(destination);
            }

            for (int i = 0; i < positionMods.Length; i++) {
                Vector3 modifiedPosition = destination + positionMods[i];
                float height = getTerrainHeight(modifiedPosition);
                if (!taken.Contains(modifiedPosition) && System.Math.Abs(height) - System.Math.Abs(center.y) < 1 && System.Math.Abs(height) - System.Math.Abs(center.y) > -1) {
                    points.Enqueue(modifiedPosition);
                    taken.Enqueue(modifiedPosition);
                }
            }

            runCount++;
            if (runCount % 2 == 0) {
                yield return null;
            }
        }
    }

    void selectUnit(GameObject unit) {
        selected.Add(unit.GetComponent<Collider>().gameObject);

        if (unit.GetComponent<LineRenderer>() != null) {
            controller.addCard(unit.GetComponent<Unit>());
            unit.GetComponent<LineRenderer>().enabled = true;
        }

        unit.GetComponent<Unit>().onSelect();
    }

    public void deselectUnit(GameObject unit) {
        if (selected.Contains(unit)) {
            controller.removeCard(unit.GetComponent<Unit>());
            selected.Remove(unit.GetComponent<Collider>().gameObject);
        }

        if (unit.GetComponent<Collider>().gameObject.GetComponent<LineRenderer>() != null) {
            unit.GetComponent<Collider>().gameObject.GetComponent<LineRenderer>().enabled = false;
        }

        unit.GetComponent<Unit>().onDeSelect();
    }
                                                        // 0.

   private float getTerrainHeight(Vector3 point) {
       RaycastHit hit;
       Physics.Raycast(new Vector3(point.x, 300, point.z), Vector3.down, out hit, Mathf.Infinity, terrainMask);
       return hit.point.y;
   }

    private void clearSelection() {
        selected.ForEach(unit => {
            if (unit != null && unit.GetComponent<LineRenderer>() != null) {
                controller.clearCards();
                unit.GetComponent<LineRenderer>().enabled = false;
                unit.GetComponent<Unit>().onDeSelect();
            }
        });
        selected.Clear();
    }

    private void selectFellowWorkers(Militia militia) {
        foreach (Militia worker in militia.getFellowWorkers()) {
            if (!selected.Contains(worker.gameObject)) {
                selectUnit(worker.gameObject);
            }
        }
    }

    private void selectUnitsInRadius(Vector3 point) {
        Collider[] hitColliders = Physics.OverlapSphere(point, QUICK_SELECT_RANGE, unitMask);
        for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders[i].GetComponent<ownership>() != null &&
                hitColliders[i].GetComponent<ownership>().owned == true && 
                hitColliders[i].GetComponent<ownership>().owner == player.playerID &&
                hitColliders[i].GetComponent<Unit>().isSelected == false) { 
                Militia militia = hitColliders[i].GetComponent<Militia>();
                if (militia != null && militia.assigned) {
                } else {
                    selectUnit(hitColliders[i].gameObject);
                }
            }
        }
    }

    private IEnumerator sendAttackOrders(int id) {
        for (int i = 0; i < selected.Count; i++) {
            GameObject unit = selected[i];
            if (unit != null) {
                unit.gameObject.GetComponent<Unit>().cancelOrders();
                unit.gameObject.GetComponent<Unit>().callAttack(id);
            }

            if (i % 10 == 0) {
                yield return null;
            }
        }
    }
}
