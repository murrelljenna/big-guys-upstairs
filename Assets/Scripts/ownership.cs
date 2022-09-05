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

    public void localCapture(game.assets.Player player) {
        this.owned = true;
        this.owner = player.playerID;
    }

    public void capture(game.assets.Player player) {
        this.playerColor = player.playerColor;
        PhotonView photonView = this.gameObject.GetComponent<PhotonView>();
        photonView.RPC("captureRPC", RpcTarget.AllBuffered, player.playerID);
    }

    public void deCapture() {
        this.gameObject.GetComponent<PhotonView>().RPC("deCaptureRPC", RpcTarget.AllBuffered);
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

    public game.assets.Player getPlayer() {
        return (GameObject.Find(this.owner.ToString()).GetComponent<game.assets.Player>());
    }

    private IEnumerator waitForPlayerColour(GameObject player) {
        yield return new WaitUntil(() => {
            return (getPlayer().hasColor != false);
        });
        
        this.playerColor = player.GetComponent<game.assets.Player>().playerColor;
        this.gameObject.GetComponent<Renderer>().material.color = playerColor;
        this.gameObject.GetComponent<Attackable>().onCapture(); // Callback function that is overriden by various classes to respond to capture.
    }

    [PunRPC] public void captureRPC(int playerID, PhotonMessageInfo info) {
        GameObject player = GameObject.Find(playerID.ToString());
        if (player != null) {
            owned = true;
            this.owner = playerID;

            StartCoroutine(waitForPlayerColour(player));
        }
    }

    [PunRPC] public void deCaptureRPC() {
        this.gameObject.GetComponent<Attackable>().onDeCapture();
        owned = false;
        owner = 0;
        playerColor = new Color (0, 255, 255, 255);
    }
}
