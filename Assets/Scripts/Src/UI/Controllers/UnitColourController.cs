using Fusion;
using game.assets.player;
using UnityEngine;

namespace game.assets.ui
{
    public class UnitColourController : NetworkBehaviour
    {
        public void SetColourToPlayer(player.Player player)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            string colorName = player.colour.name;

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.SetTexture("_MainTex", (Resources.Load("TT_RTS_Units_" + colorName) as Texture));
            }
        }

        public override void Spawned()
        {
            var ownership = GetComponent<Ownership>();

            if (ownership != null && ownership.owned)
            {
                SetColourToPlayer(ownership.owner);
            }
        }
    }
}
