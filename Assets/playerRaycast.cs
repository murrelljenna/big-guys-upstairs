using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerRaycast : MonoBehaviour
{
	Camera cam;
    int layerMask;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        layerMask = 1 << 9;
    }

    // Update is called once per frame
    void Update()
    {    	
        RaycastHit hit;

        
		Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

	    if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) 
	   	{
            if (Input.GetKeyDown(KeyCode.E) && hit.collider.GetComponent<ownership>().owned == false) {
            	Collider[] hitColliders = Physics.OverlapSphere(hit.collider.bounds.center, 10f);

            	for (int i = 0; i < hitColliders.Length; i++) {
            		Debug.Log(hitColliders[i].tag);
            		if (hitColliders[i].tag == "town") {
						hit.collider.GetComponent<ownership>().capture(); 
						
						GameObject Player = GameObject.Find("Player");
    					Player.GetComponent<res>().addResource(hit.collider.tag);
						break;
            		} 

            	}
            	      	
            }
	    } 

    }
}
