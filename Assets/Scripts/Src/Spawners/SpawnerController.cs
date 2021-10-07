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

        public GameObject Spawn(GameObject prefab, ResourceSet price, Vector3 spawnLocation, Quaternion rotation)
        {
            if (transactor.canAfford(price))
            {
                transactor.takeResources(price);
                return instantiator.InstantiateAsMine(prefab, spawnLocation, rotation);
            }
            return null;
        }

        public GameObject SpawnAsPlayer(GameObject prefab, ResourceSet price, Vector3 spawnLocation, Quaternion rotation, player.Player player) {
            if (transactor.canAfford(price))
            {
                transactor.takeResources(price);
                return instantiator.InstantiateAsPlayer(prefab, spawnLocation, rotation, player);
            }
            return null;
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
