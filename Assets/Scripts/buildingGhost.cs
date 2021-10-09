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
        if (this.transform.Find("Model2") != null) {
            this.transform.Find("Model2").gameObject.SetActive(false);
            this.transform.Find("Model3").gameObject.SetActive(false);
        }
        previousColor = renderer.material.color;
    }

    public void OnTriggerExit(Collider other) {
        if (active) {
            setColliding(false);
        }
    }

    public void OnTriggerEnter(Collider other) {
        if (active) {
            setColliding(true);
        }
    }

    public void setColliding(bool isColliding) {
        if (isColliding) {
            colliding = true;
            if (renderer != null) {
                renderer.material.color = Color.red;
            }
        } else {
            colliding = false;
            if (renderer != null) {
                renderer.material.color = previousColor;
            }
        }
    }
}
