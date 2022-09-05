using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace game.assets.spawners
{
    public class RaycastSpawner : Spawner
    {
        [Tooltip("Numeric value representing layer mask used by raycast")]
        public int layer;

        [Tooltip("Show building ghost at raycast")]
        public bool showGhost;

        [Tooltip("Invoked on plop animation complete")]
        public UnityEvent onPlopped;

        private int layerMask;

        public Camera cam;

        private void Start()
        {
            if (cam == null)
            {
                Debug.LogError("RaycastSpawner Camera has not been assigned.");
            }

            layerMask = 1 << layer;
        }

        public override GameObject Spawn()
        {
            Vector3? raycastHit = raycastFromCamera(cam);

            if (raycastHit != null)
            {
                Vector3 endSpawnLocation = (Vector3)raycastHit;
                Vector3 startSpawnLocation = endSpawnLocation;
                startSpawnLocation.y += 0.5f;

                GameObject spawnedObject = spawnerController.Spawn(prefab, price, startSpawnLocation, Quaternion.identity);

                if (spawnedObject != null)
                {
                    StartCoroutine(plop(spawnedObject, endSpawnLocation));
                }

                return spawnedObject;
            }
            else
            {
                return null;
            }
        }

        private IEnumerator plop(GameObject gameObject, Vector3 destination) {
            float startTime = Time.time;
            while (Vector3.Distance(gameObject.transform.position, destination) > 0.01f) {
                float distCovered = (Time.time - startTime) * 0.5f;
                float fractionOfJourney = distCovered / Vector3.Distance(gameObject.transform.position, destination);

                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, destination, fractionOfJourney);
                yield return null;
            }
        }


        private Vector3? raycastFromCamera(Camera cam)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                return hit.point;
            }

            return null;
        }
    }
}
