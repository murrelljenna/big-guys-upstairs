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
        public Vector3 endpointPosition;
        public Vector3 startingPointPosition;

        public CloudAndEndpointPair(GameObject cloud, CloudEndpoint endpoint, CloudSpawner startingPoint)
        {
            this.cloud = cloud;
            this.endpoint = endpoint;
            this.startingPoint = startingPoint;
            elapsedFrames = 0;

            this.endpointPosition = endpoint.transform.position;
            this.startingPointPosition = startingPoint.transform.position;
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
            cloudSpawners[i].lastSpawn = -1500;
        }

        startRandomCloudsMidPath();

        InvokeRepeating("spawnCloudMetered", 0f, 4f);
    }

    private CloudAndEndpointPair spawnCloudMetered()
    {
        CloudSpawner spawner = cloudSpawners.RandomElem();

        if (totalElapsedFrames - spawner.lastSpawn < 1500)
        {
            return null;
        }

        var cloudPair = spawnCloud(spawner);

        cloudPair.startingPoint.lastSpawn = totalElapsedFrames;

        return cloudPair;
    }

    private CloudAndEndpointPair spawnCloud(CloudSpawner spawner)
    {
        var cloudPair = new CloudAndEndpointPair(
                Instantiate(cloudPrefabs.RandomElem(), spawner.transform.position, Quaternion.identity),
                spawner.endpoint,
                spawner
                );
        activeClouds.Add(cloudPair);
        var kindaRandomSize = Random.Range(0.4f, 1.25f);

        cloudPair.cloud.transform.localScale = new Vector3(
            kindaRandomSize,
            kindaRandomSize,
            kindaRandomSize
        );

        float randomX = Random.Range(-10f, 10f);
        float randomY = Random.Range(-0f, 20f);
        float randomZ = Random.Range(-10f, 10f);
        cloudPair.startingPointPosition = new Vector3(
            cloudPair.startingPointPosition.x + randomX,
            cloudPair.startingPointPosition.y + randomY,
            cloudPair.startingPointPosition.z + randomZ
        );

        cloudPair.endpointPosition = new Vector3(
            cloudPair.endpointPosition.x + randomX,
            cloudPair.endpointPosition.y + randomY,
            cloudPair.endpointPosition.z + randomZ
        );

        return cloudPair;
    }

    private void startRandomCloudsMidPath()
    {
        var randomNumberOfClouds = Random.Range(10, 30);

        for (int i = 0; i < randomNumberOfClouds; i++) {

            CloudSpawner spawner = cloudSpawners.RandomElem();
            var cloudPair = spawnCloud(spawner);

            float randomElapsedFrames = Random.Range(0f, cloudSpeed);

            Vector3 midwaySpawnLocation = Vector3.Lerp(
                cloudPair.startingPointPosition,
                cloudPair.endpointPosition,
                randomElapsedFrames / cloudSpeed
                );

            cloudPair.startingPointPosition = midwaySpawnLocation;
        }
        
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
                    pair.startingPointPosition,
                    pair.endpointPosition,
                    interpolationRatio
                    );
            }
        }
    }
}
