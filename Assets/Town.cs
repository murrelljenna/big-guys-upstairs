using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Town : MonoBehaviour
{
	public int woodCost = 75;
	public int foodCost = 25;
    // Start is called before the first frame update
    void Awake() {
        if (!this.gameObject.GetComponent<PhotonView>() != null && !this.gameObject.GetComponent<PhotonView>().IsMine) {
            this.gameObject.GetComponent<LineRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
