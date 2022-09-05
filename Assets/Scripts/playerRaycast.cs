using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class playerRaycast : MonoBehaviour
{
    bool midAnimation = false;
    GameObject cityViewed = null;
    GameObject resourceViewed = null;
    GameObject buildingViewed = null;
	Camera cam;
    int resourceMask = 1 << 9;
    int townMask = 1 << 10;
    int buildingMask = 1 << 14;
    game.assets.Player player;

    GameObject up1;
    GameObject down1;
    GameObject up2;
    GameObject down2;

    private TooltipController tooltips;

    void Start()
    {
        cam = GetComponent<Camera>();
        player = this.transform.parent.parent.gameObject.GetComponent<game.assets.Player>();

        tooltips = GameObject.Find("Tooltips").GetComponent<TooltipController>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        /* Interaction with resource tiles */    	

		Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
	    if (Physics.Raycast(ray, out hit, Mathf.Infinity, resourceMask)) {
            ResourceTile tile = hit.collider.gameObject.GetComponent<ResourceTile>();

            Transform[] trans = hit.collider.gameObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in trans) {
                if (t.gameObject.name == "Info") {
                    resourceViewed = t.gameObject;
                    resourceViewed.SetActive(true);
                }
            }

            if (!midAnimation) {
                resourceViewed.transform.Find("1_Pressed").gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.E) && hit.collider.GetComponent<ownership>().owned == false) {
                if (player.canAfford(tile.woodCost, tile.foodCost)) {
                    up1 = resourceViewed.transform.Find("1_Normal").gameObject;
                    down1 = resourceViewed.transform.Find("1_Pressed").gameObject;

                    up1.SetActive(false);
                    down1.SetActive(true);
                    midAnimation = true;
                    Invoke("releaseButton1", 0.2f);

                    bool inRange = false;

                	Collider[] hitColliders = Physics.OverlapSphere(hit.collider.bounds.center, 10f);
                	for (int i = 0; i < hitColliders.Length; i++) {
                		if (hitColliders[i].tag == "town" && hitColliders[i].GetComponent<ownership>().owner == player.playerID) { // If there is a town in range that belongs to the player.
                            player.makeTransaction(tile.woodCost, tile.foodCost);
    						hit.collider.GetComponent<ownership>().capture(player);
                            inRange = true;
    						break;
                		} 
                	}

                    if (!inRange) {
                        tooltips.flashInsideTown();
                    }
                } else {
                    tooltips.flashLackResources();
                }      	
            }
	    } else if (resourceViewed != null) {
            resourceViewed.SetActive(false);
            resourceViewed = null;
        }

        
        /* Interaction with towns */ 

        ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, buildingMask)) {
            if (hit.collider.gameObject.tag != "buildingGhost" && hit.collider.GetComponent<ownership>().owner == player.playerID) {
                hit.collider.gameObject.GetComponent<Attackable>().interactionOptions(player);

                buildingViewed = hit.collider.gameObject.transform.Find("Info").gameObject;
            }
        }   else if (buildingViewed != null) {
            buildingViewed.SetActive(false);
            buildingViewed = null;
        }

        /* Interactions with barracks */

                /* Interaction with towns */ 

        ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, townMask)) {
            if (hit.collider.gameObject.tag != "buildingGhost" && hit.collider.GetComponent<ownership>().owner == player.playerID) {
                hit.collider.gameObject.GetComponent<Attackable>().interactionOptions(player);

                cityViewed = hit.collider.gameObject.transform.Find("Info").gameObject;
            }
        }   else if (cityViewed != null) {
            cityViewed.SetActive(false);
            cityViewed = null;
        }
    }

    private static Vector2 RandomPointOnUnitCircle(float radius)
    {
        float angle = Random.Range (0f, Mathf.PI * 2);
        float x = Mathf.Sin (angle) * radius;
        float y = Mathf.Cos (angle) * radius;

        return new Vector2(x, y);
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
            if (hitColliders[i].tag == "town" && hitColliders[i].GetComponent<ownership>().owner == ownerID) { // If there is a town in range that belongs to the player.
                return true;
            }
        }

        return false;
    }

    void releaseButton1() {
        up1.SetActive(true);
        down1.SetActive(false);
        midAnimation = false;
    }

    void releaseButton2() {
        up2.SetActive(true);
        down2.SetActive(false);
        midAnimation = false;
    }
}