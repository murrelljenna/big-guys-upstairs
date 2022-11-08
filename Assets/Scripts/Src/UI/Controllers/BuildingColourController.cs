﻿using Fusion;
using game.assets.player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static game.assets.utilities.GameUtils;

namespace game.assets.ui
{
    public class BuildingColourController : NetworkBehaviour, IColourController
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
            Debug.Log("AC - Player name: " + player.playerName);
            string colorName = player.colour.name;
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            Debug.Log("AC - Colour name: " + colorName);
            for (int i = 0; i < renderers.Length; i++)
            {
                Debug.Log("AC - SetColourToPlayer index: " + i);
                if (listNames.Contains(renderers[i].gameObject.name)) {
                    Debug.Log("AC - Setting texture");
                    renderers[i].material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Buildings_" + colorName) as Texture));
                }
            }
        }

        public override void Spawned()
        {
            var ownership = GetComponent<Ownership>();

            if (ownership != null && ownership.owned)
            {
                //SetColourToPlayer(ownership.owner);
            }
        }
    }
}
