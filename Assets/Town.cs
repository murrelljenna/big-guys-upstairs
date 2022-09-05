using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Town : Attackable, IPunObservable
{
	public int woodCost = 75;
	public int foodCost = 25;

    void Start() {
        this.hp = 40;
    }
    // Start is called before the first frame update
    public override void Awake() {
        base.Awake();
        if (!this.gameObject.GetComponent<PhotonView>() != null && !this.gameObject.GetComponent<PhotonView>().IsMine) {
            this.gameObject.GetComponent<LineRenderer>().enabled = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(this.hp);
        }
        else
        {
            this.hp = (int)stream.ReceiveNext();
        }
    }
}
