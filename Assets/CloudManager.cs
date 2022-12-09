using game.assets.utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    class CloudAndEndpointPair {
        public GameObject cloud;
        public CloudEndpoint endpoint;
        public CloudSpawner startingPoint;
        public int elapsedFrames;

        public CloudAndEndpointPair(GameObject cloud, CloudEndpoint endpoint, CloudSpawner startingPoint)
        {
            this.cloud = cloud;
            this.endpoint = endpoint;
            this.startingPoint = startingPoint;
            elapsedFrames = 0;
        }
    }

    public GameObject[] cloudPrefabs;

    private List<CloudAndEndpointPair> activeClouds = new List<CloudAndEndpointPair>();

    [SerializeField]
    public CloudSpawner[] cloudSpawners;

    private const int cloudSpeed = 8000;
    private int totalElapsedFrames = 0;

    public void Start()
    {
        if (cloudSpawners == null || cloudSpawners.Length == 0)
        {
            Debug.LogWarning("No cloud spawners found. No clouds in your game then!");
            enabled = false;
            return;
        }

        for (int i = 0; i < cloudSpawners.Length; i++)
        {
            cloudSpawners[i].lastSpawn = -200;
        }

        InvokeRepeating("spawnCloud", 0f, 0.5f);
    }

    private void spawnCloud()
    {
        var spawner = cloudSpawners.RandomElem();

        if (totalElapsedFrames - spawner.lastSpawn < 200)
        {
            Debug.Log(":( last spawn: " + spawner.lastSpawn);
            Debug.Log(":( total elapsed frames: " + totalElapsedFrames);
            Debug.Log(":( really sad: " + (spawner.lastSpawn - totalElapsedFrames));
            return;
        }

        var cloudPair = new CloudAndEndpointPair(
                Instantiate(cloudPrefabs.RandomElem(), spawner.transform.position, Quaternion.identity),
                spawner.endpoint,
                spawner
                );
        activeClouds.Add(cloudPair);
        var kindaRandomSize = Random.Range(0.1f, 1.25f);
        cloudPair.cloud.transform.localScale = new Vector3(
            kindaRandomSize,
            kindaRandomSize,
            kindaRandomSize
        );
        cloudPair.startingPoint.lastSpawn = totalElapsedFrames;
    }

    private void FixedUpdate()
    {
        totalElapsedFrames = (totalElapsedFrames + 1) % (cloudSpeed + 1);
        for (int i = 0; i < activeClouds.Count; i++)
        {
            var pair = activeClouds[i];
            pair.elapsedFrames = (pair.elapsedFrames + 1) % (cloudSpeed + 1);
            float interpolationRatio = (float)pair.elapsedFrames / cloudSpeed;

            var anyFinishedClouds = pair.endpoint.AnyFinishedClouds();
            if (anyFinishedClouds.Contains(pair.cloud))
            {
                anyFinishedClouds.RemoveAt(i);
            }
            else
            {
                pair.cloud.transform.position = Vector3.Lerp(
                    pair.startingPoint.transform.position,
                    pair.endpoint.transform.position, 
                    interpolationRatio
                    );
            }
        }
    }
}
