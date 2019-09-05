using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class showMenu : MonoBehaviourPun
{
	public GameObject BuildingMenu;
	public GameObject[] options;
    // Start is called before the first frame update
    void Start()
    {
       
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
        //Debug.Log(transform.parent.parent.gameObject.name);
        if (transform.parent.parent.parent.parent.GetComponent<PhotonView>().IsMine == false && PhotonNetwork.IsConnected == true) {
            return;
        }

    	if (Input.GetKeyDown("1")) {
    		if (Input.GetKeyDown("1")) {
    			GetComponent<buildingPlacement>().setBuilding(options[0]);
    		}
    	}
    }
}
