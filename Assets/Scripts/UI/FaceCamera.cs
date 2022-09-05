using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace game.assets.ui
{
    public class FaceCamera : MonoBehaviour
    {

        private Camera playerCamera;
        void FixedUpdate()
        {
            if (playerCamera != null)
            {
                this.transform.LookAt(playerCamera.transform);
            }
            else
            {
                playerCamera = getLocalCamera();
            }
        }

        private Camera getLocalCamera()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<PhotonView>().IsMine)
                {
                    return players[i].transform.Find("FPSController").transform.Find("FirstPersonCharacter").GetComponent<Camera>();
                }
            }
            return null;
        }
    }
}