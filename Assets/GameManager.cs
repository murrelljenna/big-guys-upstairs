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
	[Tooltip("The prefab to use for representing the player")]
	public GameObject playerPrefab;
	public GameObject cityPrefab;
	GameObject localPlayer;
	GameObject startingCity;

	[SerializeField]
	List<Color> colours = new List<Color>() { Color.black, Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.yellow };
	[SerializeField]
	List<int> possibleColours;

	List<Vector3> spawns = new List<Vector3>() { new Vector3(14.54f, 0f, 13.89f), new Vector3(13.9f, 0f, 108.4f), new Vector3(108.03f, 0f, 108.4f), new Vector3(108.41f, 0f, 16.36f) };

    // Start is called before the first frame update
    public override void OnLeftRoom() {
        SceneManager.LoadScene(0);
    }

    void Start() {
    	if (playerPrefab == null)
		{
		    Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
		}
		else
		{
		    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);

		    if (PhotonNetwork.IsMasterClient) {
		    	GameObject[] resourceSpawners = GameObject.FindGameObjectsWithTag("resourceSpawner");

		    	for (int i = 0; i < resourceSpawners.Length; i++) {
		    		resourceSpawners[i].GetComponent<SpawnTile>().spawnResource();
				}

				for (int i = 0; i < colours.Count; i++) {
					possibleColours.Add(i);
				}

	    		int index = UnityEngine.Random.Range(0, possibleColours.Count);
	    		Debug.Log(index);

				ExitGames.Client.Photon.Hashtable playerColour = new ExitGames.Client.Photon.Hashtable();
		    	playerColour.Add("color", possibleColours[index].ToString());
		    	possibleColours.RemoveAt(index);

		    	PhotonNetwork.SetPlayerCustomProperties(playerColour);
		    }
		    
		    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate

		    Vector3 spawn = spawns[PhotonNetwork.CurrentRoom.PlayerCount - 1];
		    Vector2 randomInCircle = RandomPointOnUnitCircle(3f);
        	Vector3 spawnLocation = new Vector3(randomInCircle.x+spawn.x, 0, randomInCircle.y+spawn.z);

			localPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, spawnLocation, Quaternion.identity, 0);
		    localPlayer.GetComponent<game.assets.Player>().playerColor = colours[Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["color"])];

		    ExitGames.Client.Photon.Hashtable playerSettings = new ExitGames.Client.Photon.Hashtable();
		    playerSettings.Add("playerid", localPlayer.GetComponent<game.assets.Player>().playerID.ToString());
		   	PhotonNetwork.SetPlayerCustomProperties(playerSettings);

		   	localPlayer.GetComponent<PhotonView>().RPC("setColour", RpcTarget.AllBuffered, Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["color"]));

		   	startingCity = PhotonNetwork.Instantiate(this.cityPrefab.name, spawn, Quaternion.identity, 0);
		   	startingCity.GetComponent<ownership>().capture(localPlayer.GetComponent<game.assets.Player>());
	    	//localPlayer.transform.Find("FPSController").transform.Find("Capsule").GetComponent<MeshRenderer>().material.color = colours[Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["color"])];
		}
    }

    void OnEnable() {

    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    void LoadArena() {
    	if (!PhotonNetwork.IsMasterClient) {
	        Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
	    }
   		Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
    	PhotonNetwork.LoadLevel("Flat");
	}

	public override void OnPlayerPropertiesUpdate (Player target, ExitGames.Client.Photon.Hashtable changedProps) {
		Debug.Log("LOCAL PLAYER");
		Debug.Log(localPlayer);
		if (localPlayer != null) {
			Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["color"]);

			localPlayer.GetComponent<PhotonView>().RPC("setColour", RpcTarget.All, Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["color"]));
			/*
	    	localPlayer.GetComponent<game.assets.Player>().playerColor = colours[Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["color"])];
	    	localPlayer.transform.Find("FPSController").transform.Find("Capsule").GetComponent<MeshRenderer>().material.color = colours[Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["color"])];
			*/
		}
	}

	public override void OnMasterClientSwitched(Player newMasterClient) {
		// Transfer all resources
		/*if (PhotonNetwork.IsMasterClient) {
			GameObject[] woodResources = GameObject.FindGameObjectsWithTag("wood");
			GameObject[] foodResources = GameObject.FindGameObjectsWithTag("food");

			for (int i = 0; i < woodResources.Length; i++) {
				woodResources[i].GetComponent<PhotonView>().TransferOwnership(newMasterClient);
			}

			for (int i = 0; i < foodResources.Length; i++) {
				foodResources[i].GetComponent<PhotonView>().TransferOwnership(newMasterClient);
			}
		}*/
	}

	public override void OnPlayerEnteredRoom(Player other) {
    	Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);

	    if (PhotonNetwork.IsMasterClient) {
	    	int index = UnityEngine.Random.Range(0, possibleColours.Count);

	    	ExitGames.Client.Photon.Hashtable playerSettings = new ExitGames.Client.Photon.Hashtable();
	    	playerSettings.Add("color", possibleColours[index].ToString());
	    	possibleColours.RemoveAt(index);

	    	other.SetCustomProperties(playerSettings);

	        Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
	    }
	}

	void OnPhotonInstantiate(PhotonMessageInfo info) {
    	localPlayer.GetComponent<PhotonView>().RPC("setColour", RpcTarget.All, Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["color"]));

	    if (PhotonNetwork.IsMasterClient) {
	        Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
	    }
	}

	public override void OnPlayerLeftRoom(Player other) {
	    Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

	    if (PhotonNetwork.IsMasterClient) {
	    	GameObject[] woodResources = GameObject.FindGameObjectsWithTag("wood");
			GameObject[] foodResources = GameObject.FindGameObjectsWithTag("food");

			Debug.Log(foodResources.Length);

			for (int i = 0; i < woodResources.Length; i++) {
				Debug.Log(i);
				if (woodResources[i].GetComponent<ownership>().owner == Convert.ToInt32(other.CustomProperties["playerid"])) {
					woodResources[i].GetComponent<ownership>().deCapture();
				}
			}

			for (int i = 0; i < foodResources.Length; i++) {
				Debug.Log(foodResources[i]);
				if (foodResources[i].GetComponent<ownership>().owner == Convert.ToInt32(other.CustomProperties["playerid"])) {
					foodResources[i].GetComponent<ownership>().deCapture();
				}
			}

	        Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
	    }
	}

	private static Vector2 RandomPointOnUnitCircle(float radius) {
        float angle = UnityEngine.Random.Range (0f, Mathf.PI * 2);
        float x = Mathf.Sin (angle) * radius;
        float y = Mathf.Cos (angle) * radius;

        return new Vector2(x, y);
    }
}
