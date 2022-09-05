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

	    		// Left-click to place building

	    		if (Input.GetMouseButtonDown(0)) {
	    			int wood = currentBuilding.GetComponent<Town>().woodCost;
	    			int food = currentBuilding.GetComponent<Town>().foodCost;

	    			if (wallet.canAfford(wood, food)) {
	    				wallet.makeTransaction(wood, food);
                        this.currentBuilding.gameObject.name = "Town";
                        GameObject placedBuilding = PhotonNetwork.Instantiate(this.currentBuilding.gameObject.name, hit.point, Quaternion.identity, 0);
                        placedBuilding.GetComponent<LineRenderer>().enabled = false;
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
}
