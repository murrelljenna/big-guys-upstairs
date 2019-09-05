using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
	[SerializeField]
	private GameObject controlPanel;

	[SerializeField]
	private GameObject progressLabel;

	[SerializeField]
	private byte maxPlayersPerRoom = 4;

	string gameVersion = "1";
	bool isConnecting;

	void Start() {
		progressLabel.SetActive(false);
		controlPanel.SetActive(true);
	}

    void Awake() {
    	PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void Connect(){
    	progressLabel.SetActive(true);
		controlPanel.SetActive(false);

		isConnecting = true;

    	if (PhotonNetwork.IsConnected) {
    		PhotonNetwork.JoinRandomRoom();
    	} else {
    		PhotonNetwork.GameVersion = gameVersion;
    		string region = "us";
    		//PhotonNetwork.ConnectToRegion(region);
    		PhotonNetwork.ConnectUsingSettings();
    	}
    }

    // Connection callbacks

    public override void OnConnectedToMaster() {
    	Debug.Log("Connected");
    	if (isConnecting)
		{
			PhotonNetwork.JoinRandomRoom();
		}
    }

    public override void OnDisconnected(DisconnectCause cause) {
    	progressLabel.SetActive(false);
		controlPanel.SetActive(true);	
    	Debug.LogWarningFormat("Disconnected - {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
    	Debug.Log(message);
    	Debug.Log("No room available. Creating new room");
    	PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = maxPlayersPerRoom});
    }

	public override void OnJoinedRoom() {
		Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
		if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
		    Debug.Log("We load room");
		    PhotonNetwork.LoadLevel("Flat");
		}
	}
}
