using game.assets.utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareSelection : MonoBehaviour
{
    private Vector3 VIEWPORT_POINT_TO_RAY = new Vector3(0.5F, 0.5F, 0);
    public Camera camera;
    private bool firstPointTaken = false;
    private Vector3 firstPoint;

    private const float PROJECTOR_Z_AXIS = 2f;

    public GameObject projectorPrefab;
    private GameObject projectorObj;

    private void Start()
    {
        if (projectorPrefab == null)
        {
            Debug.LogError("SquareSelection has no projector prefab to spawn");
        }
    }

    private void Update()
    {
        if (!firstPointTaken)
        {
            return;
        }

        RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(VIEWPORT_POINT_TO_RAY);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameUtils.LayerMask.Terrain))
        {
            Debug.Log("B- First point is taken, raycast hit terrain");
            var secondPoint = hit.point;

            var xDist = secondPoint.x - firstPoint.x;
            var zDist = secondPoint.z - firstPoint.z;

            var projectorScale = new Vector3(xDist, PROJECTOR_Z_AXIS, zDist);

            //projectorObj.GetComponent<Projector>().aspectRatio = projectorScale;

            //projectorObj.transform.LookAt(camera.transform);
        }
    }

    public void StartSquareSelection()
    {
        RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(VIEWPORT_POINT_TO_RAY);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameUtils.LayerMask.Terrain))
        {
            Debug.Log("B - Looking at terrain, starting square selection");
            firstPointTaken = true;
            firstPoint = hit.point;

            if (projectorObj == null)
            {
                projectorObj = GameObject.Instantiate(projectorPrefab);
            }

            //var projectorPos = new Vector3(firstPoint.x, 1f, firstPoint.z);

            //projectorObj.transform.position = projectorPos;
        }
    }

    public void LetGo()
    {
        firstPointTaken = false;
    }
}
