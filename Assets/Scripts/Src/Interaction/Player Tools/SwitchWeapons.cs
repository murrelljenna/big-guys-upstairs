using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets.player
{
    public class SwitchWeapons : MonoBehaviour
    {

        public uint selectedTool = 0u;

        public void Start()
        {
            selectWeapons();
        }

        public void Update()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.Q))
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

        private void selectWeapons()
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
