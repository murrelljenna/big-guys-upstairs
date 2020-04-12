using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
using Photon.Realtime;

public class Unit : Attackable
{
    public bool movable;
	public int atk; // Damage inflicted per second
	public float rng = 0.08f; // x/z Range of attack
	public bool isAttacking = false;
    public bool isSelected = false;
	public bool selectable = false;
    private int unitMask = 1 << 12;

    public bool updateTargetLive = false;

	public Attackable attackee = null;

	protected int lastNoEnemies = 0;
    protected float responseRange;

    

    void OnEnable() {
    	InvokeRepeating("checkEnemiesInRange", 0.5f, 0.5f);

        base.OnEnable();
    }

    // Update is called once per frame
    public override void Update()
    {
        if (animator != null && GetComponent<UnityEngine.AI.NavMeshAgent>() != null) {
            animator.SetFloat("speed", this.GetComponent<UnityEngine.AI.NavMeshAgent>().velocity.magnitude);
        }

        // If unit is currently attacking another unit (attackable that can move), update that target's position every frame.

        if (updateTargetLive == true && attackee != null && this.gameObject.GetComponent<NavMeshAgent>() != null) {
            this.gameObject.GetComponent<NavMeshAgent>().destination = attackee.gameObject.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position);
        }

        base.Update();
    }

    public override void onCapture() {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        string colorName = GetComponent<ownership>().getPlayer().colorName;

        for (int i = 0; i < renderers.Length; i++) {
            renderers[i].material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Units_" + colorName) as Texture));
        }
    }

    public override void destroyObject() {
        int playerID = this.gameObject.GetComponent<ownership>().owner;
        GameObject player = GameObject.Find(playerID.ToString());

        AudioSource[] sources = this.gameObject.transform.Find("DestroySounds").GetComponents<AudioSource>();
        sources[UnityEngine.Random.Range(0, sources.Length)].Play((ulong)UnityEngine.Random.Range(0l, 2l));

		player.transform.Find("FPSController").transform.Find("FirstPersonCharacter").transform.Find("Tools").transform.Find("Command").GetComponent<selection>().deselectUnit(this.gameObject);
        base.destroyObject();
    }

    public virtual void move(Vector3 destination) {
        cancelOrders();
        AudioSource[] sources = this.transform.Find("SelectionSounds").GetComponents<AudioSource>();
        AudioSource source = sources[UnityEngine.Random.Range(0, sources.Length)];
        AudioSource.PlayClipAtPoint(source.clip, this.transform.position);

        photonView.RPC("setDestination", RpcTarget.All, destination);
    }

    [PunRPC]
    protected virtual void setDestination(Vector3 destination) {
        this.gameObject.GetComponent<NavMeshAgent>().destination = (destination);
    }

    [PunRPC]
    public virtual void callAttack(int idOfThingToAttack, PhotonMessageInfo info) {
        if (canAttack) {
        	StartCoroutine(moveAndAttack(idOfThingToAttack));

            AudioSource[] sources = this.gameObject.transform.Find("AttackingSounds").GetComponents<AudioSource>();
            sources[UnityEngine.Random.Range(0, sources.Length)].Play((ulong)UnityEngine.Random.Range(0l, 2l));

        	lastNoEnemies=-1;
        }
    }

    public virtual IEnumerator moveAndAttack(int idOfThingToAttack) {
    	GameObject thingToAttackObj = GameObject.Find(idOfThingToAttack.ToString());
    	if (thingToAttackObj != null) {
    		Attackable thingToAttack = thingToAttackObj.GetComponent<Attackable>();
    	 	if (thingToAttack.hp > 0) {
                attackee = thingToAttack;

                if (thingToAttackObj.tag == "unit") {
                    updateTargetLive = true;
                }

                attackee.attackers.Add(this);
                isAttacking = true;

				GetComponent<NavMeshAgent>().SetDestination(attackee.gameObject.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position));
				yield return new WaitUntil (() => isInRange(thingToAttack));
				this.gameObject.GetComponent<NavMeshAgent>().isStopped = true;

				InvokeRepeating("attack", 1f, 1f);
			}
		}
    }

    public virtual void attack() {
        if (animator != null) {
            animator.SetTrigger("attack");
        }

    	if (attackee != null && attackee.hp > 0) {
            this.faceTarget(attackee.transform);
    		attackee.takeDamage(atk);
    	} else {
    		cancelOrders();
    	}
    }

    public virtual void cancelOrders() {
        photonView.RPC("cancelOrdersRPC", RpcTarget.All);
    }

    [PunRPC]
    public virtual void cancelOrdersRPC() {
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

    protected bool isInRange(Attackable thingToAttack) {
    	if (thingToAttack != null) {
            Vector3 closestPoint = thingToAttack.gameObject.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position);

            float deltaX = this.gameObject.transform.position.x - closestPoint.x;
            float deltaZ = this.gameObject.transform.position.z - closestPoint.z; 
            float distance = Mathf.Sqrt(deltaX * deltaX + deltaZ * deltaZ);

            return (distance < this.rng);
    	}

        return false;
    }

    public void checkEnemiesInRange() {
    	if (!isAttacking && canAttack) {
		    Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Collider>().bounds.center, responseRange, unitMask);
            Attackable closestAttackee = null;
            Vector3 playerPos = this.GetComponent<Collider>().bounds.center;

		    if (hitColliders.Length != lastNoEnemies) {
				for (int i = 0; i < hitColliders.Length; i++) {
					if (hitColliders[i] != null) {
						if (hitColliders[i].GetComponent<ownership>() != null &&
							hitColliders[i].GetComponent<ownership>().owned == true && 
							hitColliders[i].GetComponent<ownership>().owner != this.gameObject.GetComponent<ownership>().owner) { 

                            if (closestAttackee == null) {
                                closestAttackee = hitColliders[i].GetComponent<Attackable>();
                            } else if (Vector3.Distance(hitColliders[i].bounds.center, playerPos) < Vector3.Distance(closestAttackee.GetComponent<Collider>().bounds.center, playerPos)) {
                                closestAttackee = hitColliders[i].GetComponent<Attackable>();
                            }
							break;
						} 
					}
				}

                if (closestAttackee != null) {
                    photonView.RPC("callAttack", RpcTarget.All, closestAttackee.id);
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

    private void faceTarget(Transform target) {
        if (this.GetComponent<UnityEngine.AI.NavMeshAgent>() != null) {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * this.GetComponent<UnityEngine.AI.NavMeshAgent>().angularSpeed);
        }
    }

    public virtual void onSelect() {
        isSelected = true;
    }

    public virtual void onDeSelect() {
        isSelected = false;
    }
}
