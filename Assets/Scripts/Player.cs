using System;
using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

using UnityStandardAssets.Characters.FirstPerson;
using game.assets.utilities.resources;

namespace game.assets {
    public class Player : MonoBehaviourPunCallbacks, IPunObservable
    {
        public bool hasLost = false;
        public bool hasWon = false;

        public ResourceSet resources = new ResourceSet(100, 100); // This is the literal resources of the player

        public int wood;
        public int food;
        public int gold;
        public int stone;
        public int iron;

        private int maxUnits = 10;
        private int noUnits;

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

        List<string> htmlStrings = new List<string>() {"#95B2FF", "#FF89B8", "#89FFA0", "#FCFF89", "#FF80EB", "#FFFFFF", "#9F9F9F"};
        List<Color> colors = new List<Color>() { Color.blue, Color.red, Color.green, Color.yellow, Color.magenta, Color.white, Color.black, };
        List<string> colourStrings = new List<string>() { "blue", "red", "green", "yellow", "pink", "white", "black" };

        // Start is called before the first frame update
        void Start()
        {
            cityCount = 0;

            wood = 150;
            food = 400;
            stone = 0;
            iron = 0;
        }

        void Awake() {
            if (this.photonView.IsMine) {
                playerName = PhotonNetwork.LocalPlayer.NickName;
            } else {
                transform.Find("FPSController").Find("FirstPersonCharacter").Find("Tools").gameObject.SetActive(false);
                this.transform.Find("Pause Menu").gameObject.SetActive(false);
            }

            /* photonView.ViewId is player object's game ID and identifies resources and building ownership */

            playerID = this.transform.Find("FPSController").gameObject.GetComponent<PhotonView>().ViewID;
            this.gameObject.name = playerID.ToString();

            /* Find player timer in UI for resource collection */

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
            }
        }

        public void addUnit(int count = 1) {
            if (this.photonView.IsMine) {
                noUnits+=count;
                GameObject.Find("Pop_Count").GetComponent<Text>().text = noUnits.ToString();
            }
        }

        public void deposit(ResourceSet yield) {
            this.resources = this.resources + yield;
            
        }

        public void addUnitMax(int count = 1) {
            if (this.photonView.IsMine) {
                maxUnits+=count;
                GameObject.Find("Pop_Max").GetComponent<Text>().text = maxUnits.ToString();
            }
        }

        public bool maxedUnits() {
            return (noUnits >= maxUnits);
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
            if (!this.photonView.IsMine) {
                if (nameTag != null && playerCamera != null) {
                    nameTag.transform.parent.LookAt(playerCamera.transform);
                } else {
                    playerCamera = getLocalCamera();
                }
            }
        }

        public void giveResources(string resType, int yield = 4) {
            switch (resType) {
                case "wood":
                    this.resources.wood += yield;
                break;
                case "food":
                    this.resources.food += yield;
                break;
                case "gold":
                    this.resources.gold += yield;
                break;
                case "stone":
                    this.resources.stone += yield;
                break;
                case "iron":
                    this.resources.iron += yield;
                break;
            }
        }

        public bool canAfford(int wood = 0, int food = 0, int gold = 0, int stone = 0, int iron = 0) {
            if (this.resources.wood >= wood && this.resources.food >= food && this.resources.gold >= gold && this.resources.stone >= stone && this.resources.iron >= iron) {
                return true;
            }

            GameObject resources = GameObject.Find("Resources");

            if (this.resources.wood < wood) {
                StartCoroutine(flashRed(resources.transform.Find("Wood").transform.Find("Resource_Icon").gameObject, 0.2f));
            }

            if (this.resources.food < food) {
                StartCoroutine(flashRed(resources.transform.Find("Food").transform.Find("Resource_Icon").gameObject, 0.2f));
            }

            if (this.resources.gold < gold) {
                StartCoroutine(flashRed(resources.transform.Find("Gold").transform.Find("Resource_Icon").gameObject, 0.2f));
            }
            if (this.resources.stone < stone) {
                StartCoroutine(flashRed(resources.transform.Find("Stone").transform.Find("Resource_Icon").gameObject, 0.2f));
            }
            if (this.resources.iron < iron) {
                StartCoroutine(flashRed(resources.transform.Find("Iron").transform.Find("Resource_Icon").gameObject, 0.2f));
            }
            return false;
        }

        public void makeTransaction(int wood = 0, int food = 0, int gold = 0, int stone = 0, int iron = 0) {
            if (canAfford(wood, food, gold, stone, iron)) {
                this.resources.wood -= wood;
                this.resources.food -= food;
                this.resources.gold -= gold;
                this.resources.stone -= stone;
                this.resources.iron -= iron;
            }
        }

        [PunRPC]
        public void setColour(int colourIndex) {
            string cloudColorHtml = htmlStrings[colourIndex];
            Color cloudColor;
            ColorUtility.TryParseHtmlString(cloudColorHtml, out cloudColor);
            playerColor = colors[colourIndex];
            colorName = colourStrings[colourIndex];
            this.gameObject.transform.Find("FPSController").transform.Find("Capsule").GetComponent<Renderer>().material.color = cloudColor;
            this.hasColor = true;
        }

        [PunRPC]
        private void setName(string name) {
            this.nameTag.GetComponent<UnityEngine.UI.Text>().text = name;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.IsWriting) {
            }
            else
            {
            }
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
