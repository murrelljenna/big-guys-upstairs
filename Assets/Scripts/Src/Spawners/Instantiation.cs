using Fusion;
using game.assets.player;
using UnityEngine;
using UnityEngine.Events;

namespace game.assets
{
    public static class Instantiation
    {
        public static UnityEvent<NetworkObject> ObjectSpawned = new UnityEvent<NetworkObject>();
        public static UnityEvent<NetworkObject> OnBeforeObjectDespawned = new UnityEvent<NetworkObject>();
        public static NetworkObject SpawnNetwork(NetworkRunner runner, NetworkPrefabRef prefab, Vector3 spawnLocation, Quaternion rotation, Player owner)
        {
            NetworkObject netObj = runner.Spawn(prefab, spawnLocation, rotation, owner.networkPlayer, (runner, obj) =>
            {
                obj.AssignInputAuthority(owner.networkPlayer);
                obj.GetComponent<Ownership>()?.setOwner(owner);
            });

            NetworkedGameManager.Get().registerNetworkObject(owner, netObj);
            ObjectSpawned.Invoke(netObj);

            return netObj;
        }

        public static void Despawn(NetworkRunner runner, NetworkObject obj)
        {
            OnBeforeObjectDespawned.Invoke(obj);
            runner.Despawn(obj);
        }
    }
}