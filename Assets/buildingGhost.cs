using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildingGhost : MonoBehaviour
{
	Color previousColor;
	Renderer renderer;
	public bool colliding = false;
	public bool active = true;

	void Start() {
		renderer = this.transform.Find("Model").gameObject.GetComponent<MeshRenderer>();
		previousColor = renderer.material.color;
	}

    public void OnTriggerExit(Collider other) {
    	if (active) {
    		colliding = false;
            if (renderer != null) {
                renderer.material.color = previousColor;
            }
    	}
    }

    public void OnTriggerEnter(Collider other) {
    	if (active) {
    		colliding = true;
            if (renderer != null) {
                renderer.material.color = Color.red;
            }
    	}
    }
}
