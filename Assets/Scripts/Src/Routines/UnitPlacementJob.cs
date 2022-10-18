using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using game.assets.ai;
using System;
using UnityEngine.Events;
using game.assets.utilities;

namespace game.assets.routines
{
    public class UnitPlacementJob : InterruptibleJob
    {
        private int reachedLocation = 0;
        private int lastSent = 0;
        private Vector3 center;
        private Queue<Movement> units;
        private MovementAggregation movAgg;
        private const float unitSize = 0.2f;
        private const float gapSize = 0.3f;

        private bool fuckOff = false;

        public UnitPlacementJob(Vector3 center, MovementAggregation movAgg)
        {
            this.center = center;
            this.units = new Queue<Movement>(movAgg.units);
            this.movAgg = movAgg;

        }

        protected override IEnumerator execute_impl()
        {
            Queue<Vector3> points = new Queue<Vector3>();
            List<Vector3> taken = new List<Vector3>();
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
            int j = 0;
            while (units.Count > 0)
            {
                Movement unit = units.Dequeue();

                if (unit == null)
                {
                    continue;
                }


                Vector3 destination = points.Dequeue();


                void destinationReached()
                {
                    reachedLocation++;
                    if (reachedLocation == lastSent && units.Count == 0 && !fuckOff)
                    {
                        j++;
                        movAgg.locationReached.Invoke(center);
                        Interrupt();
                        fuckOff = true;
                    }
                    unit.reachedDestination.RemoveListener(destinationReached);
                }

                void lessLastSent(Health _)
                {
                    lastSent--;
                    unit.GetComponent<Health>()?.onZeroHP?.RemoveListener(lessLastSent);
                }

                unit.goTo(destination);
                taken.Add(destination);

                unit.reachedDestination.AddListener(destinationReached);
                var zeroHpEvent = unit.GetComponent<Health>()?.onZeroHP;
                markForCleanup(unit.reachedDestination, destinationReached);

                if (zeroHpEvent != null)
                {
                    var action = new UnityAction<Health>(lessLastSent);
                    zeroHpEvent.AddListener(lessLastSent);
                    markForCleanup(zeroHpEvent, action);
                }

                lastSent++;

                for (int i = 0; i < positionMods.Length; i++)
                {
                    Vector3 modifiedPosition = destination + positionMods[i];
                    float height = GameUtils.getTerrainHeight(modifiedPosition);

                    NavMeshHit hit;
                    // TODO: Fix magic 0.1f float
                    var isOnMesh = NavMesh.SamplePosition(modifiedPosition, out hit, 0.1f, NavMesh.AllAreas);
                    if (!alreadyTaken(taken, modifiedPosition) && Math.Abs(height) - Math.Abs(center.y) < 4 && Math.Abs(height) - Math.Abs(center.y) > -4 && isOnMesh)
                    {
                        points.Enqueue(modifiedPosition);
                        taken.Add(modifiedPosition);
                    }
                }

                runCount++;
                if (runCount % 2 == 0)
                {
                    yield return null;
                }
            }
        }
        private static bool alreadyTaken(List<Vector3> vectors, Vector3 target)
        {
            return !vectors.TrueForAll((Vector3 v) => Vector3.Distance(v, target) > 0.2f);
        }
    }
}
