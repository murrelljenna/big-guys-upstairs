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

    public AudioClip[] plopSounds;
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

        /* Play effects and audio */
        this.transform.Find("Dust").GetComponent<ParticleSystem>().Emit(30);
        AudioSource.PlayClipAtPoint(plopSounds[Random.Range(0, plopSounds.Length - 1)], this.transform.position);

        base.Start();
    }

    public override void onCapture() {
        string colorName = GetComponent<ownership>().getPlayer().colorName;
        this.transform.Find("Model").Find("Wall_A_gate").gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Buildings_" + colorName) as Texture));
    }

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
                photonView.RPC("unlockGate", RpcTarget.AllBuffered);
            } else {
                photonView.RPC("lockGate", RpcTarget.AllBuffered);
            }
        }

        base.interactionOptions(player);
    }

    [PunRPC] 
    private void lockGate() {
        controller.lockGate();
    }

    [PunRPC]
    private void unlockGate() {
        controller.unlockGate();
    }
}