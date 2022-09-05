using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
using Photon.Realtime;

public class Unit : Attackable
{
    private Camera playerCamera;

	public int atk; // Damage inflicted per second
	public float rng = 0.02f; // x/z Range of attack
	public bool isAttacking = false;
    public bool isSelected = false;
	public bool selectable = false;

    public bool updateTargetLive = false;

    private GameObject canvas;
    private SimpleHealthBar healthBar;

	PhotonView photonView;
	Attackable attackee = null;

	private int lastNoEnemies = 0;

    void OnEnable() {
    	photonView = PhotonView.Get(this);
    	InvokeRepeating("checkEnemiesInRange", 2.0f, 2.0f);

        canvas = this.gameObject.transform.Find("Canvas").gameObject;
        healthBar = canvas.transform.Find("Simple Bar").transform.Find("Status Fill 01").GetComponent<SimpleHealthBar>();

        playerCamera = GameObject.Find("FirstPersonCharacter").GetComponent<Camera>(); 
    }

    // Update is called once per frame
    public override void Update()
    {
        if (!isAttacking || attackee == null || attackee.hp <= 0) {
        	cancelOrders();
        }

        canvas.transform.LookAt(transform.position + playerCamera.transform.rotation * Vector3.forward, playerCamera.transform.rotation * Vector3.up);   

        if (this.hp != this.lastHP) {
            healthBar.UpdateBar(this.hp, this.maxHP);
        }

        // If unit is currently attacking another unit (attackable that can move), update that target's position every frame.

        if (updateTargetLive == true && attackee != null) {
            this.gameObject.GetComponent<NavMeshAgent>().destination = attackee.gameObject.transform.position;
        }

        base.Update();
    }

    public override void onCapture() {
    	this.gameObject.transform.Find("Capsule").GetComponent<MeshRenderer>().material.color = GetComponent<ownership>().playerColor;
    }

    public override void destroyObject() {
        int playerID = this.gameObject.GetComponent<ownership>().owner;
        GameObject player = GameObject.Find(playerID.ToString());

        AudioSource[] sources = this.gameObject.transform.Find("DestroySounds").GetComponents<AudioSource>();
        sources[UnityEngine.Random.Range(0, sources.Length)].Play((ulong)UnityEngine.Random.Range(0l, 2l));

		player.transform.Find("FPSController").transform.Find("FirstPersonCharacter").transform.Find("Tools").transform.Find("Command").GetComponent<selection>().deselectUnit(this.gameObject);
        base.destroyObject();
    }

    public void move(Vector3 destination) {
    	this.gameObject.GetComponent<NavMeshAgent>().destination = (destination);
    }

    [PunRPC]
    public void callAttack(int idOfThingToAttack, PhotonMessageInfo info) {
    	StartCoroutine(moveAndAttack(idOfThingToAttack));

        AudioSource[] sources = this.gameObject.transform.Find("AttackingSounds").GetComponents<AudioSource>();
        sources[UnityEngine.Random.Range(0, sources.Length)].Play((ulong)UnityEngine.Random.Range(0l, 2l));

    	lastNoEnemies=-1; 
    }

    public IEnumerator moveAndAttack(int idOfThingToAttack) {
    	GameObject thingToAttackObj = GameObject.Find(idOfThingToAttack.ToString());
    	if (thingToAttackObj != null) {
    		Attackable thingToAttack = thingToAttackObj.GetComponent<Attackable>();
    	 	if (thingToAttack.hp > 0) {
                attackee = thingToAttack;
                if (thingToAttackObj.tag == "unit") {
                    updateTargetLive = true;
                }
				this.gameObject.GetComponent<NavMeshAgent>().destination = thingToAttack.gameObject.transform.position;

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
    		attackee.takeDamage(atk);
    	} else {
    		cancelOrders();
    	}
    }

    public void cancelOrders() {
    	if (attackee != null) {
    		attackee.attackers.Remove(this); // Remove this attacker from that units attackers
    	}

        updateTargetLive = false;
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

    public void checkEnemiesInRange() {
    	if (!isAttacking) {
		    Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Collider>().bounds.center, 2f);

		    if (hitColliders.Length != lastNoEnemies) {
				for (int i = 0; i < hitColliders.Length; i++) {
					if (hitColliders[i] != null) {

						Debug.Log(hitColliders[i].tag);
						if (hitColliders[i].GetComponent<ownership>() != null &&
							hitColliders[i].GetComponent<ownership>().owned == true && 
							hitColliders[i].GetComponent<ownership>().owner != this.gameObject.GetComponent<ownership>().owner) { 

							photonView.RPC("callAttack", RpcTarget.All, hitColliders[i].gameObject.GetComponent<Attackable>().id);
							break;
						} 
					}
				}

				lastNoEnemies = hitColliders.Length;
			}
    	}  
    }

    public void checkEnemiesInRange(float range) {
        if (!isAttacking) {
            Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Collider>().bounds.center, range);

            if (hitColliders.Length != lastNoEnemies) {
                for (int i = 0; i < hitColliders.Length; i++) {
                    if (hitColliders[i] != null) {

                        Debug.Log(hitColliders[i].tag);
                        if (hitColliders[i].GetComponent<ownership>() != null &&
                            hitColliders[i].GetComponent<ownership>().owned == true && 
                            hitColliders[i].GetComponent<ownership>().owner != this.gameObject.GetComponent<ownership>().owner) { 

                            photonView.RPC("callAttack", RpcTarget.All, hitColliders[i].gameObject.GetComponent<Attackable>().id);
                            break;
                        } 
                    }
                }

                lastNoEnemies = hitColliders.Length;
            }
        }  
    }

    public void onSelect() {

    }

    public void onDeSelect() {
        
    }

    void exitBuilding() {
    	
    }
}
