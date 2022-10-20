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

    private GameObject projectorObj;

    private void Start()
    {
        var maybeProjectorObj = GameObject.Find("SquareProjector");
        if (maybeProjectorObj == null)
        {
            Debug.LogError("SquareSelection couldn't find the projector for the visible square. SquareSelection will be invisible");
        }
        else
        {
            projectorObj = maybeProjectorObj;
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

            projectorObj?.GetComponent<Projector>()?.material?.SetVector("_Corner2", secondPoint);
        }

        projectorObj?.GetComponent<Projector>()?.material?.SetVector("_Corner1", firstPoint);
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
        }
    }

    public void LetGo()
    {
        firstPoint = new Vector3(0, 0, 0);
        firstPointTaken = false;
    }
}
