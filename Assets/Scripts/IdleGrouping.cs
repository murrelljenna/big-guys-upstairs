using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class IdleGrouping : MonoBehaviour
{
    private List<Unit> selected = new List<Unit>();
    private Camera playerCamera;
    private GameObject info;

    private GameObject down1;
    private GameObject up1;

    private bool midAnimation = false;

    private void Start() {
        info = this.gameObject.transform.Find("Info").gameObject;

        up1 = this.transform.Find("Info").Find("1_Normal").gameObject;
        down1 = this.transform.Find("Info").Find("1_Pressed").gameObject;

        down1.SetActive(false);
    }

    private void Update() {
        if (playerCamera != null && info.active == true) {
            info.transform.LookAt(playerCamera.transform);   
        } else {
            playerCamera = getLocalCamera();
        }
    }

    public void addUnit(Unit unit) {
        selected.Add(unit);
        this.gameObject.transform.Find("Info").Find("Building Name").GetComponent<Text>().text = selected.Count.ToString() + " Idle Units";
    }

    public void removeUnit(Unit unit) {
        selected.Remove(unit);
        if (this.selected.Count < 1) {
            Destroy(this.gameObject);
        }
        this.gameObject.transform.Find("Info").Find("Building Name").GetComponent<Text>().text = selected.Count.ToString() + " Idle Units";
    }

    private Camera getLocalCamera() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++) {
            if (players[i].GetComponent<PhotonView>().IsMine) {
                return players[i].transform.Find("FPSController").transform.Find("FirstPersonCharacter").GetComponent<Camera>();
            }
        }
        return null;
    }

    public List<Unit> getGrouping() {
        return this.selected;
    }

    // UI Control

    public void clickButton() {
        if (!midAnimation) {
            up1.SetActive(false);
            down1.SetActive(true);
            midAnimation = true;

            Invoke("releaseButton1", 0.2f);
        }
    }

    private void releaseButton1() {
        up1.SetActive(true);
        down1.SetActive(false);
        midAnimation = false;
    }

    private void moveToCenter() {
        print("Not yet implemented");
        // Loop through everything and move to center. Not implemented yet.
    }
}
