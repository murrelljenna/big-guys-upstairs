using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using game.assets.spawners;
using game.assets.utilities.resources;
using Fusion;

namespace game.assets.tools
{
    [System.Serializable]
    public struct PrefabCostMapping
    {
        [Tooltip("Floats above the ground before you've placed the building")]
        public GameObject ghost;
        [Tooltip("Actual building to place")]
        public NetworkPrefabRef prefab;
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

        [Networked]
        public bool placingBuilding { get; set; } = false;

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
                placingBuilding = true;
                placeWalls.enabled = true;
                spawner.enabled = false;
            } else if (index < buildableItems.Length)
            {
                placingBuilding = true;
                placeWalls.enabled = false;
                spawner.enabled = true;
                spawner.prefab = buildableItems[index].prefab;
                spawner.price = buildableItems[index].cost;
                spawner.setGhost(buildableItems[index].ghost);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_ResetSpawner()
        {
            resetSpawner();
        }

        public void resetSpawner() {
            if (placeWalls != null) 
                placeWalls.enabled = false;
            if (spawner != null && spawner.Object != null)
            {
                spawner.enabled = false;
                spawner.price = new ResourceSet();
                spawner.setGhost(null);
                spawner.showGhost = false;
            }

            placingBuilding = false;
        }
    }
}
