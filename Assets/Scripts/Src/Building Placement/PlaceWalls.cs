using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.utilities.resources;
using game.assets;
using game.assets.player;
using game.assets.ai.units;
using Fusion;

[RequireComponent(typeof(LineRenderer))]
public class PlaceWalls : NetworkBehaviour
{
    private bool firstPointPlaced = false;
    private bool firstPointSnapped = false;
    private bool lastPointSnapped = false;
    private Vector3 firstPoint;
    private Vector3 lastPoint;
    private Transform currentBuilding;
    private LineRenderer lineRen;
    public Camera cam;
    public Ownership ownership;
    public GameObject ghostPrefab;
    private Color previousColor;

    public GameObject wallPrefab;
    public GameObject cornerPrefab;

    private RaycastHit hit;

    public override void Spawned()
    {
        if (ownership == null)
        {
            Debug.LogError("PlaceWalls.cs is missing a reference to an Ownership script.");
        }
        lineRen = this.GetComponent<LineRenderer>();
    }

    void OnDisable()
    {
        firstPointPlaced = false;
        lastPointSnapped = false;
        firstPointSnapped = false;
        lineRen.positionCount = 0;
        DestroyImmediate(currentBuilding.gameObject);
    }

    private void OnEnable()
    {
        setBuilding(ghostPrefab);
    }

    public void Reset()
    {
        firstPointPlaced = false;
        lastPointSnapped = false;
        firstPointSnapped = false;
        lineRen.positionCount = 0;
    }

    public void PlaceWall()
    {
        if (firstPointPlaced)
        {
            lastPoint = hit.point;
            float wallUnitLength = currentBuilding.transform.Find("Model").GetComponent<MeshFilter>().mesh.bounds.size.z / 3f * currentBuilding.transform.Find("Model").localScale.z;
            float pointDistance = Vector3.Distance(firstPoint, lastPoint);

            float noWalls = pointDistance / wallUnitLength;
            int mapLayer = ~(1 << 11);
            int wood = 1;

            if (ownership.owner.canAfford(new ResourceSet(wood = (wood * (int)noWalls))))
            {
                RaycastHit info;
                if ((!Physics.Linecast(firstPoint, lastPoint, out info, mapLayer) || info.collider.gameObject.GetComponent<DoNotAutoAttack>() != null))
                {
                    if (System.Math.Abs(firstPoint.y) - System.Math.Abs(lastPoint.y) < 0.5f && System.Math.Abs(firstPoint.y) - System.Math.Abs(lastPoint.y) > -0.5f)
                    {
                        if (Object.HasStateAuthority)
                        {
                            ownership.owner.takeResources(new ResourceSet(wood = (wood * (int)noWalls)));
                            StartCoroutine(placeWalls(noWalls, firstPointSnapped, lastPointSnapped));
                        }

                        firstPointPlaced = false;
                        lastPointSnapped = false;
                        firstPointSnapped = false;
                    }
                }
                else
                {
                    StartCoroutine(flashRed(currentBuilding.gameObject, 0.2f));
                    StartCoroutine(flashRed(info.collider.gameObject, 0.2f));
                }
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(flashRed(currentBuilding.gameObject, 0.2f));
            }

        }
        else
        {
            firstPoint = hit.point;
            firstPointPlaced = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, game.assets.utilities.GameUtils.LayerMask.Terrain))
        {
            if (wallInRange(hit.point, 0.5f))
            {
                hit.point = snapWallInRange(hit.point, 0.5f);
                currentBuilding.gameObject.SetActive(false);

                if (firstPointPlaced)
                {
                    lastPointSnapped = true;
                }
                else
                {
                    firstPointSnapped = true;
                }
            }
            else
            {
                if (firstPointPlaced)
                {
                    lastPointSnapped = false;
                }
                else
                {
                    firstPointSnapped = false;
                }
            }

            Vector3 position = hit.point;
            position.y += 0.5f;
            currentBuilding.transform.position = position;

            if (firstPointPlaced)
            {
                Vector3[] positions = new Vector3[2];
                lineRen.positionCount = 2;
                positions[0] = firstPoint;
                positions[1] = hit.point;
                lineRen.SetPositions(positions);

                currentBuilding.rotation = Quaternion.LookRotation(hit.point - firstPoint);
            }
        }
    }

    public void setBuilding(GameObject building)
    {
        if (currentBuilding != null)
        {
            Destroy(currentBuilding.gameObject);
            currentBuilding = null;
        }

        currentBuilding = ((GameObject)Instantiate(building)).transform;

        Renderer renderer = currentBuilding.transform.Find("Model").gameObject.GetComponent<MeshRenderer>();

        previousColor = renderer.material.color;
    }

    private bool wallInRange(Vector3 location, float range)
    {
        Collider[] hitColliders = Physics.OverlapSphere(location, range);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.BelongsTo(ownership.owner) && hitColliders[i].gameObject.GetComponent<WallCorner>() != null)
            {
                return true;
            }
        }

        return false;
    }

    private Vector3 snapWallInRange(Vector3 location, float range)
    {
        Collider[] hitColliders = Physics.OverlapSphere(location, range);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.GetComponent<WallCorner>() != null)
            {
                return hitColliders[i].transform.position;
            }
        }

        return new Vector3(0, 0, 0);
    }

    private IEnumerator placeWalls(float noWalls, bool firstPointSnapped, bool lastPointSnapped)
    {
        for (int i = 0; i <= (int)noWalls; i++)
        {
            Vector3 destination = Vector3.Lerp(firstPoint, lastPoint, (float)i * (1f / noWalls));
            GameObject placedBuilding = null;
            if ((i == 0 && !firstPointSnapped) || (i == (int)noWalls && !lastPointSnapped))
            {
                placedBuilding = //InstantiatorFactory.getInstantiator(false).InstantiateAsMine(cornerPrefab, destination, Quaternion.LookRotation(lastPoint - destination));
                Runner.Spawn(
                cornerPrefab, destination,
                Quaternion.LookRotation(lastPoint - destination),
                null,
                (runner, o) =>
                {
                    o.SetAsPlayer(ownership.owner);
                }
                ).gameObject;
            }
            else
            {
                placedBuilding = Runner.Spawn(
                wallPrefab, destination,
                Quaternion.LookRotation(lastPoint - destination),
                null,
                (runner, o) =>
                {
                    o.SetAsPlayer(ownership.owner);
                }
                ).gameObject;
            }

            yield return null;
        }
    }

    private IEnumerator flashRed(GameObject building, float offset = 0.2f)
    {
        Renderer renderer = building.transform.Find("Model").gameObject.GetComponent<MeshRenderer>();

        renderer.material.color = Color.red;

        yield return new WaitForSeconds(offset);

        renderer.material.color = previousColor; // Set when building gets selected in setBuilding.
    }
}
