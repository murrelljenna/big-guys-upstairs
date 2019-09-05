using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class selection : MonoBehaviour
{
	public Camera cam;
	public List<GameObject> selected;
 	int layerMask;
 	int terrainMask;
    // Start is called before the first frame update
    void Start()
    {
    	selected = new List<GameObject>();
        layerMask = 1 << 12;
        terrainMask = 1 << 11;
    }

    void onDisable() {
		selected.Clear();
    }

    void onEnable() {
    	
    }
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

		Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
			if (Input.GetMouseButtonDown(0)) {
				Debug.Log(hit.collider.gameObject.name);
				if (selected.Exists(unit => unit.GetInstanceID() == hit.collider.gameObject.GetInstanceID())) {
					selected.Remove(hit.collider.gameObject);
				} else {
					selected.Add(hit.collider.gameObject);
				}
			}
		}
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainMask)) {
			if (Input.GetMouseButtonDown(1)) {
				Vector3 destination = hit.point;
				bool alt = true;
				selected.ForEach(unit => {
					unit.transform.parent.gameObject.GetComponent<NavMeshAgent>().destination = (destination);

					if (alt = true) {
						destination.x += 0.2f;
					} else {
						destination.z += 0.2f;
					}

					alt = !alt;
				});
			}
		}
    }

    void sendUnits() {

    }
}
