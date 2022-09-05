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
        IPlayerTransaction transactor;

        public bool Try()
        {
            transactor = LocalPlayer.getPlayerDepositor();
            Debug.Log(price.wood);
            Debug.Log(transactor.canAfford(price));
            if (transactor.canAfford(price))
            {
                transactor.takeResources(price);
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
