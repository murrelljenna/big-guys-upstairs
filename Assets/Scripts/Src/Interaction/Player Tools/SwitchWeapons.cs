using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets.player
{
    public class SwitchWeapons : NetworkBehaviour
    {

        public uint selectedTool = 0u;

        public void Start()
        {
            selectWeapons();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out PlayerNetworkInput input))
            {
                if (input.IsDown(PlayerNetworkInput.BUTTON_ACTION2))
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
                /*else if (input.MOUSE_SCROLLWHEEL < 0f)
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
                }*/
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
