using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets.player
{
    public class SwitchWeapons : NetworkBehaviour
    {
        [Networked(OnChanged=nameof(SelectWeaponsCallback))]
        public uint selectedTool { get; set; } = 0u;

        public override void Spawned()
        {
            selectWeapons();
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority && GetInput(out PlayerNetworkInput input))
            {
                if (input.IsDown(PlayerNetworkInput.BUTTON_ACTION2))
                {
                    Debug.Log("Is down!");
                    if (selectedTool >= transform.childCount - 1)
                    {
                        selectedTool = 0u;
                    }
                    else
                    {
                        selectedTool++;
                    }
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

        private static void SelectWeaponsCallback(Changed<SwitchWeapons> sw)
        {
            sw.Behaviour.selectWeapons();
        }

        public void selectWeapons()
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
