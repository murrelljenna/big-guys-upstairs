using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

using UnityEngine.UI;
using UnityEngine.Events;

public class showMenu : MonoBehaviourPun
{
    public GameObject BuildingMenu;
    public GameObject[] options;

    private GameObject up1;
    private GameObject down1;
    private GameObject up2;
    private GameObject down2;
    private GameObject up3;
    private GameObject down3;
    private GameObject up4;
    private GameObject down4;
    
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
        if (BuildingMenu != null) { // Resulting in exception at beginning of game unless inside this statement
            BuildingMenu.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent.parent.parent.GetComponent<PhotonView>().IsMine == false && PhotonNetwork.IsConnected == true) { // This gets the FPSController gameobject
            return;
        }

        if (Input.GetKeyDown("z")) {
            GetComponent<buildingPlacement>().setBuilding(options[0]);
        }
        if (Input.GetKeyDown("x")) {
            GetComponent<buildingPlacement>().setBuilding(options[1]);
        }

        if (Input.GetKeyDown("c")) {
            GetComponent<buildingPlacement>().setBuilding(options[2]);
        }

        if (Input.GetKeyDown("v")) {
            GetComponent<buildingPlacement>().setBuilding(options[3]);
        }

        if (Input.GetKeyDown("b")) {
            GetComponent<buildingPlacement>().setBuilding(options[4]);
        }
        if (Input.GetKeyDown("n")) {
            GetComponent<buildingPlacement>().setBuilding(options[5]);
        }
    }
}
