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
    public bool inFight = false;
    public bool isSelected = false;
	public bool selectable = false;
    private int unitMask = 1 << 12;
    protected float attackRate = 1f;

    public bool updateTargetLive = false;

	public Attackable attackee = null;
    public int attackeeId = 0;

	protected int lastNoEnemies = 0;
    protected float responseRange;

    

    public override void OnEnable() {
        if (this.photonView.IsMine) {
            InvokeRepeating("checkEnemiesInRange", 0.5f, 0.5f);
        }

        PhotonNetwork.AddCallbackTarget(this);
        base.OnEnable();
    }

    public override void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
        base.OnDisable();
    }

    // Update is called once per frame
    public override void Update()
    {
        if (animator != null && GetComponent<UnityEngine.AI.NavMeshAgent>() != null) {
            animator.SetFloat("speed", this.GetComponent<UnityEngine.AI.NavMeshAgent>().velocity.magnitude);
        }

        // If unit is currently attacking another unit (attackable that can move), update that target's position every frame.

        if (updateTargetLive == true && attackee != null && this.gameObject.GetComponent<NavMeshAgent>() != null) {
            this.photonView.RPC("setDestination", RpcTarget.All, attackee.gameObject.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position));
        }

        base.Update();
    }

    public override void onCapture() {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        string colorName = GetComponent<ownership>().getPlayer().colorName;

        this.owner.getPlayer().addUnit();

        for (int i = 0; i < renderers.Length; i++) {
            renderers[i].material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Units_" + colorName) as Texture));
        }
    }

    public override void destroyObject() {
        int playerID = this.gameObject.GetComponent<ownership>().owner;
        GameObject player = GameObject.Find(playerID.ToString());

        if (this.photonView.IsMine) {
            this.owner.getPlayer().addUnit(-1);
        }

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

        photonView.RPC("setDestination", RpcTarget.AllBuffered, destination);
    }

    [PunRPC]
    protected virtual void setDestination(Vector3 destination) {
        this.gameObject.GetComponent<NavMeshAgent>().destination = (destination);
    }

    public virtual void callAttack(int idOfThingToAttack) {
        if (canAttack) {
            if (photonView.IsMine) {
        	   StartCoroutine(moveAndAttack(idOfThingToAttack));
            }

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
                attackeeId = idOfThingToAttack;

                if (thingToAttackObj.tag == "unit") {
                    updateTargetLive = true;
                }

                attackee.attackers.Add(this);
                isAttacking = true;

				this.photonView.RPC("setDestination", RpcTarget.All, attackee.gameObject.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position));
				yield return new WaitUntil (() => isInRange(thingToAttack));
                this.inFight = true;
				this.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                print("TRYING TO ATTACK 1 ");
				InvokeRepeating("attack", attackRate, attackRate);
			}
		}
    }

    public virtual void attack() {
    	if (attackee != null && attackee.hp > 0) {
            photonView.RPC("attackRPC", RpcTarget.All, attackeeId);
    	} else {
    		cancelOrders();
    	}
    }

    [PunRPC]
    public void attackRPC(int idOfThingToAttack) {
        GameObject thingToAttackObj = GameObject.Find(idOfThingToAttack.ToString());
        Attackable thingToAttack = thingToAttackObj.GetComponent<Attackable>();
        if (animator != null) {
            animator.SetTrigger("attack");
        }

        this.inFight = true;

        NavMeshAgent navAgent = this.gameObject.GetComponent<NavMeshAgent>();
        if (navAgent != null) {
            this.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
        }

        this.faceTarget(thingToAttack.transform);
        thingToAttack.takeDamage(this.atk);
    }

    public virtual void cancelOrders() {
        Debug.Log("Cancelling orders locally");
        photonView.RPC("cancelOrdersRPC", RpcTarget.All);
    }

    [PunRPC]
    public virtual void cancelOrdersRPC() {
        Debug.Log("Cancelling orders through RPC now");
    	if (attackee != null) {
    		attackee.attackers.Remove(this); // Remove this attacker from that units attackers
    	}

        updateTargetLive = false;
    	CancelInvoke("attack");
    	this.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
    	this.gameObject.GetComponent<NavMeshAgent>().isStopped = false;
        this.attackeeId = 0;
        isAttacking = false;
        inFight = false;
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
                    callAttack(closestAttackee.id);
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

                            callAttack(hitColliders[i].gameObject.GetComponent<Attackable>().id);
                            break;
                        } 
                    }
                }

                lastNoEnemies = hitColliders.Length;
            }
        }  
    }

    void OnCollisionEnter(Collision collision) {
        if (this.photonView.IsMine) {
            Unit collidingUnit = collision.gameObject.GetComponent<Unit>();
            if (collidingUnit != null && collidingUnit.gameObject.GetComponent<ownership>().owner != this.gameObject.GetComponent<ownership>().owner && !this.inFight) {
                cancelOrders();
                callAttack(collidingUnit.id);
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

    [PunRPC]
    public void updatePosition(Vector3 pos) {
        Debug.Log("RECEIVED");
        this.transform.position = pos;
    }

    public override void OnPlayerEnteredRoom(Player other) {
        Debug.Log("PLAYER ENTERED DA ROOM");
        if (PhotonNetwork.IsMasterClient) {
            Debug.Log("Sending RPC");
            this.photonView.RPC("updatePosition", RpcTarget.OthersBuffered, this.transform.position);
        }
    }

    public virtual void onSelect() {
        isSelected = true;
    }

    public virtual void onDeSelect() {
        isSelected = false;
    }
}
