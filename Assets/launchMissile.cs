using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class launchMissile : MonoBehaviour
{
    public void OnTriggerEnter(Collider collision) {
    	Unit collidingUnit = collision.gameObject.GetComponent<Unit>();
        if (collidingUnit != null && collidingUnit.gameObject.GetComponent<ownership>().owner != this.gameObject.GetComponent<ownership>().owner) {
        	Destroy(this);
        }
    }
}
