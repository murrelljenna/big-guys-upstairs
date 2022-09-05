using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class ResourceTile : Attackable
{	

	ownership ownerInfo;
	int yield = 4;
	public string resType;
    // Start is called before the first frame update

    public override void Update()
    {
        if (playerCamera != null && info != null) {
            info.transform.LookAt(playerCamera.transform);   
        } else {
            playerCamera = getLocalCamera();
        }

        base.Update();
    }

    public override void Start()
    {
        this.woodCost = 15;
        this.foodCost = 15;
        this.hp = 75;
        ownerInfo = this.gameObject.GetComponent<ownership>();
        this.id = this.gameObject.GetComponent<PhotonView>().ViewID;
        this.gameObject.name = id.ToString();

        Transform infoTransform = this.gameObject.transform.Find("Info");
        if (infoTransform != null) {
            if (infoTransform.gameObject != null) {
                info = infoTransform.gameObject;
                info.SetActive(false);
            }
        }

        base.Start();
    }

    // Update is called once per frame

    public override void onCapture() {
        this.transform.Find("Info").Find("1_Normal").Find("Text").GetComponent<Text>().text = '\u2714'.ToString();
        this.transform.Find("Info").Find("1_Pressed").Find("Text").GetComponent<Text>().text = '\u2714'.ToString();
        this.transform.Find("Info").Find("Text").GetComponent<Text>().text = "Captured";
        StartCoroutine(checkOwnerAndCapture());
    }

    public override void onDeCapture() {
        destroyObject();
    }

    private IEnumerator checkOwnerAndCapture() {
        yield return new WaitUntil(() => {
            return (GameObject.Find(ownerInfo.owner.ToString()) != null);
        });
        GameObject.Find(ownerInfo.owner.ToString()).GetComponent<game.assets.Player>().addResource(resType, yield);

        this.gameObject.transform.Find("RegularFlag").GetComponent<Renderer>().material.color = GetComponent<ownership>().playerColor;
    }

    public override void destroyObject() {
        if (attackers.Count > 0) {
            for (int i = attackers.Count - 1; i >= 0; i--) {
                attackers[i].cancelOrders();
            };
        }

        this.gameObject.transform.Find("RegularFlag").GetComponent<Renderer>().material.color = Color.white;

        this.transform.Find("Info").Find("1_Normal").Find("Text").GetComponent<Text>().text = "E";
        this.transform.Find("Info").Find("1_Pressed").Find("Text").GetComponent<Text>().text = "E";
        this.transform.Find("Info").Find("Text").GetComponent<Text>().text = "Capture";

        if (this.photonView.IsMine) {
       		ownerInfo.getPlayer().loseResource(resType, yield);
            
            ownerInfo.owned = false;
            ownerInfo.owner = 0;
            this.hp = this.maxHP;
        }
    }

    public override void takeDamage(int damage) {
        if (this.GetComponent<ownership>().owned) { // Unowned resource tiles should not take damage.
            base.takeDamage(damage);
        }
    }
}
