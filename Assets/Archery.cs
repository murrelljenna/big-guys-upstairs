using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Archery : Building, IPunObservable
{
    void Start() {
        prefabName = "Archery";

        this.hp = 400;
        this.woodCost = 60;
        this.foodCost = 0;

        if (!this.photonView.IsMine) {
            this.GetComponent<buildingGhost>().active = false;
        }

        base.Start();
    }

    public override void Awake() {
        base.Awake();
    }

    public override void interactionOptions(game.assets.Player player) {
        info.SetActive(true);

        if (!midAnimation) {
            info.transform.Find("Archer Selector").Find("1_Pressed").gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            int wood = 5; // Please replace with real values soon.
            int food = 7;

            up1 = info.transform.Find("Archer Selector").Find("1_Normal").gameObject;
            down1 = info.transform.Find("Archer Selector").Find("1_Pressed").gameObject;

            midAnimation = true;
            Invoke("releaseButton1", 0.2f);

            if (!player.maxedUnits()) {
                if (player.canAfford(wood, food)) {
                    player.makeTransaction(wood, food);

                    /* Instantiate new militia outside city */

                    Vector2 randomInCircle = RandomPointOnUnitCircle(1.2f);
                    Vector3 spawnLocation = new Vector3(randomInCircle.x+info.transform.position.x, this.transform.position.y, randomInCircle.y+info.transform.position.z);

                    GameObject militia = PhotonNetwork.Instantiate("Archer", spawnLocation, Quaternion.identity, 0);

                    militia.GetComponent<ownership>().capture(player);
                } else {
                    tooltips.flashLackResources();
                }
            } else {
                
            }

            up1.SetActive(false);
            down1.SetActive(true);
        }

        base.interactionOptions(player);
    }
}