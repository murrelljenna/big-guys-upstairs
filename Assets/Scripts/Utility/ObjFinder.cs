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
    }
}
