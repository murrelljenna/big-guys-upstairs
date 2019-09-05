using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

using UnityStandardAssets.Characters.FirstPerson;

public class res : MonoBehaviourPun
{
	public int wood;
	public int food;

	public int woodIt;
	public int foodIt;

    // Start is called before the first frame update
    void Start()
    {
        wood = 100;
        food = 100;

        woodIt = 2;
        foodIt = 2;
    }

    void Awake() {

        /* Set ownership controls */

        if (!this.transform.Find("FPSController").gameObject.GetComponent<PhotonView>().IsMine) {
            transform.Find("FPSController").gameObject.GetComponent<FirstPersonController>().enabled = false;
            transform.Find("FPSController").gameObject.GetComponent<CharacterController>().enabled = false;
            transform.Find("FPSController").Find("FirstPersonCharacter").GetComponent<Camera>().enabled = false;
            transform.Find("FPSController").Find("FirstPersonCharacter").GetComponent<AudioListener>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void iterate() {
    	wood += woodIt;
        food += foodIt;
    }

    public void addResource(string resType, int yield = 4) {
		switch (resType) {
			case "wood":
				woodIt += yield;
			break;
			case "food":
				foodIt += yield;
			break;
		}
    }

    public bool canAfford(int wood = 0, int food = 0) {
        if (this.wood >= wood && this.food >= food) {
            return true;
        }

        return false;
    }

    public void makeTransaction(int wood = 0, int food = 0) {
        if (this.wood >= wood && this.food >= food) {
            this.wood -= wood;
            this.food -= food;
        }
    }
}
