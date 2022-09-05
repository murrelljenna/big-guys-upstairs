using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using game.assets.spawners;
using game.assets.utilities.resources;

namespace game.assets.tools
{
    [System.Serializable]
    public struct PrefabCostMapping
    {
        public GameObject prefab;
        public ResourceSet cost;
    }

    [RequireComponent(typeof(RaycastSpawner))]
    public class BuilderTool : MonoBehaviour
    {
        [Tooltip("Set of prefabs and associated costs")]
        public PrefabCostMapping[] buildableItems;

        private RaycastSpawner spawner;

        private void Start()
        {
            spawner = GetComponent<RaycastSpawner>();
        }

        public void setPrefab(int index)
        {
            index--;
            if (index < buildableItems.Length) {
                spawner.prefab = buildableItems[index].prefab;
                spawner.price = buildableItems[index].cost;
            }
        }
    }
}
