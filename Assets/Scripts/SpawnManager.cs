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
    
    private Vector3[] spawnPoints = new Vector3[20];

    private int spawnIndex;

    private Vector3 csvToVector(string pos) {
        string[] coords = pos.Split(',');
        return new Vector3(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
        }
        else
        {
        }
    }

    void Start() {
        string[] spawns = (string[])PhotonNetwork.CurrentRoom.CustomProperties["spawns"];
        int noPlayers = (int)PhotonNetwork.CurrentRoom.CustomProperties["maxPlayers"];

        for (int i = 0; i < noPlayers; i++) {
            spawnPoints[i] = csvToVector(spawns[i]);
        }

        if (PhotonNetwork.IsMasterClient) {
            for (int i = 0; i < colours.Count; i++) {
                possibleColours.Add(i);
            }

            int index = UnityEngine.Random.Range(0, possibleColours.Count);

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
            startingCity.GetComponent<Town>().build();
        } else {
            spawnPlayer();
        }
    }

    public void cleanupSpawns() {
        clearPoint(spawnIndex);
    }
    
    public void spawnPlayer() {
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

        startingCity.GetComponent<Town>().build();
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
        for (int i =0; i < spawnPoints.Length; i++) {
            Debug.Log(i);
            if (!coloursAvail[i]) {
                takeColour(i);
                return i;
            }
        }

        return 0;
    }

    public Vector3 getAvailableSpawn(int playerID) {
        bool[] spawnsAvail = (bool[])PhotonNetwork.CurrentRoom.CustomProperties["spawnsAvail"];
        for (int i =0; i < spawnPoints.Length; i++) {
            print(spawnsAvail[1]);
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
}
