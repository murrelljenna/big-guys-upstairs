using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class showMenu : MonoBehaviourPun
{
	public GameObject BuildingMenu;
	public GameObject[] options;
    
    void Awake()
    {
       BuildingMenu = GameObject.Find("BuildingMenu");
    }

    void OnDisable()
    {
        BuildingMenu.SetActive(false);
    }

    void OnEnable()
    {
        BuildingMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent.parent.parent.GetComponent<PhotonView>().IsMine == false && PhotonNetwork.IsConnected == true) { // This gets the FPSController gameobject
            return;
        }

		if (Input.GetKeyDown("1")) {
			GetComponent<buildingPlacement>().setBuilding(options[0]); // Town
		}
        if (Input.GetKeyDown("2")) {
            Debug.Log("setBuilding");
            GetComponent<buildingPlacement>().setBuilding(options[1]); // Guard tower
        }
    }
}
