using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.spawners;
using game.assets;

namespace game.assets.ai {
    public class AIUnitRecruiter
    {
        player.Player player;

        public AIUnitRecruiter(player.Player player) {
            this.player = player;

        }

        public GameObject InvokeSpawn(Vector3 position) {
            Spawner spawner = closestSpawner(position);
            return spawner.SpawnForPlayer(player);
        }

        private Spawner closestSpawner(Vector3 position) {
            Spawner[] spawners = GameObject.FindObjectsOfType<Spawner>();
            Transform tMin = null;
            float minDist = Mathf.Infinity;
            Debug.Log(spawners.Length);
            foreach (Spawner spawner in spawners)
            {
                Debug.Log(spawner.name);
                if (spawner.BelongsTo(player))
                {
                    Debug.Log("lol");
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
    }
}
