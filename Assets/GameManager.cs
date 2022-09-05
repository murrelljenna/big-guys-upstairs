using System;
using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using UnityStandardAssets.Characters.FirstPerson;

public class GameManager : MonoBehaviourPunCallbacks
{
	private SpawnManager spawnManager;

	[Tooltip("The prefab to use for representing the player")]
	public GameObject playerPrefab;
	public GameObject cityPrefab;
	public GameObject spawnManagerPrefab;
	GameObject localPlayer;
	GameObject startingCity;

	//Events

	[SerializeField]
	List<Color> colours = new List<Color>() { Color.black, Color.blue, Color.white, Color.green, Color.magenta, Color.red, Color.yellow };
	List<string> colourStrings = new List<string>() { "black", "blue", "white", "green", "pink", "red", "yellow" };

	bool[] coloursAvail = new bool[7];

	[SerializeField]
	List<int> possibleColours;

	GameObject spawnManagerObj;

    // Start is called before the first frame update
    public override void OnConnectedToMaster() {
        PhotonNetwork.LoadLevel("Launcher");
    }

    void Start() {
    	if (playerPrefab == null)
		{
		    Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
		} else {
		    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);

		    if (PhotonNetwork.IsMasterClient) {
		    	ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
		    	roomProperties.Add("coloursAvail", coloursAvail);
		    	PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);

		    	for (int i = 0; i < colours.Count; i++) {
					possibleColours.Add(i);
				}

		    	spawnManager = PhotonNetwork.InstantiateSceneObject(this.spawnManagerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0).GetComponent<SpawnManager>();
		    	GameObject[] resourceSpawners = GameObject.FindGameObjectsWithTag("resourceSpawner");
		    	for (int i = 0; i < resourceSpawners.Length; i++) {
		    		resourceSpawners[i].GetComponent<SpawnTile>().spawnResource();
				}
		    } else {
		    	StartCoroutine(getSpawnManager());
		    }
		}
	}

	void OnEnable() {
		base.OnEnable();
	}

	void OnDisable() {
		base.OnDisable();
	}

    public override void OnPlayerEnteredRoom(Player other) {
    	Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);
	    if (PhotonNetwork.IsMasterClient) {
	
	        Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
	    }
	}

	void OnConnectionFail(DisconnectCause cause) {
		print("FAILED");
	}

    public void LeaveRoom()
    {
    	spawnManager.cleanupSpawns();
        PhotonNetwork.LeaveRoom();
    }

    void LoadArena() {
    	if (!PhotonNetwork.IsMasterClient) {
	        Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
	    }
   		Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
    	PhotonNetwork.LoadLevel("Flat");
	}

	void OnPhotonInstantiate(PhotonMessageInfo info) {
	    if (PhotonNetwork.IsMasterClient) {
	        Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
	    }
	}

	public override void OnPlayerLeftRoom(Player other) {
	    Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

	    if (PhotonNetwork.IsMasterClient) {
	    	GameObject[] woodResources = GameObject.FindGameObjectsWithTag("wood");
			GameObject[] foodResources = GameObject.FindGameObjectsWithTag("food");

			for (int i = 0; i < woodResources.Length; i++) {
				if (woodResources[i].GetComponent<ownership>().owner == Convert.ToInt32(other.CustomProperties["playerid"])) {
					woodResources[i].GetComponent<ownership>().deCapture();
				}
			}

			for (int i = 0; i < foodResources.Length; i++) {
				if (foodResources[i].GetComponent<ownership>().owner == Convert.ToInt32(other.CustomProperties["playerid"])) {
					foodResources[i].GetComponent<ownership>().deCapture();
				}
			}

	        Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
	    }
	}

    public void checkVictory() {
    	GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    	GameObject winner = null;
    	int stillPlaying = 0;

    	for (int i = 0; i < players.Length; i++) {
    		Debug.Log(players[i].name);
    		Debug.Log(players[i].GetComponent<game.assets.Player>().hasLost);
    		if (!players[i].GetComponent<game.assets.Player>().hasLost) {
    			winner = players[i];
    			stillPlaying++;
    		}
    	}

    	if (stillPlaying == 1) {
    		winner.GetComponent<game.assets.Player>().win();
    	}
    }

    private IEnumerator getSpawnManager() {
    	GameObject sm = GameObject.Find("SpawnManager(Clone)");
		if (spawnManager == null) {
			yield return null;		
		} else {
			spawnManager = sm.GetComponent<SpawnManager>();
		}
    }
}
