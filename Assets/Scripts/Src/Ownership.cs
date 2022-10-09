using UnityEngine;
using UnityEngine.Events;

namespace game.assets.player
{
    public class Ownership : MonoBehaviour
    {
        [Tooltip("If owned")]
        public bool owned = false;

        [Tooltip("Player who owns this GameObject")]
        public Player owner;

        [Tooltip("Invoke when new owner is set")]
        public UnityEvent<Player> onNewOwner;

        public bool isOwnedBy(Player player)
        {
            if (owned && owner != null && player == owner) {
                return true;
            }

            return false;
        }

        public void setOwner(Player player)
        {
            owned = true;
            owner = player;
            if (onNewOwner != null)
            {
                onNewOwner.Invoke(player);
            }
        }

        public void clearOwner()
        {
            owned = false;
            owner = null;
        }
    }
}
