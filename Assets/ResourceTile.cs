﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class ResourceTile : Attackable
{	
	ownership ownerInfo;
	int yield = 4;
	public string resType;
    // Start is called before the first frame update
    public virtual void Start()
    {
        this.woodCost = 20;
        this.hp = 75;
        ownerInfo = this.gameObject.GetComponent<ownership>();
        this.id = this.gameObject.GetComponent<PhotonView>().ViewID;
        this.gameObject.name = id.ToString();
    }

    // Update is called once per frame

    public override void onCapture() {
        GameObject.Find(ownerInfo.owner.ToString()).GetComponent<game.assets.Player>().addResource(resType, yield);

        this.gameObject.transform.Find("RegularFlag").GetComponent<Renderer>().material.color = GetComponent<ownership>().playerColor;
    }

    public override void onDeCapture() {
        destroyObject();
    }

    public override void destroyObject() {
        for (int i = attackers.Count - 1; i >= 0; i--) {
            attackers[i].cancelOrders();
        };

        this.gameObject.transform.Find("RegularFlag").GetComponent<Renderer>().material.color = new Color(0, 255, 255, 255);
   		GameObject.Find(ownerInfo.owner.ToString()).GetComponent<game.assets.Player>().loseResource(resType, yield);
        
        ownerInfo.owned = false;
        ownerInfo.owner = 0;
        this.hp = 25;
    }
}
