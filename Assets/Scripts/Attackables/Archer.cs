using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
using Photon.Realtime;

public class Archer : Unit
{
	public GameObject missile;

    // Start is called before the first frame update
    void Start()
    {
    	this.movable = true;
    	this.responseRange = 3f;
        this.woodCost = 2;
        this.foodCost = 10;

        this.atk = 2;
        this.hp = 8;
        this.lastHP = this.hp;
        this.rng = 3f;
        this.attackRate = 1.8f;

        base.Start();
    }

    void Update() {
        base.Update();
    }

    public override void onSelect() {
        AudioSource[] sources = this.transform.Find("SelectionSounds").GetComponents<AudioSource>();
        AudioSource source = sources[UnityEngine.Random.Range(0, sources.Length)];
        AudioSource.PlayClipAtPoint(source.clip, this.transform.position);

        base.onSelect();
    }

    public override void attack() {
        if (attackee != null && attackee.hp > 0) {
            if (this.photonView.IsMine) {
                this.photonView.RPC("fireProjectileRPC", RpcTarget.All, attackee.id);
            }
        } else {
            cancelOrders();
        }
    }

    private void fireProjectile() {
        AudioSource[] sources = this.gameObject.transform.Find("AttackSounds2").GetComponents<AudioSource>();    
        sources[UnityEngine.Random.Range(0, sources.Length)].Play(1);

        GameObject bow = this.transform.Find("Model").Find("Bip001").Find("Bip001 Pelvis").Find("Bip001 Spine").Find("Bip001 L Clavicle").Find("Bip001 L UpperArm").Find("Bip001 L Forearm").Find("Bip001 L Hand").Find("L_hand_container").Find("w_recurve_bow").gameObject;
        GameObject arrow = Instantiate(missile, bow.transform.position, Quaternion.LookRotation((attackee.gameObject.transform.position - bow.transform.position).normalized));
        arrow.transform.Rotate(-90, 0, 0); // Can't figure out how to get this fucking thing to face the right way.

        if (photonView.IsMine) {
            arrow.GetComponent<launchMissile>().activated = true;
        }

        arrow.GetComponent<launchMissile>().dmg = this.atk;

        arrow.GetComponent<ownership>().localCapture(GetComponent<ownership>().getPlayer());

        arrow.GetComponent<Rigidbody>().AddForce((attackee.transform.position - bow.transform.position).normalized * 400);
    }

    [PunRPC]
    public void fireProjectileRPC(int idOfThingToAttack) {
        Invoke("fireProjectile", 0.4f);
        GameObject thingToAttackObj = GameObject.Find(idOfThingToAttack.ToString());
        attackee = thingToAttackObj.GetComponent<Attackable>();

    	if (attackee != null) {
            this.inFight = true;

            if (navAgent != null) {
                this.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
            }

            AudioSource[] sources = this.gameObject.transform.Find("AttackSounds1").GetComponents<AudioSource>();
            sources[UnityEngine.Random.Range(0, sources.Length)].Play(0);

            this.faceTarget(thingToAttackObj.transform);

            if (animator != null) {
                animator.SetTrigger("attack");
            }
		}
    }
}
