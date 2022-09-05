using System;
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
		    }
		    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
		    GameObject player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(15f,0f,15f), Quaternion.identity, 0);
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

	public override void OnPlayerEnteredRoom(Player other) {
    	Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);
	    if (PhotonNetwork.IsMasterClient) {
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
