using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using game.assets.utilities.resources;
using game.assets.player;

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

        private Vector2 RandomPointOnUnitCircle(float radius)
        {
            float angle = Random.Range(0f, Mathf.PI * 2);
            float x = Mathf.Sin(angle) * radius;
            float y = Mathf.Cos(angle) * radius;

            return new Vector2(x, y);
        }

        private Vector3 getSpawnLocation(Vector3 spawnCenter)
        {
            Vector2 randomInCircle = RandomPointOnUnitCircle(1.2f);

            return new Vector3(
                randomInCircle.x + spawnCenter.x,
                spawnCenter.y,
                randomInCircle.y + spawnCenter.z
            );
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
