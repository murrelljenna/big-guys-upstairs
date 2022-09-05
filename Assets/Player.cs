using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

using UnityStandardAssets.Characters.FirstPerson;

namespace game.assets {
    public class Player : MonoBehaviourPun
    {
    	public int wood;
    	public int food;

    	public int woodIt;
    	public int foodIt;

        public int playerID;

        public Color playerColor;

        // Start is called before the first frame update
        void Start()
        {
            wood = 100;
            food = 100;

            woodIt = 2;
            foodIt = 2;
        }

        void Awake() {

            /* photonView.ViewId is player object's game ID and identifies resources and building ownership */

            playerID = this.transform.Find("FPSController").gameObject.GetComponent<PhotonView>().ViewID;
            this.gameObject.name = playerID.ToString();

            /* Setup player resource iteration */

            InvokeRepeating("iterateResources", 10f, 10f);

            /* Set ownership controls */

            if (!this.transform.Find("FPSController").gameObject.GetComponent<PhotonView>().IsMine) {
                transform.Find("FPSController").gameObject.GetComponent<FirstPersonController>().enabled = false;
                transform.Find("FPSController").gameObject.GetComponent<CharacterController>().enabled = false;
                transform.Find("FPSController").Find("FirstPersonCharacter").GetComponent<Camera>().enabled = false;
                transform.Find("FPSController").Find("FirstPersonCharacter").GetComponent<AudioListener>().enabled = false;
            } else {
                /* If this is THE player, find the GUI and pass reference of this script to UI */
                GameObject.Find("Resources").GetComponent<ResourcePanel>().assignPlayer(this.GetComponent<game.assets.Player>());
            }
        }

        // Update is called once per frame
        void Update()
        {

        }


        void iterateResources() {
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

        public void loseResource(string resType, int yield = 4) {
            switch (resType) {
                case "wood":
                    woodIt -= yield;
                break;
                case "food":
                    foodIt -= yield;
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
}