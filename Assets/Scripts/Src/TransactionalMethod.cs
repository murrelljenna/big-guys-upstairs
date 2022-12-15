using Fusion;
using game.assets.player;
using game.assets.utilities.resources;
using UnityEngine;
using UnityEngine.Events;

namespace game.assets
{
    [RequireComponent(typeof(Ownership))]
    public class TransactionalMethod : NetworkBehaviour
    {
        public UnityEvent cannotAfford = new UnityEvent();
        public UnityEvent canAfford = new UnityEvent();
        public ResourceSet price;
        public Ownership ownership;
        public bool isEnabled = true;

        public override void Spawned()
        {
            ownership = GetComponent<Ownership>();
        }

        public bool Try()
        {
            if (ownership.owner.canAfford(price))
            {
                if (Object.HasStateAuthority)
                {
                    ownership.owner.takeResources(price);
                    canAfford.Invoke();
                }
                return true;
            }
            else
            {
                cannotAfford.Invoke();
                return false;
            }
        }

        public void InvokeTry()
        {
            Try();
        }
    }
}
