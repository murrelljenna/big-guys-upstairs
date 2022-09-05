using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
using Photon.Realtime;

public class Unit : Attackable
{
	public int atk; // Damage inflicted per second
	public float rng; // x/z Range of attack
	public bool isAttacking;
	public bool selectable = false;

	PhotonView photonView;
	Attackable attackee = null;

    void OnEnable() {
    	photonView = PhotonView.Get(this);
    }

    // Update is called once per frame
    public override void Update()
    {
    	base.Update();
        if (!isAttacking || attackee == null || attackee.hp <= 0) {
        	cancelOrders();
        }
    }

    public void move(Vector3 destination) {
    	Debug.Log("moving");
    	this.gameObject.GetComponent<NavMeshAgent>().destination = (destination);
    }

    [PunRPC]
    public void callAttack(int idOfThingToAttack, PhotonMessageInfo info) {
    	StartCoroutine(moveAndAttack(idOfThingToAttack));
    }

    public IEnumerator moveAndAttack(int idOfThingToAttack) {
    	Attackable thingToAttack = GameObject.Find(idOfThingToAttack.ToString()).GetComponent<Attackable>();
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

    	CancelInvoke();
    	this.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
    	this.gameObject.GetComponent<NavMeshAgent>().isStopped = false;
        isAttacking = false;
        attackee = null;
    }

    bool isInRange(Attackable thingToAttack) {
    	return (this.gameObject.transform.position.x - thingToAttack.gameObject.transform.position.x < (rng)
			&& this.gameObject.transform.position.x - thingToAttack.gameObject.transform.position.x > (rng * -1f)
			|| this.gameObject.transform.position.z - thingToAttack.gameObject.transform.position.z > (rng * -1f)
			&& this.gameObject.transform.position.z - thingToAttack.gameObject.transform.position.z < (rng));
    }
}
