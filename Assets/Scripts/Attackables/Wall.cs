using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Wall : Building, IPunObservable
{
    void Start() {
    	if (prefabName != "Wall_Corner") {
        	prefabName = "Wall";
        }
        
        this.hp = 200;
        this.woodCost = 3;
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

}
