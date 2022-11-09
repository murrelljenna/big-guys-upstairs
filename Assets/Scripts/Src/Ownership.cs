using Fusion;
using UnityEngine;
using UnityEngine.Events;

namespace game.assets.player
{
    public class Ownership : NetworkBehaviour
    {
        [Networked]
        public bool owned { get; set; } = false;

        [SerializeField]
        [Networked(OnChanged=nameof(FireNewOwnerEvent))]
        public Player owner { get; set; }
        [SerializeField]
        public string playerName;

        [Tooltip("Invoke when new owner is set")]
        public UnityEvent<Player> onNewOwner;

        public bool isOwnedBy(Player player)
        {
            if (owned && owner != null && player == owner) {
                return true;
            }

            return false;
        }

        public static void FireNewOwnerEvent(Changed<Ownership> changed)
        {
            if (changed.Behaviour.owned)
            {
                changed.Behaviour.onNewOwner.Invoke(changed.Behaviour.owner);
            }
        }

        public void setOwner(Player player)
        {
            owned = true;
            owner = player;
            if (onNewOwner != null)
            {
                onNewOwner.Invoke(player);
            }
            playerName = owner.playerName;
        }

        public void setOwnerRecursively(Player player)
        {
            Debug.Log("AD - SetOwnerRecursively");
            setOwner(player);
            
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
