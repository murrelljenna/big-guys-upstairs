using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Text;
using game.assets;

public class Launcher : MonoBehaviourPunCallbacks
{
	[SerializeField]
	private GameObject controlPanel;

	[SerializeField]
	private GameObject progressLabel;

    public GameObject localGameManagerPrefab;

    private MapReader.Map map;

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
    		//string region = "us";
    		//PhotonNetwork.ConnectToRegion(region);
    		PhotonNetwork.ConnectUsingSettings();
    	}
    }

    // Connection callbacks

    public override void OnConnectedToMaster() {
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
        map = this.GetComponent<MapReader>().randomMap();
        string[] spawns = new string[20];
        for (int i = 0; i < map.spawns.locations.Count; i++) {
            spawns[i] = map.spawns.locations[i].toCSV();
        }

        // Not great but... fill up the rest of this static array with empty strings so it can serialize.
        for (int i = map.spawns.locations.Count; i < spawns.Length; i++) {
            spawns[i] = "";
        }

        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
        bool[] spawnsAvail = new bool[20];
        
        roomProperties.Add("spawnsAvail", spawnsAvail);
        roomProperties.Add("maxPlayers", int.Parse(map.players));
        roomProperties.Add("spawns", spawns);

    	PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = (byte)int.Parse(map.players), CustomRoomProperties = roomProperties});
    }

	public override void OnJoinedRoom() {
		if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
		    PhotonNetwork.LoadLevel(map.sceneName);
		}
	}

    public void exitGame() {
        Application.Quit();
    }

    public void launchLocalGame()
    {
        map = GetComponent<MapReader>().randomMap();

        string[] spawns = new string[20];
        for (int i = 0; i < map.spawns.locations.Count; i++)
        {
            spawns[i] = map.spawns.locations[i].toCSV();
        }

        LocalGameManager gameManager = Instantiate(localGameManagerPrefab).GetComponent<LocalGameManager>();
    }
}
