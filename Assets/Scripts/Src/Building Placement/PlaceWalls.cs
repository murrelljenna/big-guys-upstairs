using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.utilities.resources;
using game.assets;
using game.assets.player;
using game.assets.ai.units;
using Fusion;
using game.assets.utilities;
using UnityEngine.AI;

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

    private bool activeWallCoroutine = false;

    private bool snappedToExistingWall = false;

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
        Reset();
        if (currentBuilding != null)
        {
            DestroyImmediate(currentBuilding.gameObject);
        }
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
        if (enabled == false || activeWallCoroutine)
        {
            return;
        }
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, game.assets.utilities.GameUtils.LayerMask.Terrain))
        {
            if (firstPointPlaced)
            {
                lastPoint = GameUtils.SnapToWalkableArea(hit.point);
                float wallUnitLength = currentBuilding.transform.Find("Model").GetComponent<MeshFilter>().mesh.bounds.size.z / 3f * currentBuilding.transform.Find("Model").localScale.z;
                float pointDistance = Vector3.Distance(firstPoint, lastPoint);

                float noWalls = pointDistance / wallUnitLength;
                int wood = 1;
                Vector3 slightlyOffGround = new Vector3(0, 0.4f);

                if (ownership.owner.canAfford(new ResourceSet(wood * (int)noWalls)))
                {
                    RaycastHit info;
                    if ((!Physics.Linecast(firstPoint + slightlyOffGround, lastPoint + slightlyOffGround, out info, GameUtils.LayerMask.All) || info.collider.gameObject.GetComponent<DoNotAutoAttack>() != null || info.collider.isTrigger))
                    {
                        if (System.Math.Abs(firstPoint.y) - System.Math.Abs(lastPoint.y) < 0.5f && System.Math.Abs(firstPoint.y) - System.Math.Abs(lastPoint.y) > -0.5f)
                        {
                            if (Object.HasStateAuthority)
                            {
                                ownership.owner.takeResources(new ResourceSet(wood * (int)noWalls));
                                StartCoroutine(placeWalls(noWalls, firstPointSnapped, lastPointSnapped));
                            }

                            firstPointPlaced = false;
                            lastPointSnapped = false;
                            firstPointSnapped = false;
                        }
                    }
                    else
                    {
                        if (info.collider != null)
                        {
                            Debug.Log("WAL - Couldn't place walls because gameobject " + info.collider.gameObject.name + " was detected between start and end wall");
                            StartCoroutine(flashRed(info.collider.gameObject, 0.2f));
                        }
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
    }

    public override void FixedUpdateNetwork()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, game.assets.utilities.GameUtils.LayerMask.Terrain))
        {
            if (wallInRange(hit.point, 0.2f))
            {
                hit.point = snapWallInRange(hit.point, 0.5f);
                currentBuilding.Find("Model").GetComponent<Renderer>().enabled = false;

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
                snappedToExistingWall = false;
                currentBuilding.Find("Model").GetComponent<Renderer>().enabled = true;
                if (firstPointPlaced)
                {
                    lastPointSnapped = false;
                }
                else
                {
                    firstPointSnapped = false;
                }
            }

            Vector3 position = snappedToExistingWall ? hit.point : GameUtils.SnapToWalkableArea(hit.point);

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
                snappedToExistingWall = true;
                return hitColliders[i].transform.position;
            }
        }

        return new Vector3(0, 0, 0);
    }

    private IEnumerator placeWalls(float noWalls, bool firstPointSnapped, bool lastPointSnapped)
    {
        activeWallCoroutine = true;
        for (int i = 0; i <= (int)noWalls; i++)
        {
            Vector3 destination = Vector3.Lerp(firstPoint, lastPoint, (float)i * (1f / noWalls));
            float terrainHeight = GameUtils.getTerrainHeight(destination);
            Vector3 destinationAdjustedForTerrain = new Vector3(destination.x, terrainHeight, destination.z);
            GameObject placedBuilding = null;
            if ((i == 0 && !firstPointSnapped) || (i == (int)noWalls && !lastPointSnapped))
            {
                NetworkObject netObj = Runner.Spawn(
                cornerPrefab, destinationAdjustedForTerrain,
                Quaternion.LookRotation(lastPoint - destination),
                null,
                (runner, o) =>
                {
                    o.SetAsPlayer(ownership.owner);
                }
                );

                NetworkedGameManager.Get().registerNetworkObject(ownership.owner, netObj);

                placedBuilding = netObj.gameObject;
            }
            else
            {
                var netObj = Runner.Spawn(
                    wallPrefab, destinationAdjustedForTerrain,
                    Quaternion.LookRotation(lastPoint - destination),
                    ownership.owner.networkPlayer,
                    (runner, o) =>
                    {
                        o.SetAsPlayer(ownership.owner);
                    }
                );

                NetworkedGameManager.Get().registerNetworkObject(ownership.owner, netObj);

                placedBuilding = netObj.gameObject;
            }

            yield return null;
        }
        activeWallCoroutine = false;
    }

    private IEnumerator flashRed(GameObject building, float offset = 0.2f)
    {
        Renderer renderer = building.transform.Find("Model")?.gameObject?.GetComponent<Renderer>();

        if (renderer == null)
        {
            renderer = building.GetComponentInChildren<Renderer>();
        }

        if (renderer == null)
        {
            yield break;
        }

        renderer.material.color = Color.red;

        yield return new WaitForSeconds(offset);

        renderer.material.color = previousColor; // Set when building gets selected in setBuilding.
    }
}
