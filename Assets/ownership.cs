using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class ownership : MonoBehaviourPunCallbacks, IPunObservable
{
	public bool owned = false;
    [SerializeField]
    public int owner;
    // Start is called before the first frame upda
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void capture(game.assets.Player player) {
        PhotonView photonView = this.gameObject.GetComponent<PhotonView>();
        photonView.RPC("captureRPC", RpcTarget.All, player.playerID);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            // We own this player: send the others our data
            stream.SendNext(owner);
            stream.SendNext(owned);
        }
        else
        {
            // Network player, receive data
            owner = (int)stream.ReceiveNext();
            owned = (bool)stream.ReceiveNext();
        }
    }

    [PunRPC] public void captureRPC(int playerID, PhotonMessageInfo info) {
        owned = true;
        this.owner = playerID;
    }
}
