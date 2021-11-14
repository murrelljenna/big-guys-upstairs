using game.assets;
using game.assets.ai;
using game.assets.ai.units;
using game.assets.player;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets
{
    [RequireComponent(typeof(Ownership))]
    public class UpgradeWallToGate : MonoBehaviour
    {
        public GameObject gatePrefab;
        public void Upgrade()
        {
            List<GameObject> neighbours = new List<GameObject>();

            Collider[] hitColliders = Physics.OverlapSphere(gameObject.GetComponent<Collider>().bounds.center, 0.2f);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                GameObject go = hitColliders[i].gameObject;
                if (isWall(go) && go.IsFriendOf(gameObject))
                {
                    neighbours.Add(go);
                }
                else
                {
                    return;
                }
            }

            // Nothing in way, create gate
            neighbours.ForEach(neighbour =>
            {
                Destroy(neighbour); // Can I do this? We'll find out
            });

            GameObject gate = InstantiatorFactory.getInstantiator().Instantiate(gatePrefab, this.transform.position, this.transform.rotation);
            game.assets.player.Player player = GetComponent<Ownership>().owner;
            gate.SetAsPlayer(player);
        }

        private bool isWall(GameObject gameObject)
        {
            return (gameObject.GetComponent<Health>() != null && gameObject.GetComponent<DoNotAutoAttack>() != null);
        }
    }
}
