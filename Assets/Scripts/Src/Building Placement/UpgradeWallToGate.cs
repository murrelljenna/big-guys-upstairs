using Fusion;
using game.assets;
using game.assets.ai;
using game.assets.ai.units;
using game.assets.player;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets
{
    [RequireComponent(typeof(Ownership))]
    public class UpgradeWallToGate : NetworkBehaviour
    {
        public NetworkPrefabRef gatePrefab;
        public void Upgrade()
        {
            if (!Object.HasStateAuthority)
            {
                return;
            }
            List<GameObject> neighbours = new List<GameObject>();

            Collider[] hitColliders = Physics.OverlapSphere(gameObject.GetComponent<Collider>().bounds.center, 0.2f);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                GameObject go = hitColliders[i].gameObject;
                Debug.Log(":( isWall: " + isWall(go));
                Debug.Log(":( isFriendOf: " + go.IsFriendOf(gameObject));
                if (isWall(go) && go.IsFriendOf(gameObject))
                {
                    neighbours.Add(go);
                }
            }

            // Nothing in way, create gate
            Player owner = GetComponent<Ownership>().owner;
            GameObject gate = Runner.Spawn(
                gatePrefab,
                this.transform.position, 
                this.transform.rotation, 
                GetComponent<Ownership>().owner.networkPlayer,
                (runner, obj) => obj.GetComponent<Ownership>().setOwner(owner)
                ).gameObject;;

            DelayedDespawn.AsDevCube(neighbours).DestroyGos();
        }

        private bool isWall(GameObject gameObject)
        {
            return (gameObject.GetComponent<Health>() != null && gameObject.GetComponent<DoNotAutoAttack>() != null);
        }


    }
}
