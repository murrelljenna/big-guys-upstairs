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
        [Tooltip("Floats above the ground before you've placed the building")]
        public GameObject ghost;
        [Tooltip("Actual building to place")]
        public GameObject prefab;
        public ResourceSet cost;
    }

    [RequireComponent(typeof(RaycastSpawner))]
    [RequireComponent(typeof(PlaceWalls))]
    public class BuilderTool : MonoBehaviour
    {
        [Tooltip("Set of prefabs and associated costs")]
        public PrefabCostMapping[] buildableItems;
        private PlaceWalls placeWalls;
        private RaycastSpawner spawner;

        private void Start()
        {
            spawner = GetComponent<RaycastSpawner>();
            placeWalls = GetComponent<PlaceWalls>();

            spawner.onPlopped.AddListener(() => resetSpawner());
        }

        private void OnDisable()
        {
            resetSpawner();
        }

        public void setPrefab(int index)
        {
            index--;
            if (index == 5)
            {
                placeWalls.enabled = true;
                spawner.enabled = false;
            } else if (index < buildableItems.Length)
            {
                placeWalls.enabled = false;
                spawner.enabled = true;
                spawner.prefab = buildableItems[index].prefab;
                spawner.price = buildableItems[index].cost;
                spawner.setGhost(buildableItems[index].ghost);
            }
        }

        private void resetSpawner() {
            placeWalls.enabled = false;

            spawner.enabled = false;
            spawner.prefab = null;
            spawner.price = new ResourceSet();
            spawner.setGhost(null);
        }
    }
}
