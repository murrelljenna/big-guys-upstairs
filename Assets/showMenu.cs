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
    
    void Awake()
    {
        BuildingMenu = GameObject.Find("BuildingMenu");
        up1 = GameObject.Find("1_Normal");
        down1 = GameObject.Find("1_Pressed");
        up2 = GameObject.Find("2_Normal");
        down2 = GameObject.Find("2_Pressed");

        if (down2 != null) {
            down1.SetActive(false);
        }

        if (down2 != null) {
            down2.SetActive(false);
        }
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
            up1.SetActive(false);
            down1.SetActive(true);
            Invoke("releaseButton1", 0.2f);

			GetComponent<buildingPlacement>().setBuilding(options[0]); // Town
		}
        if (Input.GetKeyDown("2")) {
            up2.SetActive(false);
            down2.SetActive(true);
            Invoke("releaseButton2", 0.2f);

            GetComponent<buildingPlacement>().setBuilding(options[1]); // Guard tower
        }
    }

    void releaseButton1() {
        up1.SetActive(true);
        down1.SetActive(false);
    }

    void releaseButton2() {
        up2.SetActive(true);
        down2.SetActive(false);
    }
}
