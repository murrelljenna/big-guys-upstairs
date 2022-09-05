using UnityEngine;

namespace game.assets.ui
{
    public interface IColourController
    {
        void SetColourToPlayer(player.Player player);
    }
    public class UnitColourController : MonoBehaviour, IColourController
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
    }
}
