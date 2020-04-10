using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class buildingPlacement : MonoBehaviourPunCallbacks
{
	game.assets.Player wallet;
	public Camera cam;
	private Transform currentBuilding;
	private int layerMask;

    private TooltipController tooltips;
    Color previousColor;

    public AudioClip[] plopSounds;
    
    void Start()
    {
    	wallet = this.transform.parent.parent.parent.parent.GetComponent<game.assets.Player>();
        currentBuilding = null;
        tooltips = GameObject.Find("Tooltips").GetComponent<TooltipController>();
        layerMask = 1 << 11;
    }

    void OnDisable() {
    	if (currentBuilding != null) {
			Destroy(currentBuilding.gameObject);
			currentBuilding = null;
		}
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBuilding != null) {
            RaycastHit hit;

			Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

	    	if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {

                currentBuilding.position = new Vector3(hit.point.x, hit.point.y+0.5f, hit.point.z);

	    		if (Input.GetMouseButtonDown(0)) {

                    int wood = currentBuilding.GetComponent<Attackable>().woodCost;
                    int food = currentBuilding.GetComponent<Attackable>().foodCost;

	    			if (wallet.canAfford(wood, food)) { // If can afford and no town radius overlapping
                        if (!currentBuilding.GetComponent<buildingGhost>().colliding) {
                            if (currentBuilding.name == "town") {
                                if (!townInRange(hit.point, 20f)) {
                                    StartCoroutine(plopBuilding(currentBuilding.gameObject, hit.point));
                                    currentBuilding = null;
                                } else { // if town is TOO close
                                    StopAllCoroutines();
                                    tooltips.flashCityRadius();
                                    StartCoroutine(flashRed(currentBuilding.gameObject, 0.2f));
                                }
                            } else {
                                if (townInRange(hit.point, 10f, wallet.playerID)) {
                                    StartCoroutine(plopBuilding(currentBuilding.gameObject, hit.point));
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
        
        AudioSource.PlayClipAtPoint(plopSounds[Random.Range(0, plopSounds.Length - 1)], destination);

        Destroy(building);
    }
}
