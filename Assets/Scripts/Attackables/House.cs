using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class House : Building, IPunObservable
{
    private int housingBump = 5;

    void Start() {
        prefabName = "House";

        this.woodCost = 20;
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

    public override void build() {
        base.build();

        if (photonView.IsMine) {
            owner.getPlayer().addUnitMax(housingBump);
        }
    }

    public override void onCapture() {     
        base.onCapture();
    }

    public override void destroyObject() {
        if (photonView.IsMine) {
            owner.getPlayer().addUnitMax(-housingBump);
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
        base.interactionOptions(player);
    }
}