using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Archery : Building, IPunObservable
{
    void Start() {
        prefabName = "Archery";

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

            if (Input.GetKeyDown(KeyCode.U)) {
                up2 = info.transform.Find("Upgrade").Find("2_Normal").gameObject;
                down2 = info.transform.Find("2_Pressed").gameObject;

                up2.SetActive(false);
                down2.SetActive(true);
                midAnimation = true;
                Invoke("releaseButton2", 0.2f);

                if (player.canAfford(upgradeCostWood, upgradeCostFood, upgradeCostGold, upgradeCostStone, upgradeCostIron) && this.upgradeLevel < this.maxUpgrade){
                    player.makeTransaction(upgradeCostWood, upgradeCostFood, upgradeCostGold, upgradeCostStone, upgradeCostIron);
                    photonView.RPC("upgrade", RpcTarget.AllBuffered);
                } else {
                    tooltips.flashLackResources();
                }
            } else {
                tooltips.flashLackResources();
            }

            up1.SetActive(false);
            down1.SetActive(true);
        }

        base.interactionOptions(player);
    }

    [PunRPC]
    public override void upgrade() {
        game.assets.Player player = owner.getPlayer();

        maxHP+=(maxHP/2);
        hp=maxHP;
        base.upgrade();
    }
}