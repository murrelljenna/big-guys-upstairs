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
    
    void Start()
    {
        Debug.Log(this.transform.parent.parent.parent.parent.name);
    	wallet = this.transform.parent.parent.parent.parent.GetComponent<game.assets.Player>();
        currentBuilding = null;
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
	    		currentBuilding.position = hit.point;
                currentBuilding.tag = "buildingGhost";

	    		// Left-click to place building

	    		if (Input.GetMouseButtonDown(0)) {
	    			int wood = currentBuilding.GetComponent<Town>().woodCost;
	    			int food = currentBuilding.GetComponent<Town>().foodCost;

                    Debug.Log(townInRange(hit.point, 20f));

	    			if (wallet.canAfford(wood, food) && !townInRange(hit.point, 20f)) { // If can afford and no town radius overlapping
	    				wallet.makeTransaction(wood, food);
                        GameObject placedBuilding = PhotonNetwork.Instantiate("Town", hit.point, Quaternion.identity, 0);
                        placedBuilding.GetComponent<ownership>().capture(wallet);
                        Destroy(currentBuilding.gameObject);
	    			} else {
	    				Debug.Log("Not True");
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
            Debug.Log(hitColliders[i].tag);
            if (hitColliders[i].tag == "town" && hitColliders[i].GetComponent<ownership>().owner == ownerID) { // If there is a town in range that belongs to the player.
                return true;
            }
        }

        return false;
    }
}
