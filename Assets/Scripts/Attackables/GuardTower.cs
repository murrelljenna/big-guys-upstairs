using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine.UI;
using UnityEngine.Events;

public class GuardTower : Unit
{
    public GameObject missile;

    protected int upgradeLevel = 1;
    protected int maxUpgrade = 3;
    private GameObject info;
    private int upgradeCostStone = 20;

    void Start()
    {
    	this.movable = false;
    	prefabName = "GuardTower";
    	responseRange = 4f;
        this.woodCost = 40;
        this.foodCost = 0;

        this.atk = 3;
        this.hp = 125;
        this.lastHP = this.hp;
        this.rng = 4f;

        info = transform.Find("Info").gameObject;

        info.transform.Find("2_Pressed").gameObject.SetActive(false);
        info.transform.Find("upgrade_cost").GetComponent<Text>().text = upgradeCostStone.ToString();
        info.SetActive(false);

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

        if (playerCamera != null && info.active == true) {
            info.transform.LookAt(playerCamera.transform);   
        } else {
            playerCamera = getLocalCamera();
        }

        base.Update();
    }

    public override void onCapture() {
    	string colorName = GetComponent<ownership>().getPlayer().colorName;
        this.transform.Find("Model").gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Buildings_" + colorName) as Texture));

        Transform model2 = this.transform.Find("Model2");

        if (model2 != null) {
            model2.gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Buildings_" + colorName) as Texture));
            model2.gameObject.SetActive(false);
        }

        Transform model3 = this.transform.Find("Model3");

        if (model3 != null) {
            model3.gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Buildings_" + colorName) as Texture));
            model3.gameObject.SetActive(false);
        }
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
                attackeeId = attackee.id;
				isAttacking = true;

				yield return new WaitUntil (() => isInRange(thingToAttack));

				InvokeRepeating("attack", 1f, 1f);
			}
		}
    }

    public override void callAttack(int idOfThingToAttack) {
        if (this.photonView.IsMine) {
        	if (canAttack) {
                StartCoroutine(moveAndAttack(idOfThingToAttack));
    	    	lastNoEnemies=-1;
    	    }
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
        attackeeId = 0;
    }

    public override void attack() {
        if (attackee != null) {
            GameObject arrow = Instantiate(missile, GetComponent<Renderer>().bounds.center, Quaternion.LookRotation((attackee.gameObject.transform.position - GetComponent<Renderer>().bounds.center).normalized));
            arrow.transform.Rotate(-90, 0, 0); // Can't figure out how to get this fucking thing to face the right way.
            arrow.GetComponent<ownership>().localCapture(GetComponent<ownership>().getPlayer());
            arrow.GetComponent<Rigidbody>().AddForce((attackee.gameObject.transform.position - this.transform.position).normalized * 500);
        }

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

    public override void destroyObject() {
        photonView.RPC("playDestructionEffect", RpcTarget.All);

        base.destroyObject();
    }
    
    public override void checkEnemiesInRange() {
        if (!isAttacking && canAttack) {
            Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Collider>().bounds.center, responseRange, attackableMask);
            Attackable closestAttackee = null;
            Vector3 playerPos = this.GetComponent<Collider>().bounds.center;

            if (hitColliders.Length != lastNoEnemies) {
                for (int i = 0; i < hitColliders.Length; i++) {
                    if (hitColliders[i] != null) {
                        if (hitColliders[i].GetComponent<ownership>() != null &&
                            hitColliders[i].GetComponent<ownership>().owned == true && 
                            hitColliders[i].GetComponent<ownership>().owner != this.gameObject.GetComponent<ownership>().owner &&
                            hitColliders[i].GetComponent<Attackable>().hp > 0) { 

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

    public override void interactionOptions(game.assets.Player player) {
        GameObject towerViewed = this.transform.Find("Info").gameObject;
        towerViewed.SetActive(true);

        if (!midAnimation) {
            towerViewed.transform.Find("2_Pressed").gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.U)) {
            up2 = towerViewed.transform.Find("2_Normal").gameObject;
            down2 = towerViewed.transform.Find("2_Pressed").gameObject;

            up2.SetActive(false);
            down2.SetActive(true);
            midAnimation = true;
            Invoke("releaseButton2", 0.2f);

            if (player.canAfford(0, 0, 0, 0, 0) && this.upgradeLevel < this.maxUpgrade){
                photonView.RPC("upgrade", RpcTarget.AllBuffered);
            } else {
                tooltips.flashLackResources();
            }
        } 

        base.interactionOptions(player);
    }


    [PunRPC]
    public void upgrade() {
        game.assets.Player player = owner.getPlayer();
        this.transform.Find("Model").gameObject.SetActive(false);
        this.transform.Find("Model2").gameObject.SetActive(false);
        this.transform.Find("Model3").gameObject.SetActive(false);

        Transform model = this.transform.Find("Model" + (upgradeLevel + 1).ToString());
        if (model != null) {
            model.gameObject.SetActive(true);
        }
        maxHP+=(maxHP/2);
        hp=maxHP;
        atk++;

        upgradeLevel++;
    }

    public override void checkEnemiesInRange(float range) {
        if (!isAttacking) {
            Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Collider>().bounds.center, range);

            if (hitColliders.Length != lastNoEnemies) {
                for (int i = 0; i < hitColliders.Length; i++) {
                    if (hitColliders[i] != null) {
                        if (hitColliders[i].GetComponent<ownership>() != null &&
                            hitColliders[i].GetComponent<ownership>().owned == true && 
                            hitColliders[i].GetComponent<ownership>().owner != this.gameObject.GetComponent<ownership>().owner &&
                            hitColliders[i].GetComponent<Attackable>().hp > 0) { 

                            if (this.isInRange(hitColliders[i].GetComponent<Attackable>())) {
                                callAttack(hitColliders[i].gameObject.GetComponent<Attackable>().id);
                                break;
                            }
                        } 
                    }
                }

                lastNoEnemies = hitColliders.Length;
            }
        }  
    }

    [PunRPC]
    private void playDestructionEffect() {
        AudioSource[] sources = this.transform.Find("DestroySounds").GetComponents<AudioSource>();
        AudioSource source = sources[UnityEngine.Random.Range(0, sources.Length)];
        AudioSource.PlayClipAtPoint(source.clip, this.transform.position);

        GameObject explosion = Instantiate(Resources.Load("FX_Building_Destroyed_mid") as GameObject);
        explosion.transform.position = this.transform.position;
        ParticleSystem effect = explosion.GetComponent<ParticleSystem>();
        effect.Play();
        Destroy(effect.gameObject, effect.duration);
    }
}
