using game.assets.ai;
using game.assets.utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static game.assets.utilities.GameUtils;

public class MovementAggregation : IMovement
{
    public List<Movement> units = new List<Movement>();

    public UnityEvent<Vector3> locationReached = new UnityEvent<Vector3>();

    private int reachedLocation = 0;
    private int lastSent = 0;

    public MovementAggregation(List<Movement> units)
    {
        this.units = units;
    }

    public void goTo(Vector3 destination)
    {
        if (units.Count > 0)
        {
            units[0].StartCoroutine(placeUnits(destination, new Queue<Movement>(units)));
        }
    }



    public void stop()
    {
        units.ForEach(unit => unit.stop());
    }

    private IEnumerator placeUnits(Vector3 center, Queue<Movement> units, float unitSize = 0.2f, float gapSize = 0.2f)
    {
        reachedLocation = 0;
        lastSent = 0;
        Queue<Vector3> points = new Queue<Vector3>();
        Queue<Vector3> taken = new Queue<Vector3>();
        points.Enqueue(center);

        float branchSize = unitSize / 2f + gapSize;
        Vector3[] positionMods = new Vector3[8] {
            new Vector3(0, 0, branchSize),
            new Vector3(branchSize, 0, branchSize),
            new Vector3(branchSize, 0, 0),
            new Vector3(branchSize, 0, -branchSize),
            new Vector3(0, 0, -branchSize),
            new Vector3(-branchSize, 0, -branchSize),
            new Vector3(-branchSize, 0, 0),
            new Vector3(-branchSize, 0, branchSize)
        };

        int runCount = 0;
        while (units.Count > 0)
        {
            Movement unit = units.Dequeue();

            if (unit == null)
            {
                continue;
            }

            Vector3 destination = points.Dequeue();

            Debug.Log("New location we're going to!");

            void destinationReached()
            {
                reachedLocation++;
                Debug.Log("Location reached!");
                Debug.Log("ReachedLocation: " + reachedLocation);
                Debug.Log("lastSent: " + lastSent);
                if (reachedLocation == lastSent)
                {
                    locationReached.Invoke(center);
                }
            }

            unit.goTo(destination);

            unit.reachedDestination.AddListener(destinationReached);
            lastSent++;

            for (int i = 0; i < positionMods.Length; i++)
            {
                Vector3 modifiedPosition = destination + positionMods[i];
                float height = getTerrainHeight(modifiedPosition);

                NavMeshHit hit;
                // TODO: Fix magic 0.1f float
                var isOnMesh = NavMesh.SamplePosition(modifiedPosition, out hit, 0.1f, NavMesh.AllAreas);
                if (!taken.Contains(modifiedPosition) && Math.Abs(height) - Math.Abs(center.y) < 1 && Math.Abs(height) - Math.Abs(center.y) > -1 && isOnMesh)
                {
                    points.Enqueue(modifiedPosition);
                    taken.Enqueue(modifiedPosition);
                }
            }

            runCount++;
            if (runCount % 2 == 0)
            {
                yield return null;
            }
        }
    }
}
