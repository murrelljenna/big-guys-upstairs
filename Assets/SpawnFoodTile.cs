using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class SpawnFoodTile : SpawnTile
{
    public override void spawnResource() {
    	if (PhotonNetwork.IsMasterClient) {
        	PhotonNetwork.InstantiateSceneObject("Resource_Food", this.transform.position, Quaternion.identity, 0);
        }
    }
}
