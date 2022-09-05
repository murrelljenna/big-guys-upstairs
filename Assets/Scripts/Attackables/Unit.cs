using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
using Photon.Realtime;

public class Unit : Attackable, IPunObservable
{
    public bool movable;
	public int atk; // Damage inflicted per second
	public float rng = 0.08f; // x/z Range of attack
	public bool isAttacking = false;
    public bool inFight = false;
    public bool isSelected = false;
	public bool selectable = false;
    protected int attackableMask = (1 << 9) | (1 << 10) | (1 << 12) | (1 << 14) | (1 << 16) | (1 << 18);
    protected float attackRate = 1f;

    protected bool updateTargetLive = false;
    private bool moveOrdered = false;

	public Attackable attackee = null;
    public int attackeeId = 0;

	protected int lastNoEnemies = 0;
    protected float responseRange;

    private int frameCount = 0;
    private int serializeCount = 0;
    protected NavMeshAgent navAgent;

    Vector3 receivedPosition;
 
    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;

    private Vector3 velocity;
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    public override void OnEnable() {
        PhotonNetwork.SendRate = 15;
        PhotonNetwork.SerializationRate = 15;

        if (this.photonView.IsMine) {
            InvokeRepeating("checkEnemiesInRange", 0.2f, 0.2f);
        }
        navAgent = this.gameObject.GetComponent<NavMeshAgent>();

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
        if (dead) {
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.isKinematic = true;
            canvas.SetActive(false);
            return;
        }

        if (animator != null && GetComponent<UnityEngine.AI.NavMeshAgent>() != null) {
            animator.SetFloat("speed", this.GetComponent<UnityEngine.AI.NavMeshAgent>().velocity.magnitude);
        }

        if (photonView.IsMine) {
            // If unit is currently attacking another unit (attackable that can move), update that target's position every frame.
            if (updateTargetLive && frameCount % 10 == 0) {
                if (attackee != null && this.gameObject.GetComponent<NavMeshAgent>() != null && this.photonView.IsMine) {
                    setDestination(attackee.gameObject.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position));
                }
            }

            // If unit has been ordered to move, wait until it reaches its destination before stopping
            if (moveOrdered && navAgent.remainingDistance <= navAgent.stoppingDistance) {
                if (!navAgent.hasPath || Mathf.Abs (navAgent.velocity.sqrMagnitude) < float.Epsilon) {
                    moveOrdered = false;
                }
            }
        } else {
            animator.SetFloat("speed", velocity.magnitude);
        }

        frameCount++;
        base.Update();
    }

    public void FixedUpdate() {
        if (!photonView.IsMine) {
            this.transform.position = Vector3.MoveTowards(this.transform.position, networkPosition, Time.fixedDeltaTime * 2.0f);
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, networkRotation, Time.fixedDeltaTime * 100.0f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(gameObject.transform.position);
            stream.SendNext(gameObject.transform.rotation);
            stream.SendNext(this.GetComponent<UnityEngine.AI.NavMeshAgent>().velocity);
        }
        else
        {

            // Network player, receive data
            networkPosition = (Vector3) stream.ReceiveNext();
            networkRotation = (Quaternion) stream.ReceiveNext();
            velocity = (Vector3) stream.ReceiveNext();

            float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.timestamp));
            networkPosition += velocity * lag;
        }
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
        this.dead = true;

        if (animator != null) {
            animator.SetTrigger("death");
            animator.SetFloat("speed", 0f);
        }

        this.GetComponent<Collider>().enabled = false;
        CancelInvoke();

        if (this.photonView.IsMine) {
            int playerID = this.gameObject.GetComponent<ownership>().owner;
            GameObject player = GameObject.Find(playerID.ToString());
            this.owner.getPlayer().addUnit(-1);
            player.transform.Find("FPSController").transform.Find("FirstPersonCharacter").transform.Find("Tools").transform.Find("Command").GetComponent<selection>().deselectUnit(this.gameObject);
            cancelOrders();
            Invoke("baseDestroy", 2f);
        }

        AudioSource[] sources = this.gameObject.transform.Find("DestroySounds").GetComponents<AudioSource>();
        sources[UnityEngine.Random.Range(0, sources.Length)].Play((ulong)UnityEngine.Random.Range(0l, 2l));

        if (navAgent != null) {
            this.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
        }
    }

    public virtual void move(Vector3 destination) {
        cancelOrders();
        moveOrdered = true;
        AudioSource[] sources = this.transform.Find("SelectionSounds").GetComponents<AudioSource>();
        AudioSource source = sources[UnityEngine.Random.Range(0, sources.Length)];
        AudioSource.PlayClipAtPoint(source.clip, this.transform.position);

        cancelAndSetDestination(destination);
    }

    /*
    Local calculation of paths sometimes diverge, every 10 seconds or so, update
    the other player's instances of your units with the position as it is in
    your game.
    */

    [PunRPC]
    public virtual void syncPosition(Vector3 position) {
        this.transform.position = position;
    }

    protected virtual void cancelAndSetDestination(Vector3 destination) {
        cancelOrdersRPC();
        setDestination(destination);
    }

    protected virtual void setDestination(Vector3 destination) {
        this.gameObject.GetComponent<NavMeshAgent>().destination = (destination);
    }

    public virtual void callAttack(int idOfThingToAttack) {
        if (!canAttack) {
            return;
        }

        if (photonView.IsMine) {
    	   StartCoroutine(moveAndAttack(idOfThingToAttack));
        }

        AudioSource[] sources = this.gameObject.transform.Find("AttackingSounds").GetComponents<AudioSource>();
        sources[UnityEngine.Random.Range(0, sources.Length)].Play((ulong)UnityEngine.Random.Range(0l, 2l));

    	lastNoEnemies=-1;
    }

    public virtual IEnumerator moveAndAttack(int idOfThingToAttack) {
    	GameObject thingToAttackObj = GameObject.Find(idOfThingToAttack.ToString());
    	if (thingToAttackObj != null) {
    		Attackable thingToAttack = thingToAttackObj.GetComponent<Attackable>();

            if (thingToAttackObj.layer == LayerMask.NameToLayer("town") || thingToAttackObj.layer == LayerMask.NameToLayer("building")) {
                moveOrdered = true;
            }

    	 	if (thingToAttack.hp > 0) {
                attackee = thingToAttack;
                attackeeId = idOfThingToAttack;

                if (thingToAttackObj.tag == "unit") {
                    updateTargetLive = true;
                }

                attackee.attackers.Add(this);
                isAttacking = true;

				setDestination(attackee.gameObject.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position));
				yield return new WaitUntil (() => isInRange(thingToAttack));
                photonView.RPC("stopMovement", RpcTarget.All);

                this.inFight = true;
				InvokeRepeating("attack", 0f, attackRate);
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

        if (navAgent != null) {
            this.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
        }

        this.faceTarget(thingToAttack.transform);
        thingToAttack.takeDamage(this.atk);
    }

    public override void takeDamage(int damage) {
        if (animator != null) {
            animator.SetTrigger("takeDamage");
        }
        base.takeDamage(damage);
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
        CancelInvoke("fireProjectile");
    	this.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
    	this.gameObject.GetComponent<NavMeshAgent>().isStopped = false;
        this.attackeeId = 0;
        isAttacking = false;
        inFight = false;
        attackee = null;
    }

    [PunRPC]
    public void stopMovement() {
        this.gameObject.GetComponent<NavMeshAgent>().isStopped = false;
        this.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
    }

    protected bool isInRange(Attackable thingToAttack) {
        if (thingToAttack == null) {
            return false;
        }

        Vector3 closestPoint = thingToAttack.gameObject.GetComponent<Collider>().ClosestPointOnBounds(this.gameObject.transform.position);

        float deltaX = this.gameObject.transform.position.x - closestPoint.x;
        float deltaZ = this.gameObject.transform.position.z - closestPoint.z; 
        float distance = Mathf.Sqrt(deltaX * deltaX + deltaZ * deltaZ);

        return (distance < this.rng);
    }

    public virtual void checkEnemiesInRange() {
    	if (isAttacking || !canAttack || moveOrdered) {
            return;
        }

	    Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Collider>().bounds.center, responseRange, attackableMask);
        Attackable closestAttackee = null;
        Vector3 playerPos = this.GetComponent<Collider>().bounds.center;

	    if (hitColliders.Length != lastNoEnemies) {
			for (int i = 0; i < hitColliders.Length; i++) {
				if (hitColliders[i].GetComponent<ownership>() != null &&
					hitColliders[i].GetComponent<ownership>().owned == true && 
					hitColliders[i].GetComponent<ownership>().owner != this.gameObject.GetComponent<ownership>().owner &&
                    hitColliders[i].GetComponent<Attackable>().hp > 0) { 

                    UnityEngine.AI.NavMeshAgent agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
                    NavMeshPath path = new NavMeshPath();
                    bool isReachable = agent.CalculatePath(hitColliders[i].transform.position, path);
                    float dist = 0f;
                    for (int j = 0; j < path.corners.Length - 1; j++) {
                        dist += Vector3.Distance(path.corners[j], path.corners[j + 1]);
                    }

                    if (hitColliders[i].gameObject.tag != "wall" && (this.isInRange(hitColliders[i].GetComponent<Attackable>()) || dist < 3f)) {
                        callAttack(hitColliders[i].GetComponent<Attackable>().id);
						break;
                    }
				} 
			}
			lastNoEnemies = hitColliders.Length;
		}
    }

    public virtual void checkEnemiesInRange(float range) {
        if (!isAttacking  || !canAttack) {
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Collider>().bounds.center, range);

        if (hitColliders.Length != lastNoEnemies) {
            for (int i = 0; i < hitColliders.Length; i++) {
                if (hitColliders[i].GetComponent<ownership>() != null &&
                    hitColliders[i].GetComponent<ownership>().owned == true && 
                    hitColliders[i].GetComponent<ownership>().owner != this.gameObject.GetComponent<ownership>().owner &&
                    hitColliders[i].GetComponent<Attackable>().hp > 0) { 

                    UnityEngine.AI.NavMeshAgent agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
                    NavMeshPath path = new NavMeshPath();
                    bool isReachable = agent.CalculatePath(hitColliders[i].transform.position, path);
                    float dist = 0f;
                    for (int j = 0; j < path.corners.Length - 1; j++) {
                        dist += Vector3.Distance(path.corners[j], path.corners[j + 1]);
                    }

                    if (this.isInRange(hitColliders[i].GetComponent<Attackable>()) || dist < 10f ) {
                        callAttack(hitColliders[i].gameObject.GetComponent<Attackable>().id);
                        break;
                    }
                } 
            }

            lastNoEnemies = hitColliders.Length;
        }  
    }

    void OnCollisionEnter(Collision collision) {
        if (this.photonView.IsMine) {
            Attackable collidingUnit = collision.gameObject.GetComponent<Attackable>();
            if (collidingUnit != null && collidingUnit.gameObject.GetComponent<ownership>().owned == true && collidingUnit.gameObject.GetComponent<ownership>().owner != this.gameObject.GetComponent<ownership>().owner && collidingUnit.hp > 0 && !this.inFight) {
                cancelOrders();
                callAttack(collidingUnit.id);
            }
        }
    }

    protected void faceTarget(Transform target) {
        UnityEngine.AI.NavMeshAgent agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (agent != null) {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
        }
    }

    [PunRPC]
    public void updatePosition(Vector3 pos) {
        this.transform.position = pos;
    }

    public override void OnPlayerEnteredRoom(Player other) {
        if (PhotonNetwork.IsMasterClient) {
            this.photonView.RPC("updatePosition", RpcTarget.OthersBuffered, this.transform.position);
        }
    }

    private void baseDestroy() {
        base.destroyObject();
    }

    public virtual void onSelect() {
        isSelected = true;
    }

    public virtual void onDeSelect() {
        isSelected = false;
    }
}