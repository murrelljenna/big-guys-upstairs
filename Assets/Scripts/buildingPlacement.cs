using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class buildingPlacement : MonoBehaviourPunCallbacks
{
    private LineRenderer lineRen;
    game.assets.Player wallet;
    public Camera cam;
    private Transform currentBuilding;
    private int layerMask;

    private bool firstPointSnapped = false;
    private bool lastPointSnapped = false;

    private TooltipController tooltips;
    private Color previousColor;

    Vector3 firstPoint;
    Vector3 lastPoint;
    bool firstPointPlaced = false;

    public AudioClip[] plopSounds;
    
    void Start()
    {
        wallet = this.transform.parent.parent.parent.parent.GetComponent<game.assets.Player>();
        currentBuilding = null;
        tooltips = GameObject.Find("Tooltips").GetComponent<TooltipController>();
        layerMask = 1 << 11;

        lineRen = this.GetComponent<LineRenderer>();
        this.gameObject.GetComponent<Renderer>().material.color = wallet.playerColor;
    }

    void OnDisable() {
        if (currentBuilding != null) {
            Destroy(currentBuilding.gameObject);
            currentBuilding = null;
        }

        firstPointPlaced = false;
        lastPointSnapped = false;
        firstPointSnapped = false;
        lineRen.positionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBuilding != null) {
            RaycastHit hit;

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                currentBuilding.gameObject.SetActive(true);
                if (currentBuilding.name == "wall") {
                    if (Input.GetMouseButtonDown(1)) {
                        firstPointPlaced = false;
                        lastPointSnapped = false;
                        firstPointSnapped = false;
                        lineRen.positionCount = 0;
                    }

                    if (wallInRange(hit.point, 0.5f)) {
                        hit.point = snapWallInRange(hit.point, 0.5f);
                        currentBuilding.gameObject.SetActive(false);

                        if (firstPointPlaced) {
                            lastPointSnapped = true;
                        } else {
                            firstPointSnapped = true;
                        }
                    } else {
                        if (firstPointPlaced) {
                            lastPointSnapped = false;
                        } else {
                            firstPointSnapped = false;
                        }
                    }

                    currentBuilding.position = hit.point;

                    if (firstPointPlaced) {
                        Vector3[] positions = new Vector3[2];
                        lineRen.positionCount = 2;
                        positions[0] = firstPoint;
                        positions[1] = hit.point;
                        lineRen.SetPositions(positions);

                        currentBuilding.rotation = Quaternion.LookRotation(hit.point - firstPoint);
                    }
                } else {
                    currentBuilding.position = new Vector3(hit.point.x, hit.point.y+0.5f, hit.point.z);           
                }

                if (Input.GetMouseButtonDown(0)) {

                    int wood = currentBuilding.GetComponent<Attackable>().woodCost;
                    int food = currentBuilding.GetComponent<Attackable>().foodCost;

                    if (wallet.canAfford(wood, food)) { // If can afford and no town radius overlapping
                        if (!currentBuilding.GetComponent<buildingGhost>().colliding) {
                            if (currentBuilding.name == "town") {
                                /* Unecessary with new construction updates.
                                if (!unitInRange(hit.point, 10f, wallet.playerID)) {
                                    StopAllCoroutines();
                                    tooltips.flashFriendUnitsNearby();
                                    StartCoroutine(flashRed(currentBuilding.gameObject, 0.2f));
                                    return;
                                }

                                if (enemyUnitInRange(hit.point, 10f, wallet.playerID)) {
                                    StopAllCoroutines();
                                    tooltips.flashEnemyUnitsNearby();
                                    StartCoroutine(flashRed(currentBuilding.gameObject, 0.2f));
                                    return;
                                }*/

                                if (!townInRange(hit.point, 20f)) {
                                    StartCoroutine(plopBuilding(currentBuilding.gameObject, hit.point));
                                    currentBuilding = null;
                                } else { // if town is TOO close
                                    StopAllCoroutines();
                                    tooltips.flashCityRadius();
                                    StartCoroutine(flashRed(currentBuilding.gameObject, 0.2f));
                                }
                            } else if (currentBuilding.name == "wall") {
                                if (townInRange(hit.point, 10f, wallet.playerID)) {
                                    if (firstPointPlaced) {
                                        lastPoint = hit.point;
                                        float wallUnitLength = currentBuilding.transform.Find("Model").GetComponent<MeshFilter>().mesh.bounds.size.z/3f * currentBuilding.transform.Find("Model").localScale.z;
                                        float pointDistance = Vector3.Distance(firstPoint, lastPoint);

                                        float noWalls = pointDistance/wallUnitLength;
                                        int mapLayer = ~(1 <<11);
                                        if (wallet.canAfford(wood * (int)noWalls, food * (int)noWalls)) {
                                            RaycastHit info;
                                            if ((!Physics.Linecast(firstPoint, lastPoint, out info, mapLayer) || info.collider.gameObject.name == "wall" || info.collider.gameObject.GetComponent<Attackable>().prefabName == "Wall_Corner")) {
                                                if (System.Math.Abs(firstPoint.y) - System.Math.Abs(lastPoint.y) < 0.5f && System.Math.Abs(firstPoint.y) - System.Math.Abs(lastPoint.y) > -0.5f) {
                                                    wallet.makeTransaction(wood * (int)noWalls, food * (int)noWalls);
                                                    StartCoroutine(placeWalls(noWalls, firstPointSnapped, lastPointSnapped));

                                                    firstPointPlaced = false;
                                                    lastPointSnapped = false;
                                                    firstPointSnapped = false;
                                                } else {
                                                    tooltips.flashBuildingBlocked();
                                                }
                                            } else {
                                                StartCoroutine(flashRed(currentBuilding.gameObject, 0.2f));
                                                StartCoroutine(flashRed(info.collider.gameObject, 0.2f));
                                                tooltips.flashBuildingBlocked();
                                            }
                                        } else {
                                            StopAllCoroutines();
                                            tooltips.flashLackResources();
                                            StartCoroutine(flashRed(currentBuilding.gameObject, 0.2f));
                                        }
                                    } else {
                                        firstPoint = hit.point;
                                        firstPointPlaced = true;
                                    }
                                } else {
                                    StopAllCoroutines();
                                    tooltips.flashInsideTown();
                                    StartCoroutine(flashRed(currentBuilding.gameObject, 0.2f));
                                }
                            } else {
                                if (townInRange(hit.point, 10f, wallet.playerID)) {

                                    if (getTownInRange(hit.point, 10f, wallet.playerID).maxedBuildings()) {
                                        tooltips.flashBuildingMax();
                                        return;
                                    }
                                    StartCoroutine(plopBuilding(currentBuilding.gameObject, hit.point));
                                    Town town = getTownInRange(hit.point, 10f, wallet.playerID);
                                    town.addBuilding();

                                    currentBuilding = null;
                                } else { // If town is NOT in range
                                    StopAllCoroutines();
                                    tooltips.flashInsideTown();
                                    StartCoroutine(flashRed(currentBuilding.gameObject, 0.2f));
                                }
                            }
                        } else {
                            tooltips.flashBuildingBlocked();
                        }
                    } else { // If cannot afford.
                        StopAllCoroutines();
                        tooltips.flashLackResources();
                        StartCoroutine(flashRed(currentBuilding.gameObject, 0.2f));
                    }
                }
            }
        }
    }

    public void setBuilding(GameObject building) {
        if (currentBuilding != null) {
            Destroy(currentBuilding.gameObject);
            currentBuilding = null;
        }
        
        currentBuilding = ((GameObject)Instantiate(building)).transform;
        currentBuilding.transform.Find("Model").gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Buildings_" + wallet.colorName) as Texture));
        currentBuilding.GetComponent<Attackable>().canAttack = false;
        currentBuilding.gameObject.name = currentBuilding.tag;
        currentBuilding.tag = "buildingGhost";

        Renderer renderer = currentBuilding.transform.Find("Model").gameObject.GetComponent<MeshRenderer>();

        previousColor = renderer.material.color;
    }

    private bool townInRange(Vector3 location, float range) {       
        Collider[] hitColliders = Physics.OverlapSphere(location, range);
        for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders[i].tag == "town") {
                return true;
            }
        }

        return false;
    }

    private bool townInRange(Vector3 location, float range, int ownerID) {
        Collider[] hitColliders = Physics.OverlapSphere(location, range);
        for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders[i].tag == "town" && hitColliders[i].gameObject.GetComponent<ownership>().owner == ownerID) { // If there is a town in range that belongs to the player.
                return true;
            }
        }

        return false;
    }

    private Town getTownInRange(Vector3 location, float range, int ownerID) {
        Collider[] hitColliders = Physics.OverlapSphere(location, range);
        for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders[i].tag == "town" && hitColliders[i].gameObject.GetComponent<ownership>().owner == ownerID) { // If there is a town in range that belongs to the player.
                return hitColliders[i].gameObject.GetComponent<Town>();
            }
        }

        return null;
    }

    private bool unitInRange(Vector3 location, float range, int ownerID) {
        Collider[] hitColliders = Physics.OverlapSphere(location, range);
        for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders[i].tag == "unit" && hitColliders[i].gameObject.GetComponent<ownership>().owner == ownerID) { // If there is a town in range that belongs to the player.
                return true;
            }
        }

        return false;
    }

    private bool enemyUnitInRange(Vector3 location, float range, int ownerID) {
        Collider[] hitColliders = Physics.OverlapSphere(location, range);
        for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders[i].tag == "unit" && hitColliders[i].gameObject.GetComponent<ownership>().owner != ownerID) { // If there is a town in range that belongs to the player.
                return true;
            }
        }

        return false;
    }

    private bool wallInRange(Vector3 location, float range) {
        Collider[] hitColliders = Physics.OverlapSphere(location, range);
        for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders[i].gameObject.GetComponent<Attackable>() != null && hitColliders[i].gameObject.GetComponent<Attackable>().prefabName == "Wall_Corner") {
                return true;
            }
        }

        return false;
    }

    private Vector3 snapWallInRange(Vector3 location, float range) {
        Collider[] hitColliders = Physics.OverlapSphere(location, range);
        for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders[i].gameObject.GetComponent<Attackable>() != null && hitColliders[i].gameObject.GetComponent<Attackable>().prefabName == "Wall_Corner") {
                return hitColliders[i].transform.position;
            }
        }

        return new Vector3(0, 0, 0);
    }

    private IEnumerator flashRed(GameObject building, float offset = 0.2f) {
        Renderer renderer = building.transform.Find("Model").gameObject.GetComponent<MeshRenderer>();

        renderer.material.color = Color.red;

        yield return new WaitForSeconds(offset);

        renderer.material.color = previousColor; // Set when building gets selected in setBuilding.
    }

    private void clearColour(GameObject building) {
        if (previousColor != null) {
            Renderer renderer = building.transform.Find("Model").gameObject.GetComponent<MeshRenderer>();
            renderer.material.color = previousColor;
        }
    }

    private IEnumerator plopBuilding(GameObject building, Vector3 destination) {
        float startTime = Time.time;

        while (Vector3.Distance(building.transform.position, destination) > 0.01f) {
            float distCovered = (Time.time - startTime) * 0.5f;
            float fractionOfJourney = distCovered / Vector3.Distance(building.transform.position, destination);

            building.transform.position = Vector3.Lerp(building.transform.position, destination, fractionOfJourney);
            yield return null;
        }

        int wood = building.GetComponent<Attackable>().woodCost;
        int food = building.GetComponent<Attackable>().foodCost;

        wallet.makeTransaction(wood, food);
        GameObject placedBuilding = PhotonNetwork.Instantiate(building.GetComponent<Attackable>().prefabName, destination, Quaternion.identity, 0);
        placedBuilding.GetComponent<ownership>().capture(wallet);
        placedBuilding.GetComponent<buildingGhost>().active = false; // Disabling script makes collider callbacks error.

        if (building.GetComponent<Attackable>().prefabName == "GuardTower" && placedBuilding.GetComponent<LineRenderer>() != null) {
            placedBuilding.GetComponent<LineRenderer>().enabled = false;
        }

        placedBuilding.transform.Find("Dust").gameObject.GetComponent<ParticleSystem>().Emit(30);
        placedBuilding.GetComponent<Building>().setToConstruction();
        Debug.Log("Set to construction!");
        
        AudioSource.PlayClipAtPoint(plopSounds[Random.Range(0, plopSounds.Length - 1)], destination);

        Destroy(building);
    }

    private IEnumerator placeWalls(float noWalls, bool firstPointSnapped, bool lastPointSnapped) {
        for (int i = 0; i <= (int)noWalls; i++) {
            Vector3 destination = Vector3.Lerp(firstPoint, lastPoint, (float)i * (1f/noWalls));
            GameObject placedBuilding = null;
            if ((i == 0 && !firstPointSnapped) || (i == (int)noWalls && !lastPointSnapped)) {
                placedBuilding = PhotonNetwork.Instantiate("Wall_Corner", destination, Quaternion.LookRotation(lastPoint - destination), 0);
                placedBuilding.GetComponent<Wall>().prefabName = "Wall_Corner";
            } else {
                placedBuilding = PhotonNetwork.Instantiate("Wall", destination, Quaternion.LookRotation(lastPoint - destination), 0);
            }

            if (placedBuilding != null) {
                placedBuilding.transform.Find("Dust").gameObject.GetComponent<ParticleSystem>().Emit(30);
                placedBuilding.GetComponent<buildingGhost>().active = false;
                placedBuilding.GetComponent<ownership>().capture(wallet);
            
                AudioSource.PlayClipAtPoint(plopSounds[Random.Range(0, plopSounds.Length - 1)], destination);
            }
            yield return null;

            placedBuilding.GetComponent<Building>().setToConstruction();
        }

        currentBuilding.gameObject.GetComponent<buildingGhost>().setColliding(false);
    }
}
