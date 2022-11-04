using game.assets.player;
using game.assets.utilities.resources;
using UnityEngine;
using UnityEngine.Events;

namespace game.assets
{
    public class TransactionalMethod : MonoBehaviour
    {
        public UnityEvent cannotAfford = new UnityEvent();
        public UnityEvent canAfford = new UnityEvent();
        public ResourceSet price;
        public Player player; 

        public bool Try()
        {
            if (player.canAfford(price))
            {
                player.takeResources(price);
                canAfford.Invoke();
                return true;
            }
            else
            {
                cannotAfford.Invoke();
                return false;
            }
        }
    }
}
