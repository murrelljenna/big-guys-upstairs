using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class SpawnWoodTile : SpawnTile {
    public override void spawnResource() {
        PhotonNetwork.Instantiate("Resource_Wood", this.transform.position, Quaternion.identity, 0);
    }
}

