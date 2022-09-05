using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Barracks : Building, IPunObservable
{
    void Start() {
        prefabName = "Barracks";

        this.hp = 500;
        this.woodCost = 75;
        this.foodCost = 0;

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
        string colorName = GetComponent<ownership>().getPlayer().colorName;
        this.transform.Find("Model").gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Buildings_" + colorName) as Texture));
    }

    public override void destroyObject() {
        if (this.photonView.IsMine) {
            photonView.RPC("playDestructionEffect", RpcTarget.All);
        }

        base.destroyObject();
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

    public override void interactionOptions(game.assets.Player player) {
        GameObject buildingViewed = this.gameObject.transform.Find("Info").gameObject;
        buildingViewed.SetActive(true);

        if (!midAnimation) {
            buildingViewed.transform.Find("Light Infantry Selector").Find("1_Pressed").gameObject.SetActive(false);
            buildingViewed.transform.Find("Archer Selector").Find("2_Pressed").gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            int wood = 2; // Please replace with real values soon.
            int food = 10;

            up1 = buildingViewed.transform.Find("Light Infantry Selector").Find("1_Normal").gameObject;
            down1 = buildingViewed.transform.Find("Light Infantry Selector").Find("1_Pressed").gameObject;

            up1.SetActive(false);
            down1.SetActive(true);
            midAnimation = true;
            Invoke("releaseButton1", 0.2f);

            if (!player.maxedUnits()) {
                if (player.canAfford(wood, food)) {
                    player.makeTransaction(wood, food);

                    /* Instantiate new militia outside city */

                    Vector2 randomInCircle = RandomPointOnUnitCircle(1.2f);
                    Vector3 spawnLocation = new Vector3(randomInCircle.x+buildingViewed.transform.position.x, 0, randomInCircle.y+buildingViewed.transform.position.z);

                    GameObject militia = PhotonNetwork.Instantiate("Light Infantry", spawnLocation, Quaternion.identity, 0);

                    militia.GetComponent<ownership>().capture(player);
                } else {
                    tooltips.flashLackResources();
                }
            } else {

            }
        } else if (Input.GetKeyDown(KeyCode.R)) {
            int wood = 5; // Please replace with real values soon.
            int food = 7;

            up2 = buildingViewed.transform.Find("Archer Selector").Find("2_Normal").gameObject;
            down2 = buildingViewed.transform.Find("Archer Selector").Find("2_Pressed").gameObject;

            midAnimation = true;
            Invoke("releaseButton2", 0.2f);

            if (!player.maxedUnits()) {
                if (player.canAfford(wood, food)) {
                    player.makeTransaction(wood, food);

                    /* Instantiate new militia outside city */

                    Vector2 randomInCircle = RandomPointOnUnitCircle(1.2f);
                    Vector3 spawnLocation = new Vector3(randomInCircle.x+buildingViewed.transform.position.x, 0, randomInCircle.y+buildingViewed.transform.position.z);

                    GameObject militia = PhotonNetwork.Instantiate("Archer", spawnLocation, Quaternion.identity, 0);

                    militia.GetComponent<ownership>().capture(player);
                } else {
                    tooltips.flashLackResources();
                }
            } else {
                
            }

            up2.SetActive(false);
            down2.SetActive(true);
        }
    }
}
