using Fusion;
using game.assets.player;
using UnityEngine;

namespace game.assets
{
    public static class Instantiation
    {
        public static NetworkObject SpawnNetwork(NetworkRunner runner, NetworkPrefabRef prefab, Vector3 spawnLocation, Quaternion rotation, Player owner)
        {
            NetworkObject netObj = runner.Spawn(prefab, spawnLocation, rotation, owner.networkPlayer, (runner, obj) =>
            {
                obj.AssignInputAuthority(owner.networkPlayer);
                obj.GetComponent<Ownership>()?.setOwner(owner);
            });

            NetworkedGameManager.Get().registerNetworkObject(owner, netObj);

            return netObj;
        }
    }
}