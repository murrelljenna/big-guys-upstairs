using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildingPlacement : MonoBehaviour
{
	res wallet;
	public Camera cam;
	private Transform currentBuilding;
	private int layerMask;
    // Start is called before the first frame update
    void Start()
    {
    	wallet = GameObject.Find("Player").GetComponent<res>();
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
	    			int wood = currentBuilding.Find("Logic").GetComponent<buildingCost>().wood;
	    			int food = currentBuilding.Find("Logic").GetComponent<buildingCost>().food;

	    			if (wallet.canAfford(wood, food)) {
	    				wallet.makeTransaction(wood, food);
	    				currentBuilding.Find("Logic").GetComponent<LineRenderer>().enabled = false;
	    				currentBuilding = null;
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
