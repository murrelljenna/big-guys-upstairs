using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Barracks : Building, IPunObservable
{
    private Camera playerCamera;

    void Start() {
        prefabName = "Barracks";

        this.woodCost = 75;
        this.foodCost = 0;

        if (!this.photonView.IsMine) {
            this.GetComponent<buildingGhost>().active = false;
        }

        base.Start();
    }

    public override void Awake() {
        base.Awake();
    }

    private Camera getLocalCamera()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PhotonView>().IsMine)
            {
                return players[i].transform.Find("FPSController").transform.Find("FirstPersonCharacter").GetComponent<Camera>();
            }
        }
        return null;
    }

    public override void interactionOptions(game.assets.Player player) {
        info.SetActive(true);

        if (!midAnimation) {
            info.transform.Find("Light Infantry Selector").Find("1_Pressed").gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            int wood = 2; // Please replace with real values soon.
            int food = 10;

            up1 = info.transform.Find("Light Infantry Selector").Find("1_Normal").gameObject;
            down1 = info.transform.Find("Light Infantry Selector").Find("1_Pressed").gameObject;

            up1.SetActive(false);
            down1.SetActive(true);
            midAnimation = true;
            Invoke("releaseButton1", 0.2f);

            if (!player.maxedUnits()) {
                if (player.canAfford(wood, food)) {
                    player.makeTransaction(wood, food);

                    /* Instantiate new militia outside city */

                    Vector2 randomInCircle = RandomPointOnUnitCircle(1.2f);
                    Vector3 spawnLocation = new Vector3(randomInCircle.x+info.transform.position.x, this.transform.position.y, randomInCircle.y+info.transform.position.z);

                    GameObject militia = PhotonNetwork.Instantiate("Light Infantry", spawnLocation, Quaternion.identity, 0);

                    militia.GetComponent<ownership>().capture(player);
                } else {
                    tooltips.flashLackResources();
                }
            } else {
                Debug.Log("PLEASE MAKE TOOLTIP FOR MAX UNITS");
            }
        }
        
        base.interactionOptions(player);
    }
}