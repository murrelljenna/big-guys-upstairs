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

        if (this.underConstruction)
        {
            return;
        }

        base.interactionOptions(player);
    }
}