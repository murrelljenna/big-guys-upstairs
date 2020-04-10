using System;
using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using UnityStandardAssets.Characters.FirstPerson;

public class SpawnManager : MonoBehaviourPunCallbacks, IPunObservable
{
	public GameObject playerPrefab;
	public GameObject cityPrefab;

	private GameObject localPlayer;
	private GameObject startingCity;

	// Events
	private byte SpawnEvent = 1;

	private List<Color> colours = new List<Color>() { Color.black, Color.blue, Color.white, Color.green, Color.magenta, Color.red, Color.yellow };
	private List<string> colourStrings = new List<string>() { "black", "blue", "white", "green", "pink", "red", "yellow" };
	private List<int> possibleColours = new List<int>();
	
	private List<Vector3> spawnPoints = new List<Vector3>() { 
		new Vector3(14.54f, 0f, 13.89f),
		new Vector3(13.9f, 0f, 108.4f),
		new Vector3(108.03f, 0f, 108.4f),
		new Vector3(108.41f, 0f, 16.36f)
	};

	private int spawnIndex;

	void Start() {

		if (PhotonNetwork.IsMasterClient) {
			for (int i = 0; i < colours.Count; i++) {
				Debug.Log(colours.Count);
				Debug.Log(possibleColours);
				possibleColours.Add(i);
			}

    		int index = UnityEngine.Random.Range(0, possibleColours.Count);

/* */
			localPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0);

			Vector3 spawnPoint = getAvailableSpawn(localPlayer.GetComponent<game.assets.Player>().playerID);
			int colourIndex = getAvailableColour(localPlayer.GetComponent<game.assets.Player>().playerID);
		    Vector2 randomInCircle = RandomPointOnUnitCircle(3f);
	    	Vector3 spawnPointLocation = new Vector3(randomInCircle.x+spawnPoint.x, 0, randomInCircle.y+spawnPoint.z);
			
			localPlayer.transform.Find("FPSController").GetComponent<CharacterController>().enabled = false;
			localPlayer.transform.position = spawnPointLocation;
			localPlayer.transform.Find("FPSController").GetComponent<CharacterController>().enabled = true;

		    ExitGames.Client.Photon.Hashtable playerSettings = new ExitGames.Client.Photon.Hashtable();
		    playerSettings.Add("playerid", localPlayer.GetComponent<game.assets.Player>().playerID.ToString());
		   	PhotonNetwork.SetPlayerCustomProperties(playerSettings);

		   	startingCity = PhotonNetwork.Instantiate(this.cityPrefab.name, spawnPoint, Quaternion.identity, 0);
			startingCity.GetComponent<ownership>().capture(localPlayer.GetComponent<game.assets.Player>());
			startingCity.GetComponent<buildingGhost>().active = false;
		} else {
			spawnPlayer();
		}


	    // we're in a room. spawnPoint a character for the local player. it gets synced by using PhotonNetwork.Instantiate
	    // Instantiate player object first, because we need to do that in order to get their id
	}
/*
	public void OnEvent(EventData photonEvent) {
		byte eventCode = photonEvent.Code;

		if (eventCode == MoveUnitsToTargetPositionEvent) {
			object[] data = (object[])photonEvent.CustomData;
		}
	}
*/
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            //stream.SendNext(hp);
        }
        else
        {
            //hp = (int)stream.ReceiveNext();
        }
    }

    public override void OnPlayerEnteredRoom(Player other) {
	    // we're in a room. spawnPoint a character for the local player. it gets synced by using PhotonNetwork.Instantiate
	    // Instantiate player object first, because we need to do that in order to get their id
    }

    public void cleanupSpawns() {
    	Debug.Log("test");
    	clearPoint(spawnIndex);
	}
    
    public void spawnPlayer() {
    	Debug.Log("SPAWNING PLAYER?");
    	localPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0);

		Vector3 spawnPoint = getAvailableSpawn(localPlayer.GetComponent<game.assets.Player>().playerID);
		int colourIndex = getAvailableColour(localPlayer.GetComponent<game.assets.Player>().playerID);
	    Vector2 randomInCircle = RandomPointOnUnitCircle(3f);
    	Vector3 spawnPointLocation = new Vector3(randomInCircle.x+spawnPoint.x, 0, randomInCircle.y+spawnPoint.z);

    	Debug.Log(spawnPoint);
		
		localPlayer.transform.Find("FPSController").GetComponent<CharacterController>().enabled = false;
		localPlayer.transform.position = spawnPointLocation;
		localPlayer.transform.Find("FPSController").GetComponent<CharacterController>().enabled = true;

	    ExitGames.Client.Photon.Hashtable playerSettings = new ExitGames.Client.Photon.Hashtable();
	    playerSettings.Add("playerid", localPlayer.GetComponent<game.assets.Player>().playerID.ToString());
	    playerSettings.Add("color", colourIndex);
	   	PhotonNetwork.SetPlayerCustomProperties(playerSettings);

	   	startingCity = PhotonNetwork.Instantiate(this.cityPrefab.name, spawnPoint, Quaternion.identity, 0);
	   	startingCity.GetComponent<ownership>().capture(localPlayer.GetComponent<game.assets.Player>());
	   	startingCity.GetComponent<buildingGhost>().active = false;
    }

    private void takeColour(int index) {
    	bool[] coloursAvail = (bool[])PhotonNetwork.CurrentRoom.CustomProperties["coloursAvail"];
    	coloursAvail[index] = true;

		ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
    	roomProperties.Add("coloursAvail", coloursAvail);

    	PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }

    public int getAvailableColour(int playerID) {
    	bool[] coloursAvail = (bool[])PhotonNetwork.CurrentRoom.CustomProperties["coloursAvail"];
		for (int i =0; i < spawnPoints.Count; i++) {
			Debug.Log(i);
			if (!coloursAvail[i]) {
				takeColour(i);
				return i;
			}
		}

		return 0;
    }

    public Vector3 getAvailableSpawn(int playerID) {
    	Debug.Log("IN SPAWN");
    	bool[] spawnsAvail = (bool[])PhotonNetwork.CurrentRoom.CustomProperties["spawnsAvail"];
    	Debug.Log(spawnsAvail);
		for (int i =0; i < spawnPoints.Count; i++) {
			Debug.Log(i);
			Debug.Log(spawnPoints[i]);
			Debug.Log(spawnsAvail[i]);
			if (!spawnsAvail[i]) {
				spawnIndex = i;
				takePoint(i);
				return spawnPoints[i];
			}
		}

		return (new Vector3(0, 0, 0));
    }

    [PunRPC]
    private void takePoint(int index) {
    	bool[] spawnsAvail = (bool[])PhotonNetwork.CurrentRoom.CustomProperties["spawnsAvail"];
    	spawnsAvail[index] = true;

		ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
    	roomProperties.Add("spawnsAvail", spawnsAvail);

    	PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }

    [PunRPC]
    private void clearPoint(int index) {
    	bool[] spawnsAvail = (bool[])PhotonNetwork.CurrentRoom.CustomProperties["spawnsAvail"];
    	spawnsAvail[index] = false;

    	Debug.Log("Clearing point");
    	Debug.Log(index);

		ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
    	roomProperties.Add("spawnsAvail", spawnsAvail);

    	PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }

	private static Vector2 RandomPointOnUnitCircle(float radius) {
        float angle = UnityEngine.Random.Range (0f, Mathf.PI * 2);
        float x = Mathf.Sin (angle) * radius;
        float y = Mathf.Cos (angle) * radius;

        return new Vector2(x, y);
    }

    /* PunCallbacks */


}