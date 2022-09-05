using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class GuardTower : Unit
{
    void Start()
    {
    	this.movable = false;
    	prefabName = "GuardTower";
    	responseRange = 4f;
        this.woodCost = 40;
        this.foodCost = 0;

        this.atk = 5;
        this.hp = 50;
        this.lastHP = this.hp;
        this.rng = 4f;

        base.Start();
    }

    public override void Update()
    {
        if (!isAttacking || attackee == null || attackee.hp <= 0) {
        	cancelOrders();
        }

        // If unit is currently attacking another unit (attackable that can move), update that target's position every frame.

        if (updateTargetLive == true && attackee != null && !isInRange(attackee)) {
            cancelOrders();
        }

        base.Update();
    }

    public override void onCapture() {
    	//this.gameObject.transform.Find("Capsule").GetComponent<MeshRenderer>().material.color = GetComponent<ownership>().playerColor;
    }

    public override IEnumerator moveAndAttack(int idOfThingToAttack) {
    	GameObject thingToAttackObj = GameObject.Find(idOfThingToAttack.ToString());
    	if (thingToAttackObj != null) {
    		Attackable thingToAttack = thingToAttackObj.GetComponent<Attackable>();
    	 	if (thingToAttack.hp > 0) {
                attackee = thingToAttack;
                if (thingToAttackObj.tag == "unit") {
                    updateTargetLive = true;
                }

				attackee = thingToAttack;
				attackee.attackers.Add(this);
				isAttacking = true;

				yield return new WaitUntil (() => isInRange(thingToAttack));

				InvokeRepeating("attack", 1f, 1f);
			}
		}
    }

    [PunRPC]
    public override void callAttack(int idOfThingToAttack, PhotonMessageInfo info) {
    	if (canAttack) {
	    	StartCoroutine(moveAndAttack(idOfThingToAttack));

	    	lastNoEnemies=-1;
	    }
    }

    public override void cancelOrders() {
    	if (attackee != null) {
    		attackee.attackers.Remove(this); // Remove this attacker from that units attackers
    	}

        updateTargetLive = false;
    	CancelInvoke("attack");

        isAttacking = false;
        attackee = null;
    }

    public override void attack() {
        AudioSource[] sources = this.gameObject.transform.Find("AttackSounds1").GetComponents<AudioSource>();
        sources[UnityEngine.Random.Range(0, sources.Length)].Play(0);

		sources = this.gameObject.transform.Find("AttackSounds2").GetComponents<AudioSource>();
        sources[UnityEngine.Random.Range(0, sources.Length)].Play(1);

    	base.attack();
    }

    public override void takeDamage(int damage) {
        AudioSource[] sources = this.transform.Find("DamageSounds").GetComponents<AudioSource>();
        sources[UnityEngine.Random.Range(0, sources.Length)].Play((ulong)UnityEngine.Random.Range(0l, 2l));

        base.takeDamage(damage);
    }
}
