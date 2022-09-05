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
    public Color playerColor;
    
    // Start is called before the first frame upda
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void capture(game.assets.Player player) {
        this.playerColor = player.playerColor;
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
        GameObject player = GameObject.Find(playerID.ToString());
        if (player != null) {
            this.playerColor = player.GetComponent<game.assets.Player>().playerColor;
            this.gameObject.GetComponent<Attackable>().onCapture();
            Debug.Log("CAPTURE");
            this.gameObject.GetComponent<Renderer>().material.color = player.GetComponent<game.assets.Player>().playerColor;
            owned = true;
            this.owner = playerID;
        }
    }
}
