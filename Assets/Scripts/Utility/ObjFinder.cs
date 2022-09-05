using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets.utilities {
    public static class ObjFinder {
        public static GameObject findClosestCity(Vector3 point, game.assets.Player player) {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("town");
            GameObject closestEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (var obj in objs) {
                if (obj.GetComponent<ownership>().owner == player.playerID) {
                    float distance = Vector3.Distance(obj.transform.position, point);

                    if (distance < closestDistance) {
                        closestEnemy = obj;
                        closestDistance = distance;
                    }
                }                                                
            }

            return closestEnemy; 
        }

        public static GameObject findNearbyConstruction(Vector3 point, ownership owner)
        {
            int allBuildings = (1 << 10) | (1 << 14);

            Collider[] hitColliders = Physics.OverlapSphere(point, 10f, allBuildings);

            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i] != null && hitColliders[i].tag != "buildingGhost" && hitColliders[i].GetComponent<Building>().underConstruction && hitColliders[i].GetComponent<ownership>().owner == owner.owner)
                {
                    return hitColliders[i].gameObject;
                }
            }

            return null;
        }
    }
}
