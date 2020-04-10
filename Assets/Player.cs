using System;
using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using UnityStandardAssets.Characters.FirstPerson;

namespace game.assets {
    public class Player : MonoBehaviourPunCallbacks, IPunObservable
    {
        public bool hasLost = false;
        public bool hasWon = false;

    	public int wood;
    	public int food;
        public int gold;

    	public int woodIt;
    	public int foodIt;
        public int goldIt;

        public string playerName;
        public int playerID;

        public Color playerColor;
        public string colorName;

        protected Camera playerCamera = null;

        private float counter = 0f;
        private float countMax = 10f;
        private SimpleHealthBar timer;
        public int cityCount;

        public bool hasColor = false;

        private GameObject nameTag = null;
        private GameObject loseNotice = null;
        private GameObject winNotice = null;

        List<Color> colours = new List<Color>() { Color.blue, Color.red, Color.green, Color.yellow, Color.magenta, Color.white, Color.black, };
        List<string> colourStrings = new List<string>() { "blue", "red", "green", "yellow", "pink", "white", "black" };

        // Start is called before the first frame update
        void Start()
        {
            cityCount = 0;

            wood = 150;
            food = 75;

            woodIt = 2;
            foodIt = 2;
        }

        void Awake() {
            if (this.photonView.IsMine) {
                playerName = PhotonNetwork.LocalPlayer.NickName;
            } else {
                transform.Find("FPSController").Find("FirstPersonCharacter").Find("Tools").gameObject.SetActive(false);
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

                loseNotice = GameObject.Find("LoseNotice");
                loseNotice.SetActive(false);

                winNotice = GameObject.Find("WinNotice");
                winNotice.SetActive(false);
            } else {
                playerCamera = getLocalCamera();
            }
        }

        public override void OnPlayerPropertiesUpdate (Photon.Realtime.Player target, ExitGames.Client.Photon.Hashtable changedProps) {
            if (this.GetComponent<PhotonView>().IsMine) {
                GetComponent<PhotonView>().RPC("setColour", RpcTarget.All, Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["color"]));
            }
        }

        public Camera getLocalCamera() {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++) {
                if (players[i].GetComponent<PhotonView>().IsMine) {
                    return players[i].transform.Find("FPSController").transform.Find("FirstPersonCharacter").GetComponent<Camera>();
                }
            }
            return null;
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
            gold += goldIt;
        }

        public void addResource(string resType, int yield = 4) {
    		switch (resType) {
    			case "wood":
    				woodIt += yield;
    			break;
    			case "food":
    				foodIt += yield;
    			break;
                case "gold":
                    goldIt += yield;
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
                case "gold":
                    goldIt -= yield;
                break;
            }
        }

        public bool canAfford(int wood = 0, int food = 0, int gold = 0) {
            if (this.wood >= wood && this.food >= food && this.gold >= gold) {
                return true;
            }

            GameObject resources = GameObject.Find("Resources");

            if (this.wood < wood) {
                StartCoroutine(flashRed(resources.transform.Find("Wood").transform.Find("Resource_Icon").gameObject, 0.2f));
            }

            if (this.food < food) {
                StartCoroutine(flashRed(resources.transform.Find("Food").transform.Find("Resource_Icon").gameObject, 0.2f));
            }

            if (this.gold < gold) {
                StartCoroutine(flashRed(resources.transform.Find("Gold").transform.Find("Resource_Icon").gameObject, 0.2f));
            }

            return false;
        }

        public void makeTransaction(int wood = 0, int food = 0, int gold = 0) {
            if (this.wood >= wood && this.food >= food) {
                this.wood -= wood;
                this.food -= food;
                this.gold -= gold;
            }
        }

        [PunRPC]
        public void setColour(int colourIndex) {
            playerColor = colours[colourIndex];
            colorName = colourStrings[colourIndex];
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

        private IEnumerator flashRed(GameObject building, float offset) {
            UnityEngine.UI.Image renderer = building.GetComponent<UnityEngine.UI.Image>();

            Color previousColor = renderer.material.color;
            renderer.color = Color.red;

            yield return new WaitForSeconds(offset);

            renderer.color = previousColor;
        }

        
        public void loseCity() {
            photonView.RPC("upCityCount", RpcTarget.AllBuffered, -1);
        }

         // RPC so that lose and win are called in each game for each player
        [PunRPC]
        public void upCityCount(int count) {
            cityCount+=count;
            print(cityCount);
            if (cityCount == 0) {
                lose();
                GameObject.Find("Game Manager").GetComponent<GameManager>().checkVictory();
            }
        }

        private void lose() {
            hasLost = true;
            if (this.photonView.IsMine) {
                this.transform.Find("FPSController").transform.Find("FirstPersonCharacter").transform.Find("Tools").gameObject.SetActive(false);
                GameObject.Find("PlayerUI").SetActive(false);

                loseNotice.SetActive(true);

                AudioSource[] sources = this.transform.Find("Audio").transform.Find("FailFare").GetComponents<AudioSource>();

                sources[0].Play(0);
                sources[1].Play(0);
            } 
        }

        public void win() {
            hasWon = true;
            if (this.photonView.IsMine) {
                this.transform.Find("FPSController").transform.Find("FirstPersonCharacter").transform.Find("Tools").gameObject.SetActive(false);
                GameObject.Find("PlayerUI").SetActive(false);

                winNotice.SetActive(true);

                AudioSource[] sources = this.transform.Find("Audio").transform.Find("FanFare").GetComponents<AudioSource>();
                sources[0].Play(0);
                sources[1].Play(0);
            } 
        }
    }
}