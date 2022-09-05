using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class launchMissile : MonoBehaviour
{
    public bool activated = false;
    public int dmg;

    public void OnTriggerEnter(Collider collision) {
    	Attackable collidingEnemy = collision.gameObject.GetComponent<Attackable>();
        if (collidingEnemy != null && collidingEnemy.gameObject.GetComponent<ownership>().owner != this.gameObject.GetComponent<ownership>().owner) {
            if (activated) {
                collidingEnemy.photonView.RPC("takeDamageRPC", RpcTarget.AllBuffered, dmg);
            }

            AudioSource[] sources = this.gameObject.GetComponents<AudioSource>();
            sources[UnityEngine.Random.Range(0, sources.Length)].Play((ulong)UnityEngine.Random.Range(0l, 2l));
            Destroy(this.gameObject, 0.2f);
        }
    }
}
