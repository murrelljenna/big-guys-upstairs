using game.assets;
using game.assets.ai;
using game.assets.utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace game.assets.routines
{
    public class UnitGroupingMovementJob : InterruptibleJob
    {
        private MovementAggregation units;
        private Vector3 destination;
        private bool destinationHasBeenReached = false;
        public UnityEvent<Vector3> reachedDestination = new UnityEvent<Vector3>();
        private Vector3 currentDestination;

        public UnitGroupingMovementJob(Vector3 point, MovementAggregation units)
        {
            this.units = units;
            this.destination = point;
        }

        protected override IEnumerator execute_impl()
        {
            return getPathAndMoveAlong(destination);
        }

        private void allGoToCurrentDestination()
        {
            units.goTo(currentDestination);
        }

        private void moveUnitsToLocation(Vector3 location)
        {
            currentDestination = location;
            destinationHasBeenReached = false;
            allGoToCurrentDestination();
            units.locationReached.AddListener(setDestinationReached);
            units.units.ForEach((Movement unit) =>
            {
                UnityEvent idled = unit.GetComponent<Attack>()?.idled;
                if (idled != null)
                {
                    idled.AddListener(allGoToCurrentDestination);
                    markForCleanup(idled, allGoToCurrentDestination);

                    unit.reachedDestination.AddOneTimeListener(() =>
                    {
                        idled.RemoveListener(allGoToCurrentDestination);
                    });
                }
            });
            markForCleanup(units.locationReached, setDestinationReached);
        }

        private void setDestinationReached(Vector3 _)
        {
            destinationHasBeenReached = true;
            units.locationReached.RemoveListener(setDestinationReached);
        }

        private IEnumerator getPathAndMoveAlong(Vector3 point)
        {
            var loc = units.location();
            NavMeshAgent agent = units.getMeSomeonesNavMeshAgent();
            NavMeshPath path = new NavMeshPath();
            bool isReachable = agent.CalculatePath(point, path);
            Vector3[] corners = path.corners;
            return moveAlongPoints(corners);

        }

        private IEnumerator moveAlongPoints(Vector3[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                var loc = points[i];
                moveUnitsToLocation(loc);
                destinationHasBeenReached = false; // Will be set once callback gets called

                yield return new WaitUntil(() => destinationHasBeenReached);
            }

            reachedDestination.Invoke(points[points.Length - 1]);
        }

        private void debugNavMeshPath(Vector3[] points)
        {
            Debug.Log("Debugging nav mesh path for AIUnitGrouping. Point count: " + points.Length);
            var lineRenderer = LocalGameManager.Get().gameObject.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = LocalGameManager.Get().gameObject.AddComponent<LineRenderer>();
            }
            lineRenderer.SetWidth(0.2f, 0.2f);
            lineRenderer.SetColors(Color.yellow, Color.yellow);
            lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }
    }
}
