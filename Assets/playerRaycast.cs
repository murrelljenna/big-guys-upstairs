using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class playerRaycast : MonoBehaviour
{
	Camera cam;
    int resourceMask = 1 << 9;
    int townMask = 1 << 10;
    game.assets.Player player;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        player = this.transform.parent.parent.gameObject.GetComponent<game.assets.Player>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        /* Interaction with resource tiles */    	

		Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
	    if (Physics.Raycast(ray, out hit, Mathf.Infinity, townMask)) {
            
            if (Input.GetKeyDown(KeyCode.E) && hit.collider.GetComponent<ownership>().owned == false) {
            	Collider[] hitColliders = Physics.OverlapSphere(hit.collider.bounds.center, 10f);
            	for (int i = 0; i < hitColliders.Length; i++) {
            		Debug.Log(hitColliders[i].tag);
            		if (hitColliders[i].tag == "town") {
                        Debug.Log(this.transform.parent.parent.gameObject.name);
						hit.collider.GetComponent<ownership>().capture(player); 
						
						GameObject Player = this.transform.parent.parent.gameObject;
    					Player.GetComponent<game.assets.Player>().addResource(hit.collider.tag);
						break;
            		} 

            	}
            	      	
            }
	    }

        /* Interaction with towns */ 

        ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, townMask)) { 
            if (Input.GetKeyDown(KeyCode.E) && hit.collider.GetComponent<ownership>().owner == player) {
                int wood = 0; // Please replace with real values soon.
                int food = 5;

                if (player.canAfford(wood, food)) {
                    player.makeTransaction(wood, food);

                    /* Instantiate new militia inside city */

                    Vector3 randomInCircle = new Vector3(UnityEngine.Random.Range(hit.transform.position.x - 0.5f, hit.transform.position.x + 0.5f), 0, UnityEngine.Random.Range(hit.transform.position.z - 0.5f, hit.transform.position.z + 0.5f));
                    GameObject militia = PhotonNetwork.Instantiate("Militia", randomInCircle, Quaternion.identity, 0);

                    militia.GetComponent<ownership>().capture(player);
                }
            }
        }
    }
}
