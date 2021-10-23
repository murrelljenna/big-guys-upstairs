using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using game.assets.utilities.resources;
using game.assets.player;
using static game.assets.utilities.GameUtils;

namespace game.assets.spawners
{
    public class Spawner : MonoBehaviour
    {
        [Tooltip("Prefab to Instantiate")]
        public GameObject prefab;
        [Tooltip("Price to Instantiate")]
        public ResourceSet price = new ResourceSet();
        [Tooltip("Networked. Requires Photon network access when ticked.")]
        public bool networked = false;
        [Tooltip("Radius in which units spawn")]
        public float spawnRadius = 1.2f;

        protected SpawnerController spawnerController = new SpawnerController();

        public virtual void Start()
        {
            spawnerController.setInstantiator(InstantiatorFactory.getInstantiator(networked));
            spawnerController.setTransactor(LocalPlayer.getPlayerDepositor());
        }

        public virtual GameObject Spawn()
        {
            Vector3 spawnLocation = getSpawnLocation(this.transform.position);
            return spawnerController.Spawn(prefab, price, spawnLocation, Quaternion.identity);
        }

        private Vector3 getSpawnLocation(Vector3 spawnCenter)
        {
            return randomPointOnUnitCircle(transform.position, spawnRadius);
        }

        public GameObject SpawnForPlayer(player.Player player) {
            Vector3 spawnLocation = getSpawnLocation(this.transform.position);
            return spawnerController.SpawnAsPlayer(prefab, price, spawnLocation, Quaternion.identity, player);
        }

        public void InvokeSpawn()
        {
            Spawn();
        }
    }
}
