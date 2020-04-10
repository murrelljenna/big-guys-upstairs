using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using UnityStandardAssets.Characters.FirstPerson;

public class EscMenu : MonoBehaviourPun
{
	private GameObject gameMenu;
    private GameManager gameManager;
	private FirstPersonController playerController;

	void Start() {
		gameMenu = this.transform.Find("EscMenu").gameObject;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
	}

    // Update is called once per frame
    void Update() {
    	if(Input.GetKeyDown(KeyCode.Escape)) {
    		toggleMenu();
    	}
    }

    public void toggleMenu() {
    	if (playerController == null) {
    		playerController = GameObject.Find("FPSController").gameObject.GetComponent<FirstPersonController>();
    	}

		gameMenu.SetActive(!gameMenu.activeInHierarchy);
		playerController.enabled = !playerController.enabled;

		// Cursor stuff

		Cursor.visible = !Cursor.visible;

		if (Cursor.lockState != CursorLockMode.Confined) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.lockState = CursorLockMode.Confined;
		}
    }

    public void exitToMenu() {
    	gameManager.LeaveRoom();
    	PhotonNetwork.LoadLevel("Launcher");
    }

    public void exitToDesktop() {
    	gameManager.LeaveRoom();
    	Application.Quit();
    }
}
