using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.spawners;
using game.assets;
using static game.assets.utilities.GameUtils;

namespace game.assets.ai {
    public class AIUnitRecruiter
    {
        player.Player player;

        public AIUnitRecruiter(player.Player player) {
            Debug.Log(player);
            if (player == null)
            {
                Debug.LogError("Fucking idiot, you're passing in a null player.");
            }
            this.player = player;
        }

        public GameObject InvokeSpawn(Vector3 position) {
            Spawner[] spawners = nearbySpawners(closestSpawner(position));
            Spawner randomSpawner = spawners[Random.Range(0, spawners.Length)];
            return randomSpawner.SpawnForPlayer(player);
        }

        private Spawner closestSpawner(Vector3 position) {
            Spawner[] spawners = GameObject.FindObjectsOfType<Spawner>();
            Transform tMin = null;
            float minDist = Mathf.Infinity;
            foreach (Spawner spawner in spawners)
            {
                if (spawner.BelongsTo(player))
                {
                    float dist = Vector3.Distance(spawner.transform.position, position);
                    if (dist < minDist)
                    {
                        tMin = spawner.transform;
                        minDist = dist;
                    }
                }
            }
            return tMin.GetComponent<Spawner>();
        }

        private Spawner[] nearbySpawners(Spawner spawner)
        {
            GameObject[] gameObjects = findGameObjectsInRange(spawner.transform.position, 20f);
            return gameObjects.GetComponents<Spawner>();
        }
    }
}
