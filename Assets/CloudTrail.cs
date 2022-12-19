using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudTrail : MonoBehaviour
{
    public GameObject cloudPrefab;
    private int elapsedFrames = 0;
    private const int cloudSpeed = 18000;

    public GameObject cloudInstance;
    private void Start()
    {
        cloudInstance = GameObject.Instantiate(cloudPrefab, transform.position, transform.rotation);
    }

    private void FixedUpdate()
    {
        if (cloudInstance == null)
        {
            return;
        }

        elapsedFrames = (elapsedFrames + 1) % (cloudSpeed + 1);
        float interpolationRatio = (float)elapsedFrames / cloudSpeed;

        cloudInstance.transform.position = Vector3.Lerp(
                    cloudInstance.transform.position,
                    transform.position,
                    interpolationRatio
                    );
    }

    private void OnDestroy()
    {
        Destroy(cloudInstance);
    }
}
