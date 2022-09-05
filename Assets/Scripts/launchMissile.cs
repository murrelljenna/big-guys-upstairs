using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class launchMissile : MonoBehaviour
{
    private bool activated = false;
    public int dmg;

    public void OnTriggerEnter(Collider collision) {
    	Attackable collidingEnemy = collision.gameObject.GetComponent<Attackable>();
        if (collidingEnemy != null && collidingEnemy.gameObject.GetComponent<ownership>().owner != this.gameObject.GetComponent<ownership>().owner) {
            //collidingEnemy.takeDamage(dmg);
        	Destroy(this);
        }
    }
}
