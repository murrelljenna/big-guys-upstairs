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

        public string playerName;
        public int playerID;

        public Color playerColor;

        protected Camera playerCamera = null;

        private float counter = 0f;
        private float countMax = 10f;
        private SimpleHealthBar timer;

        public bool hasColor = false;


        private GameObject nameTag = null;

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
            if (this.photonView.IsMine) {
                playerName = PhotonNetwork.LocalPlayer.NickName;
            }

            /* photonView.ViewId is player object's game ID and identifies resources and building ownership */

            playerID = this.transform.Find("FPSController").gameObject.GetComponent<PhotonView>().ViewID;
            this.gameObject.name = playerID.ToString();

            /* Find player timer in UI for resource collection */

            timer = GameObject.Find("TimerBar").GetComponent<SimpleHealthBar>();

            timer.UpdateBar(0, 10);

            /* Setup player resource iteration */

            InvokeRepeating("iterateResources", 10f, 10f);

            if (this.GetComponent<PhotonView>().IsMine) {
                InvokeRepeating("iterateClock", 1f, 1f);
            }

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

            nameTag = this.transform.Find("FPSController").transform.Find("Name").transform.Find("PlayerName").gameObject;

            if (this.photonView.IsMine) {
                photonView.RPC("setName", RpcTarget.AllBuffered, playerName);
                nameTag.SetActive(false);
                nameTag = null;
            } else {
                playerCamera = GameObject.Find("FirstPersonCharacter").GetComponent<Camera>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (nameTag != null && playerCamera != null) {
                nameTag.transform.parent.LookAt(playerCamera.transform);
            }
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
            playerColor = colours[colourIndex];
            this.gameObject.transform.Find("FPSController").transform.Find("Capsule").GetComponent<Renderer>().material.color = playerColor;
            this.hasColor = true;
        }

        [PunRPC]
        private void setName(string name) {
            this.nameTag.GetComponent<UnityEngine.UI.Text>().text = name;
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

        private void iterateClock() {
            if (counter == countMax - 1f) {
                counter = 0f;
            } else {
                counter++;
            }
            timer.UpdateBar(counter, countMax);
        }
    }
}