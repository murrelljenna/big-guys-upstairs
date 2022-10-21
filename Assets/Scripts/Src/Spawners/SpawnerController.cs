using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using game.assets.utilities.resources;

namespace game.assets.spawners
{
    public class SpawnerController
    {
        IInstantiator instantiator;
        IPlayerTransaction transactor;

        FlashResourceIconsRed flasher;

        public GameObject Spawn(GameObject prefab, ResourceSet price, Vector3 spawnLocation, Quaternion rotation)
        {
            if (transactor.canAfford(price))
            {
                transactor.takeResources(price);
                return instantiator.InstantiateAsMine(prefab, spawnLocation, rotation);
            }
            else
            {
                getFlasher()?.flashRelevant(transactor.resources(), price);
            }
            return null;
        }

        public GameObject SpawnAsPlayer(GameObject prefab, ResourceSet price, Vector3 spawnLocation, Quaternion rotation, player.Player player) {
            if (transactor.canAfford(price))
            {
                transactor.takeResources(price);
                return instantiator.InstantiateAsPlayer(prefab, spawnLocation, rotation, player);
            }
            else
            {
                getFlasher()?.flashRelevant(transactor.resources(), price);
            }
            return null;
        }

        private FlashResourceIconsRed getFlasher()
        {
            if (flasher == null)
            {
                flasher = GameObject.Find("ResourcePanel").GetComponent<FlashResourceIconsRed>();
            }

            return flasher;
        }

        public void setInstantiator(IInstantiator instantiator)
        {
            this.instantiator = instantiator;
        }

        public void setTransactor(IPlayerTransaction transactor)
        {
            this.transactor = transactor;
        }
    }
}
