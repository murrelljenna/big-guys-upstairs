using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
using Photon.Realtime;

public class Unit : Attackable
{
	public int atk; // Damage inflicted per second
	public float rng = 0.02f; // x/z Range of attack
	public bool isAttacking = false;
	public bool selectable = false;

	PhotonView photonView;
	Attackable attackee = null;

	private int lastNoEnemies = 0;

    void OnEnable() {
    	photonView = PhotonView.Get(this);
    	InvokeRepeating("checkEnemiesInRange", 2.0f, 2.0f);
    }

    // Update is called once per frame
    public override void Update()
    {
        if (!isAttacking || attackee == null || attackee.hp <= 0) {
        	cancelOrders();
        }

        base.Update();
    }

    public void move(Vector3 destination) {
    	this.gameObject.GetComponent<NavMeshAgent>().destination = (destination);
    }

    [PunRPC]
    public void callAttack(int idOfThingToAttack, PhotonMessageInfo info) {
    	StartCoroutine(moveAndAttack(idOfThingToAttack));
    	lastNoEnemies=-1; 
    }

    public IEnumerator moveAndAttack(int idOfThingToAttack) {
    	GameObject thingToAttackObj = GameObject.Find(idOfThingToAttack.ToString());
    	if (thingToAttackObj != null) {
    		Attackable thingToAttack = thingToAttackObj.GetComponent<Attackable>();
    	 	if (thingToAttack.hp > 0) {
    		
				this.gameObject.GetComponent<NavMeshAgent>().destination = (thingToAttack.gameObject.transform.position);

				attackee = thingToAttack;
				attackee.attackers.Add(this);
				isAttacking = true;

				yield return new WaitUntil (() => isInRange(thingToAttack));
				this.gameObject.GetComponent<NavMeshAgent>().isStopped = true;

				InvokeRepeating("attack", 1f, 1f);
			}
		}
    }

    public void attack() {
    	if (attackee.hp > 0) {
    		attackee.hp -= atk;
    	} else {
    		cancelOrders();
    	}
    }

    public void cancelOrders() {
    	if (attackee != null) {
    		attackee.attackers.Remove(this); // Remove this attacker from that units attackers
    	}

    	CancelInvoke("attack");
    	this.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
    	this.gameObject.GetComponent<NavMeshAgent>().isStopped = false;
        isAttacking = false;
        attackee = null;
    }

    bool isInRange(Attackable thingToAttack) {
    	if (thingToAttack != null) {
	    	return (this.gameObject.transform.position.x - thingToAttack.gameObject.transform.position.x < (rng)
				&& this.gameObject.transform.position.x - thingToAttack.gameObject.transform.position.x > (rng * -1f)
				|| this.gameObject.transform.position.z - thingToAttack.gameObject.transform.position.z > (rng * -1f)
				&& this.gameObject.transform.position.z - thingToAttack.gameObject.transform.position.z < (rng));
    	} else {
    		return false;
    	}
    }

    void checkEnemiesInRange() {
    	if (!isAttacking && this.gameObject.GetComponent<NavMeshAgent>().isStopped) {
		    Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Collider>().bounds.center, 2f);

		    if (hitColliders.Length != lastNoEnemies) {
				for (int i = 0; i < hitColliders.Length; i++) {
					if (hitColliders[i] != null) {
						Debug.Log("There be enemies amidst");
						Debug.Log(hitColliders[i].tag);
						if (hitColliders[i].GetComponent<ownership>() != null &&
							hitColliders[i].GetComponent<ownership>().owned == true && 
							hitColliders[i].GetComponent<ownership>().owner != this.gameObject.GetComponent<ownership>().owner) { 
							Debug.Log("Attack!");
							photonView.RPC("callAttack", RpcTarget.All, hitColliders[i].gameObject.GetComponent<Attackable>().id);
							break;
						} 
					}
				}

				lastNoEnemies = hitColliders.Length;
			}
    	}  
    }

    void exitBuilding() {
    	
    }
}
