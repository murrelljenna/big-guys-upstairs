using Fusion;
using game.assets.player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static game.assets.utilities.GameUtils;

namespace game.assets.ui
{
    public class BuildingColourController : MonoBehaviour
    {
        private string[] names = new string[] {
            "Model",
            "Construction_0",
            "Construction_1",
            "Destruction_0",
            "Destruction_1"
        };

        public void SetColourToPlayer(player.Player player)
        {
            if (player == null)
            {
                return;
            }
            List<string> listNames = new List<string>(names);
            string colorName = player.colour.name;
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                if (listNames.Contains(renderers[i].gameObject.name)) {
                    renderers[i].material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Buildings_" + colorName) as Texture));
                }
            }
        }

        private void Start()
        {
            var ownership = GetComponent<Ownership>();

            if (ownership != null && ownership.Object != null && ownership.owned)
            {
                SetColourToPlayer(ownership.owner);
            }
        }
    }
}
