using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
using Photon.Realtime;

public class Gate : Building, IPunObservable
{
    private gateController controller;

    void Start() {
    	prefabName = "Gate";
        
        this.hp = 250;
        this.woodCost = 30;
        this.foodCost = 0;

        Transform infoTransform = this.gameObject.transform.Find("Info");
        if (infoTransform != null) {
            if (infoTransform.gameObject != null) {
                info = infoTransform.gameObject;
                info.SetActive(false);
            }
        }

        controller = this.transform.Find("Entrance").GetComponent<gateController>();

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
        this.transform.Find("Model").Find("Wall_A_gate").gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Buildings_" + colorName) as Texture));
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

    public override void interactionOptions(game.assets.Player player) {
        GameObject buildingViewed = this.gameObject.transform.Find("Info").gameObject;
        buildingViewed.SetActive(true);

        if (!midAnimation) {
            buildingViewed.transform.Find("Lock selector").Find("1_Pressed").gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            up1 = buildingViewed.transform.Find("Lock selector").Find("1_Normal").gameObject;
            down1 = buildingViewed.transform.Find("Lock selector").Find("1_Pressed").gameObject;

            up1.SetActive(false);
            down1.SetActive(true);
            midAnimation = true;
            Invoke("releaseButton1", 0.2f);
            
            if (controller.locked) {
                controller.unlockGate();
            } else {
                controller.lockGate();
            }
        }

        base.interactionOptions(player);
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