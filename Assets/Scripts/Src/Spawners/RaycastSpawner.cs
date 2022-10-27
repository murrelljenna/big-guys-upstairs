using game.assets.audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static game.assets.utilities.GameUtils.MagicWords;

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

        public GameObject ghost;
        private GameObject ghostInstance;

        public override void Start()
        {
            if (cam == null)
            {
                Debug.LogError("RaycastSpawner Camera has not been assigned.");
            }

            layerMask = 1 << layer;

            base.Start();
        }

        public void Update()
        {
            if (showGhost && ghostInstance != null)
            {
                Vector3? point = raycastFromCamera(cam);

                if (point.HasValue)
                {
                    Vector3 position = point.Value;
                    position.y += 0.5f;
                    ghostInstance.transform.position = position;
                }
            }
        }

        public void setGhost(GameObject ghost)
        {
            DestroyImmediate(this.ghostInstance);

            if (ghost == null)
            {
                return;
            }

            this.ghostInstance = ((GameObject)Instantiate(ghost));
        }

        public override GameObject Spawn()
        {
            if (!enabled || !Object.HasStateAuthority || !prefab.IsValid)
            {
                return null;
            }

            if (prefab == null)
            {
                return null;
            }

            Vector3? raycastHit = raycastFromCamera(cam);

            if (ghostInstance != null && ghostInstance.GetComponent<Ghost>() != null && ghostInstance.GetComponent<Ghost>().colliding)
            {
                return null;
            }

            if (raycastHit != null)
            {
                Vector3 endSpawnLocation = (Vector3)raycastHit;
                Vector3 startSpawnLocation = endSpawnLocation;
                startSpawnLocation.y += 0.5f;

                GameObject spawnedObject = SpawnIfCanAfford(prefab, startSpawnLocation, Quaternion.identity, player);

                if (spawnedObject != null)
                {
                    if (ghostInstance != null)
                    {
                        Destroy(ghostInstance);
                    }
                    StartCoroutine(plop(spawnedObject, endSpawnLocation));
                    onPlopped.Invoke();
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

            playPlopSoundIfCan(gameObject);
            playPlopEffectIfCan(gameObject);
        }

        private void playPlopSoundIfCan(GameObject gameObject)
        {
            gameObject.transform.Find(GameObjectNames.Audio)?.gameObject.GetComponent<AudioController>()?.PlayRandom("PlopSounds");
        }

        private void playPlopEffectIfCan(GameObject gameObject)
        {
            gameObject.transform.Find(GameObjectNames.Effects)?.gameObject.GetComponent<EffectsController>()?.PlayRandom("PlopEffects", 30);
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

        private void OnDisable()
        {
            if (ghostInstance != null)
            {
                Destroy(ghostInstance);
            }
        }
    }
}
