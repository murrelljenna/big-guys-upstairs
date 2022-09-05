using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace game.assets.player
{
    public class switchWeapons : MonoBehaviourPun
    {

        public uint selectedTool = 0u;

        // Start is called before the first frame update
        void Start()
        {
            selectWeapons();
        }

        // Update is called once per frame
        void Update()
        {
            if (transform.parent.parent.gameObject.GetComponent<PhotonView>().IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (selectedTool >= transform.childCount - 1)
                {
                    selectedTool = 0u;
                }
                else
                {
                    selectedTool++;
                }

                selectWeapons();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (selectedTool <= 0u)
                {
                    selectedTool = (uint)transform.childCount - 1;
                }
                else
                {
                    selectedTool--;
                }

                selectWeapons();
            }
        }

        void selectWeapons()
        {
            uint i = 0u;
            foreach (Transform tool in transform)
            {
                if (selectedTool == i)
                {
                    tool.gameObject.SetActive(true);
                }
                else
                {
                    tool.gameObject.SetActive(false);
                }
                i++;
            }
        }
    }
}
