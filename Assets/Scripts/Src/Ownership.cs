using Fusion;
using UnityEngine;
using UnityEngine.Events;

namespace game.assets.player
{
    public class Ownership : NetworkBehaviour
    {
        [Tooltip("If owned")]
        public bool owned = false;

        public Player owner { get; set; }

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

        public void setOwnerRecursively(Player player)
        {
            owned = true;
            owner = player;
            if (onNewOwner != null)
            {
                onNewOwner.Invoke(player);
            }

            foreach (Ownership ownership in transform.GetComponentsInChildren<Ownership>())
            {
                ownership.setOwner(player);
            }
        }

        public void clearOwner()
        {
            owned = false;
            owner = null;
        }
    }
}
