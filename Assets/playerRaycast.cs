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
	    if (Physics.Raycast(ray, out hit, Mathf.Infinity, resourceMask)) {
            
            if (Input.GetKeyDown(KeyCode.E) && hit.collider.GetComponent<ownership>().owned == false) {
            	Collider[] hitColliders = Physics.OverlapSphere(hit.collider.bounds.center, 10f);
            	for (int i = 0; i < hitColliders.Length; i++) {
            		Debug.Log(hitColliders[i].tag);
            		if (hitColliders[i].tag == "town" && hitColliders[i].GetComponent<ownership>().owner == player.playerID) { // If there is a town in range that belongs to the player.
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
            if (Input.GetKeyDown(KeyCode.E) && hit.collider.GetComponent<ownership>().owner == player.playerID) {
                int wood = 0; // Please replace with real values soon.
                int food = 5;

                if (player.canAfford(wood, food)) {
                    player.makeTransaction(wood, food);

                    /* Instantiate new militia outside city */

                    Vector2 randomInCircle = RandomPointOnUnitCircle(1.2f);
                    Vector3 spawnLocation = new Vector3(randomInCircle.x+hit.transform.position.x, 0, randomInCircle.y+hit.transform.position.z);

                    GameObject militia = PhotonNetwork.Instantiate("Militia", spawnLocation, Quaternion.identity, 0);

                    militia.GetComponent<ownership>().capture(player);
                }
            }
        }
    }

    private static Vector2 RandomPointOnUnitCircle(float radius)
    {
        float angle = Random.Range (0f, Mathf.PI * 2);
        float x = Mathf.Sin (angle) * radius;
        float y = Mathf.Cos (angle) * radius;

        return new Vector2(x, y);
    }
}