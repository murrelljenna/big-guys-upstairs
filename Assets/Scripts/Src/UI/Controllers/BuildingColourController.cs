using game.assets.player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static game.assets.utilities.GameUtils;

namespace game.assets.ui
{
    public class BuildingColourController : MonoBehaviour, IColourController
    {
        public void SetColourToPlayer(player.Player player)
        {
            string colorName = player.colour.name;
            transform.Find("Model").gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Buildings_" + colorName) as Texture));
        }
    }
}
