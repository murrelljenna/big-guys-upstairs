using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

using UnityStandardAssets.Characters.FirstPerson;

namespace game.assets {
    public class Player : MonoBehaviourPun, IPunObservable
    {
    	public int wood;
    	public int food;

    	public int woodIt;
    	public int foodIt;

        public int playerID;

        public Color playerColor;

        List<Color> colours = new List<Color>() { Color.black, Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.yellow };

        // Start is called before the first frame update
        void Start()
        {
            wood = 150;
            food = 75;

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

        [PunRPC]
        public void setColour(int colourIndex) {
            Debug.Log(colourIndex);
            playerColor = colours[colourIndex];
            this.gameObject.transform.Find("FPSController").transform.Find("Capsule").GetComponent<Renderer>().material.color = playerColor;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.IsWriting) {
                stream.SendNext(wood);
                stream.SendNext(food);
            }
            else
            {
                wood = (int)stream.ReceiveNext();
                food = (int)stream.ReceiveNext();
            }
        }
    }
}