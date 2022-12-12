﻿using System.Collections;
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
                return null;
            }
            Vector3 spawnLocation = getSpawnLocation(transform.position);
            return SpawnIfCanAfford(prefab, spawnLocation, Quaternion.identity, ownership.owner);
        }

        protected GameObject SpawnIfCanAfford(NetworkPrefabRef prefab, Vector3 spawnLocation, Quaternion rotation, Player player)
        {
            if (player.canAfford(price))
            {
                player.takeResources(price);
                NetworkObject whatthefuckingfuck = Spawn(prefab, spawnLocation, Quaternion.identity, player.networkPlayer);
                NetworkedGameManager.Get().registerNetworkObject(player, whatthefuckingfuck);
                whatthefuckingfuck.SetAsPlayer(player);
                return whatthefuckingfuck.gameObject;
            }
            else
            {
                getFlasher()?.flashRelevant(player.resources, price);
                return null;
            }
        }

        protected NetworkObject Spawn(NetworkPrefabRef prefab, Vector3 spawnLocation, Quaternion rotation, PlayerRef playerInput)
        {
            return Runner.Spawn(prefab, spawnLocation, rotation, ownership.owner.networkPlayer, (runner, obj) => {
                obj.AssignInputAuthority(ownership.owner.networkPlayer);
                obj.GetComponent<Ownership>()?.setOwner(ownership.owner);
                }
            );
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
