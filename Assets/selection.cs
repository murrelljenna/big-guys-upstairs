﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
using Photon.Realtime;

public class selection : MonoBehaviour
{
	public Camera cam;
	[SerializeField] List<GameObject> selected;
	game.assets.Player player;
	PhotonView photonView;

 	int layerMask;
 	int terrainMask;
 	int attackableMask;
 	int allMask;
    // Start is called before the first frame update
    void Start()
    {
    	selected = new List<GameObject>();
        layerMask = 1 << 12;
        terrainMask = 1 << 11;
        attackableMask = (1 << 9) | (1 << 10) | (1 << 12);
        allMask = terrainMask | attackableMask;  // For terrain and attackable objects.
        Debug.Log(allMask);
        player = this.transform.parent.parent.parent.parent.gameObject.GetComponent<game.assets.Player>();
    }

    void OnDisable() {
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
				if (hit.collider.gameObject.GetComponent<ownership>().owner == player.playerID) {
					if (selected.Count > 0 && selected.Exists(unit => unit.GetInstanceID() == hit.collider.gameObject.GetInstanceID())) {
						Debug.Log(selected.Count);
						selected.Remove(hit.collider.gameObject);
					} else {
						selected.Add(hit.collider.gameObject);
					}
				}
			}

			if (Input.GetKey(KeyCode.LeftShift)) {
				if (Input.GetMouseButtonDown(0)) {
					Debug.Log("ARF");
					selected.Clear();
				}
			}
		}

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, allMask)) {
			if (Input.GetMouseButtonDown(1)) {
				Vector3 destination = hit.point;
				bool alt = true; // Used to offset movement targets so not all units navigate to exact same positiln.

				if (attackableMask == (attackableMask | (1 << hit.collider.gameObject.layer)) 
					&& hit.collider.gameObject.GetComponent<ownership>().owned == true 
					&& hit.collider.gameObject.GetComponent<ownership>().owner != player.playerID) {
					
					selected.ForEach(unit => {
						unit.gameObject.GetComponent<Unit>().cancelOrders();
						photonView = unit.gameObject.GetComponent<PhotonView>();
						photonView.RPC("callAttack", RpcTarget.All, hit.collider.gameObject.GetComponent<Attackable>().id);

						/* Alternate between offsetting x and y so that all units aren't trying to navigate to same location */

						if (alt = true) {
							destination.x += 0.2f;
						} else {
							destination.z += 0.2f;
						}

						alt = !alt;
					});
				} else if (terrainMask == (terrainMask | (1 << hit.collider.gameObject.layer))) {
					selected.ForEach(unit => {
						unit.gameObject.GetComponent<Unit>().cancelOrders();

						destination = new Vector3(
							UnityEngine.Random.Range(
								hit.point.x - (0.1f * selected.Count), 
								hit.point.x + (0.1f * selected.Count)
							), 
							0, 
							UnityEngine.Random.Range(
								hit.point.z - (0.1f * selected.Count), 
								hit.point.z + (0.1f * selected.Count)
							)
						);

						unit.gameObject.GetComponent<Unit>().move(destination);

						/* Alternate between offsetting x and y so that all units aren't trying to navigate to same location */

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
    }

    void sendUnits() {

    }
}
