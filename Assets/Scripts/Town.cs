 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Town : Building, IPunObservable
{
    private int yield = 1;
    private int lastNoEnemies = 0;

    private bool underAttack = false;

    int resourceMask = 1 << 9;
    int unitMask = 1 << 12;

    void Start() {
        prefabName = "Town";

        this.hp = 600;
        this.woodCost = 50;
        this.foodCost = 50;

        Transform infoTransform = this.gameObject.transform.Find("Info");
        if (infoTransform != null) {
            if (infoTransform.gameObject != null) {
                info = infoTransform.gameObject;
                info.SetActive(false);
            }
        }

        if (!this.photonView.IsMine) {
            this.GetComponent<buildingGhost>().active = false;
        }

        InvokeRepeating("checkEnemiesInRadius", 2f, 2f);

        base.Start();
    }

    public override void Update() {
        if (playerCamera != null && info.active == true) {
            info.transform.LookAt(playerCamera.transform);   
        } else {
            playerCamera = getLocalCamera();
        }

        base.Update();
    }

    public override void onCapture() {
        game.assets.Player player = owner.getPlayer();
        player.addResource("gold", yield);
        string colorName = GetComponent<ownership>().getPlayer().colorName;
        this.transform.Find("Model").gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Buildings_" + colorName) as Texture));
        this.gameObject.GetComponent<LineRenderer>().SetColors(GetComponent<ownership>().playerColor, GetComponent<ownership>().playerColor);
        this.gameObject.GetComponent<ownership>().getPlayer().upCityCount(1);
    }

    private void checkEnemiesInRadius() {
        if (canAttack) {
            Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Renderer>().bounds.center, 10f, unitMask);
            int attackers = 0;

            if (hitColliders.Length != lastNoEnemies) {
                for (int i = 0; i < hitColliders.Length; i++) {
                    if (hitColliders[i] != null && hitColliders[i].tag != "buildingGhost" && hitColliders[i].GetComponent<ownership>().owner != this.gameObject.GetComponent<ownership>().owner) {
                        if (underAttack == false) {
                            // Being attacked first time - play noise without being annoying.
                            this.gameObject.GetComponent<AudioSource>().Play(0);
                        }

                        attackers+=1;
                        underAttack = true;
                        callForHelp();
                        break;
                    }
                }

                if (attackers < 1) {
                    underAttack = false;
                }
            }
        }
    }

    private void callForHelp() {
        Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Renderer>().bounds.center, 10f, unitMask);

        for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders[i] != null && hitColliders[i].GetComponent<ownership>().owner == this.gameObject.GetComponent<ownership>().owner && hitColliders[i].GetComponent<Unit>().isAttacking == false) {
                hitColliders[i].GetComponent<Unit>().checkEnemiesInRange(10f);
            }
        }
    }

    public override void destroyObject() {
        if (this.photonView.IsMine) {
            game.assets.Player player = owner.getPlayer();
            player.loseResource("gold", yield);

            Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Renderer>().bounds.center, 10f, resourceMask);

            if (hitColliders.Length != lastNoEnemies) {
            for (int i = 0; i < hitColliders.Length; i++) {
                    if (hitColliders[i].GetComponent<ownership>() != null && hitColliders[i].GetComponent<ownership>().owned == true) {
                        hitColliders[i].gameObject.GetComponent<ownership>().deCapture();
                    }
                }
            }

            this.gameObject.GetComponent<ownership>().getPlayer().loseCity();

            photonView.RPC("playDestructionEffect", RpcTarget.All);
        }

        base.destroyObject();
    }

    public override void interactionOptions(game.assets.Player player) {
        GameObject cityViewed = this.transform.Find("Info").gameObject;
        cityViewed.SetActive(true);

        if (!midAnimation) {
            cityViewed.transform.Find("1_Pressed").gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            if (!player.maxedUnits()) {
                int wood = 0; // Please replace with real values soon.
                int food = 5;

                up1 = cityViewed.transform.Find("1_Normal").gameObject;
                down1 = cityViewed.transform.Find("1_Pressed").gameObject;

                up1.SetActive(false);
                down1.SetActive(true);
                midAnimation = true;
                Invoke("releaseButton1", 0.2f);

                if (player.canAfford(wood, food)) {
                    player.makeTransaction(wood, food);

                    /* Instantiate new militia outside city */

                    Vector2 randomInCircle = RandomPointOnUnitCircle(1.2f);
                    Vector3 spawnLocation = new Vector3(randomInCircle.x+this.transform.position.x, this.transform.position.y, randomInCircle.y+this.transform.position.z);

                    GameObject militia = PhotonNetwork.Instantiate("Militia", spawnLocation, Quaternion.identity, 0);

                    militia.GetComponent<ownership>().capture(player);
                } else {
                    tooltips.flashLackResources();
                }
            } else {
                
            }
        } 
    }

    public override void takeDamage(int damage) {
        AudioSource[] sources = this.transform.Find("DamageSounds").GetComponents<AudioSource>();
        sources[UnityEngine.Random.Range(0, sources.Length)].Play((ulong)UnityEngine.Random.Range(0l, 2l));

        base.takeDamage(damage);
    }

    // Start is called before the first frame update
    public override void Awake() {
        base.Awake();
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
