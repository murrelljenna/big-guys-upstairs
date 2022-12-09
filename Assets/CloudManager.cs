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

    public void Start()
    {
        if (cloudSpawners == null || cloudSpawners.Length == 0)
        {
            Debug.LogWarning("No cloud spawners found. No clouds in your game then!");
            enabled = false;
            return;
        }

        InvokeRepeating("spawnCloud", 0f, 2f);
    }

    private void spawnCloud()
    {
        var spawner = cloudSpawners.RandomElem();
        activeClouds.Add(
            new CloudAndEndpointPair(
                Instantiate(cloudPrefabs.RandomElem(), spawner.transform.position, Quaternion.identity), 
                spawner.endpoint, 
                spawner
                )
            );
    }

    private void FixedUpdate()
    {
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
