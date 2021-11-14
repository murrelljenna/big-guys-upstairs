using game.assets.player;
using game.assets.utilities.resources;
using UnityEngine;
using UnityEngine.Events;

namespace game.assets
{

    public class TransactionalMethod : MonoBehaviour
    {
        public UnityEvent cannotAfford;
        public UnityEvent canAfford;
        public ResourceSet price;
        IPlayerTransaction transactor;

        public void Try()
        {
            transactor = LocalPlayer.getPlayerDepositor();
            if (transactor.canAfford(price))
            {
                transactor.takeResources(price);
                canAfford.Invoke();
            }
            else
            {
                cannotAfford.Invoke();
            }
        }
    }
}
