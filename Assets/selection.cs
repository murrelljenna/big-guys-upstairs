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

	Vector3 firstPoint;
	Vector3 lastPoint;
	bool firstPointPlaced = false;

 	int layerMask;
 	int terrainMask;
 	int attackableMask;
 	int allMask;
 	int unitMask;
    // Start is called before the first frame update
    void Start()
    {
    	selected = new List<GameObject>();
        layerMask = 1 << 12;
        terrainMask = 1 << 11;
        attackableMask = (1 << 9) | (1 << 10) | (1 << 12);
        allMask = terrainMask | attackableMask;  // For terrain and attackable objects.
        player = this.transform.parent.parent.parent.parent.gameObject.GetComponent<game.assets.Player>();
    }

    void OnDisable() {
		clearSelection();
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
				firstPointPlaced = false;
				if (hit.collider.gameObject.GetComponent<ownership>().owner == player.playerID) {
					if (selected.Count > 0 && selected.Exists(unit => unit.GetInstanceID() == hit.collider.gameObject.GetInstanceID())) {
						deselectUnit(hit.collider.gameObject);
					} else {
						selectUnit(hit.collider.gameObject);
					}
				}
			}
		}

		// Group select with shift-left-click

		if (Input.GetKey(KeyCode.LeftShift)) {
			if (Input.GetMouseButtonDown(0)) {
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainMask)) {
					if (!firstPointPlaced) {
						firstPoint = hit.point;
						firstPointPlaced = true;
					} else {
						lastPoint = hit.point;

						float topX = firstPoint.x > lastPoint.x ? firstPoint.x : lastPoint.x;
						float topZ = firstPoint.z > lastPoint.z ? firstPoint.z : lastPoint.z;
						float bottomX = firstPoint.x < lastPoint.x ? firstPoint.x : lastPoint.x;
						float bottomZ = firstPoint.z < lastPoint.z ? firstPoint.z : lastPoint.z;

						Vector3 center = new Vector3(bottomX + (topX-bottomX)/2, 0, bottomZ + (topZ-bottomZ)/2);
						Vector3 halfExtent = new Vector3(topX - bottomX, 20, topZ-bottomZ);

						Collider[] selectedUnits = Physics.OverlapBox(center, halfExtent, Quaternion.identity, layerMask);

						/*
						Currently, this section does not properly implement the rotation of physics.OverlapBox. Will work on later.

				        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				        cube.transform.position = new Vector3(center.x, 0, center.z);

				        cube.transform.rotation = Quaternion.FromToRotation(Vector3.up, transform.forward);
						*/


						foreach(Collider unit in selectedUnits) {
							if (unit.gameObject.GetComponent<ownership>().owner == player.playerID) {
								selectUnit(unit.gameObject);
							}
						}

						firstPointPlaced = false;
					}
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
						if (unit != null) {
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
						}
					});
				} else if (terrainMask == (terrainMask | (1 << hit.collider.gameObject.layer))) {
					int i = 0;
					List<Vector3> grid = createGrid(hit.point, selected.Count, 0.2f, 0.2f);
					selected.ForEach(unit => {
						if (unit != null) {
							unit.gameObject.GetComponent<Unit>().cancelOrders();

							destination = grid[i];
							Debug.Log(destination);

							if (unit.gameObject.GetComponent<NavMeshAgent>() != null) {
								unit.gameObject.GetComponent<Unit>().move(destination);
							}

							i++;
					    }
					});
				}	
			}
		}
    }

    void selectUnit(GameObject unit) {
		selected.Add(unit.GetComponent<Collider>().gameObject);

		if (unit.GetComponent<LineRenderer>() != null) {
			unit.GetComponent<LineRenderer>().enabled = true;
		}
    }

    public void deselectUnit(GameObject unit) {
    	if (selected.Contains(unit)) {
			selected.Remove(unit.GetComponent<Collider>().gameObject);
		}

		if (unit.GetComponent<Collider>().gameObject.GetComponent<LineRenderer>() != null) {
			unit.GetComponent<Collider>().gameObject.GetComponent<LineRenderer>().enabled = false;
		}
    }

														// 0.3
    public List<Vector3> createGrid(Vector3 center, float noOfUnits, float unitSize, float gapSize) {
    	float offset = unitSize + gapSize;
    			    	//GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		    	//cube.transform.position = new Vector3(center.x, 0, center.z);
    	float x; // Set beginning point of grid traversal.
    	float z = center.z - (noOfUnits * (unitSize + gapSize)) / 8;

    	List<Vector3> grid = new List<Vector3>();

    	for (int i = 0; i < Mathf.RoundToInt(Mathf.Ceil(Mathf.Sqrt(noOfUnits))); i++) {
    		x = center.x - (noOfUnits/8 * (unitSize + gapSize));

    		for (int j = 0; j < Mathf.RoundToInt(Mathf.Ceil(Mathf.Sqrt(noOfUnits))); j++) {
    			float y = getTerrainHeight(new Vector3(x, 0, z));
    			grid.Add(new Vector3(x, y, z));

    			x += offset;
    		}
    		z += offset;
    	}

    	return grid;
    }

   	float getTerrainHeight(Vector3 point) {
   		RaycastHit hit;
   		Physics.Raycast(new Vector3(point.x, 300, point.z), Vector3.down, out hit, Mathf.Infinity, terrainMask);
   		return hit.point.y;
   	}

    void clearSelection() {
    	selected.ForEach(unit => {
    		if (unit != null && unit.GetComponent<LineRenderer>() != null) {
    			unit.GetComponent<LineRenderer>().enabled = false;
    		}
    	});
    	selected.Clear();
    }
}
