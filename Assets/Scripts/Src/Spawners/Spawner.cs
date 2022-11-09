using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using game.assets.utilities.resources;
using game.assets.player;
using static game.assets.utilities.GameUtils;
using Fusion;

namespace game.assets.spawners
{
    [RequireComponent(typeof(Ownership))]
    public class Spawner : NetworkBehaviour
    {
        [Tooltip("Prefab to Instantiate")]
        public NetworkPrefabRef prefab;
        [Tooltip("Price to Instantiate")]
        public ResourceSet price = new ResourceSet();
        [Tooltip("Networked. Requires Photon network access when ticked.")]
        public bool networked = false;
        [Tooltip("Radius in which units spawn")]
        public float spawnRadius = 1.2f;

        public Ownership ownership;

        private FlashResourceIconsRed flasher;

        public override void Spawned()
        {
            ownership = GetComponent<Ownership>();
        }

        public virtual GameObject Spawn()
        {
            if (!Object.HasStateAuthority || ownership.owner.maxPop() || !prefab.IsValid)
            {
                Debug.Log("AD - Object has state authority: " + Object.HasStateAuthority);
                Debug.Log("AD - Object has maxPop: " + ownership.owner.maxPop());
                Debug.Log("AD - Object has prefab.IsValid: " + prefab.IsValid);
                return null;
            }
            Debug.Log("AD - Hey there");
            Vector3 spawnLocation = getSpawnLocation(transform.position);
            return SpawnIfCanAfford(prefab, spawnLocation, Quaternion.identity, ownership.owner);
        }

        protected GameObject SpawnIfCanAfford(NetworkPrefabRef prefab, Vector3 spawnLocation, Quaternion rotation, Player player)
        {
            if (player.canAfford(price))
            {
                player.takeResources(price);
                Debug.Log("AD - Spawning shit");
                GameObject whatthefuckingfuck = Spawn(prefab, spawnLocation, Quaternion.identity, player.networkPlayer);
                whatthefuckingfuck.SetAsPlayer(player);
                Debug.Log("WHAT THE FUCK");
                return whatthefuckingfuck;
            }
            else
            {
                getFlasher()?.flashRelevant(player.resources, price);
                return null;
            }
        }

        protected GameObject Spawn(NetworkPrefabRef prefab, Vector3 spawnLocation, Quaternion rotation, PlayerRef playerInput)
        {
            return Runner.Spawn(prefab, spawnLocation, Quaternion.identity, ownership.owner.networkPlayer).gameObject;
        }

        private Vector3 getSpawnLocation(Vector3 spawnCenter)
        {
            return randomPointOnUnitCircle(transform.position, spawnRadius);
        }

        public void InvokeSpawn()
        {
            Spawn();
        }

        private FlashResourceIconsRed getFlasher()
        {
            if (flasher == null)
            {
                flasher = GameObject.Find("ResourcePanel").GetComponent<FlashResourceIconsRed>();
            }

            return flasher;
        }
    }
}
