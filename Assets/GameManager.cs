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
	GameObject localPlayer;

	[SerializeField]
	List<Color> colours = new List<Color>() { Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.magenta, Color.red, Color.white, Color.yellow };
	[SerializeField]
	List<int> possibleColours;

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

				possibleColours = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

	    		int index = UnityEngine.Random.Range(0, possibleColours.Count);

				ExitGames.Client.Photon.Hashtable playerSettings = new ExitGames.Client.Photon.Hashtable();
		    	playerSettings.Add("color", possibleColours[index].ToString());
		    	possibleColours.RemoveAt(index);
		    	Debug.Log(index);

		    	PhotonNetwork.SetPlayerCustomProperties(playerSettings);
		    }
		    
		    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
		    localPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(35f,0f,35f), Quaternion.identity, 0);
		    localPlayer.GetComponent<game.assets.Player>().playerColor = colours[Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["color"])];
	    	localPlayer.transform.Find("FPSController").transform.Find("Capsule").GetComponent<MeshRenderer>().material.color = colours[Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["color"])];
		}
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
		Debug.Log("Executing shit");
		if (localPlayer != null) {
			Debug.Log("YES!");

			localPlayer.GetComponent<PhotonView>().RPC("setColour", RpcTarget.All, Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["color"]));
			/*
	    	localPlayer.GetComponent<game.assets.Player>().playerColor = colours[Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["color"])];
	    	localPlayer.transform.Find("FPSController").transform.Find("Capsule").GetComponent<MeshRenderer>().material.color = colours[Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["color"])];
			*/
		}
	}

	//public override void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient) {
		// Put 
	//}

	public override void OnPlayerEnteredRoom(Player other) {
    	Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);

	    if (PhotonNetwork.IsMasterClient) {
	    	int index = UnityEngine.Random.Range(0, possibleColours.Count);

	    	ExitGames.Client.Photon.Hashtable playerSettings = new ExitGames.Client.Photon.Hashtable();
	    	playerSettings.Add("color", possibleColours[index].ToString());
	    	possibleColours.RemoveAt(index);
	    	Debug.Log(index);

	    	other.SetCustomProperties(playerSettings);

	        Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
	    }
	}

	void OnPhotonInstantiate(PhotonMessageInfo info) {
	    if (PhotonNetwork.IsMasterClient) {
	        Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
	    }
	}

	public override void OnPlayerLeftRoom(Player other) {
	    Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

	    if (PhotonNetwork.IsMasterClient)
	    {
	        Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
	    }
	}
}
