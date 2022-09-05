using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Wall : Building, IPunObservable 
{
    public static bool infoViewed = false;

    protected override void Start() {

    	if (prefabName != "Wall_Corner") {
        	prefabName = "Wall";
        }
        
        this.hp = 200;
        this.woodCost = 3;
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
        if (prefabName != "Wall_Corner") { // Static var - only one wall info should  appear at a time.
            infoViewed = true;

            info.SetActive(true);

            if (!midAnimation) {
                info.transform.Find("Gate Selector").Find("1_Pressed").gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                if (owner.getPlayer().canAfford(25)) {
                    List<GameObject> neighbours = new List<GameObject>();



                    up1 = info.transform.Find("Gate Selector").Find("1_Normal").gameObject;
                    down1 = info.transform.Find("Gate Selector").Find("1_Pressed").gameObject;



                    Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.GetComponent<Collider>().bounds.center, 0.2f, (1 << 14));
                    for (int i = 0; i < hitColliders.Length; i++) {
                        Attackable neighbour = hitColliders[i].gameObject.GetComponent<Attackable>();

                        if (neighbour.prefabName == "Wall") {
                            neighbours.Add(neighbour.gameObject);
                        } else {
                            tooltips.flashBuildingBlocked();
                            return;
                        }
                    }
                    
                    // Nothing in way, create gate

                    neighbours.ForEach(neighbour => {
                        PhotonNetwork.Destroy(neighbour);
                    });

                    owner.getPlayer().makeTransaction(25);

                    GameObject gate = PhotonNetwork.Instantiate("Gate", this.transform.position, this.transform.rotation, 0);
                    gate.GetComponent<ownership>().capture(player);
                } else {
                    tooltips.flashLackResources();
                }
            }
        }

        base.interactionOptions(player);
    }
}