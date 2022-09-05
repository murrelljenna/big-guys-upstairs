using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PlayerStart : MonoBehaviourPunCallbacks
{	
	[Tooltip("The prefab to use for representing the player")]
	public GameObject playerPrefab;
	[Tooltip("The prefab to use for representing the player's starting city")]
	public GameObject cityPrefab;
	public bool taken = false;

    public GameObject spawnCity() {
		return (PhotonNetwork.Instantiate(this.cityPrefab.name, this.transform.position, Quaternion.identity, 0));
    }

    public GameObject spawnPlayer() {
    	Vector2 randomInCircle = RandomPointOnUnitCircle(3f);
        Vector3 spawnLocation = new Vector3(randomInCircle.x+this.transform.position.x, 0, randomInCircle.y+this.transform.position.z);

        GameObject playerObj = PhotonNetwork.Instantiate(this.playerPrefab.name, spawnLocation, Quaternion.identity, 0);
    	GameObject city = spawnCity();

    	city.GetComponent<ownership>().capture(playerObj.GetComponent<game.assets.Player>());

    	taken = true;

    	return (playerObj);
    }

    private static Vector2 RandomPointOnUnitCircle(float radius) {
        float angle = Random.Range (0f, Mathf.PI * 2);
        float x = Mathf.Sin (angle) * radius;
        float y = Mathf.Cos (angle) * radius;

        return new Vector2(x, y);
    }
}
