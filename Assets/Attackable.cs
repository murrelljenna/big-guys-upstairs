using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Attackable : MonoBehaviourPunCallbacks, IPunObservable
{
	public int hp;
	public int id;
	public List<Unit> attackers;
	public PhotonView photonView;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public virtual void Awake() {
    	this.photonView = this.gameObject.GetComponent<PhotonView>();
        this.id = this.photonView.ViewID;
        this.gameObject.name = id.ToString();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (hp <= 0) {
        	Debug.Log("Object about to be destroyed");
            destroyObject();
        }
    }

    public void takeDamage(int damage) {
    	this.hp -= damage;
    }

    public virtual void destroyObject() {
    	CancelInvoke();
    	if (this.photonView.IsMine) {
        	PhotonNetwork.Destroy(this.gameObject);
        }
    }
 
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(hp);
        }
        else
        {
            hp = (int)stream.ReceiveNext();
        }
    }
}
