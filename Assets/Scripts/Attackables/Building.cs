using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Building : Attackable
{
  protected bool canBeRecycled = true;

    void Start()
    {
    	buildingInteraction interactionOptions = this.GetComponent<buildingInteraction>();
       	if (interactionOptions != null) {
       		interactionOptions.enabled = false;
       	}

       base.Start(); 
    }

    protected static Vector2 RandomPointOnUnitCircle(float radius) {
        float angle = Random.Range (0f, Mathf.PI * 2);
        float x = Mathf.Sin (angle) * radius;
        float y = Mathf.Cos (angle) * radius;

        return new Vector2(x, y);
    }

    public override void destroyObject() {
    	if (this.photonView.IsMine && this.prefabName != "Wall") {
	    	for (int i = 0; i < Random.Range(2, 5); i++) {
		        Vector2 randomInCircle = RandomPointOnUnitCircle(1.2f);
		        Vector3 spawnLocation = new Vector3(randomInCircle.x+this.transform.position.x, 0, randomInCircle.y+this.transform.position.z);

		        GameObject militia = PhotonNetwork.Instantiate("Militia", spawnLocation, Quaternion.identity, 0);

		        militia.GetComponent<ownership>().capture(this.gameObject.GetComponent<ownership>().getPlayer());
		    }
		}

	    base.destroyObject();
    }

    public override void interactionOptions(game.assets.Player player) {
      if (this.photonView.IsMine && canBeRecycled && Input.GetKeyDown(KeyCode.X)) {
        owner.getPlayer().giveResources("wood", this.woodCost/2);
        owner.getPlayer().giveResources("food", this.foodCost/2);
        base.destroyObject();
      }
    }

    /* Called by buildingPlacement.cs. Returns error code interpreted by buildingPlacement.cs explaining reason why can't build */ 
/*
    public int constructionOptions() {
      Debug.Log("Not put in place yet")
    }
    */
}
