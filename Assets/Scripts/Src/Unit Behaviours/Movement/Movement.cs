using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using game.assets.utilities;
using Fusion;

namespace game.assets.ai

{
    public class DestinationWatcher : MonoBehaviour
    {
        private Movement ourMovement;
        public void SetState(Movement movement)
        {
            ourMovement = movement;
        }

        public static DestinationWatcher Create(Vector3 pos, float radius, Movement movement) {
            var obj = new GameObject("Movement Collider for unit");
            SphereCollider col = obj.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = radius;
            obj.transform.position = pos;
            var watcher = obj.AddComponent<DestinationWatcher>();
            watcher.SetState(movement);
            obj.layer = GameUtils.LayerMask.IgnoreRaycast;
            return watcher;
        }

        public void Destroy()
        {
            Destroy(this.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            var otherMovementMaybe = other.gameObject.GetComponent<Movement>();
            if (otherMovementMaybe != null && otherMovementMaybe == ourMovement) {
                otherMovementMaybe.stop();
                otherMovementMaybe.RPC_FireReachedDestinationEvents();
                Destroy();
            }
        }
    }

    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    public class Movement : NetworkBehaviour
    {
        private UnityEngine.AI.NavMeshAgent navAgent;

        public bool moveOrdered = false;

        [Tooltip("Invoked when destination reached")]
        public UnityEvent reachedDestination;

        [Tooltip("Invoked when halted")]
        public UnityEvent halted;

        [Tooltip("Invoked when ordered to move to new position, but before actual orders are set.")]
        public UnityEvent newMoveOrdered;

        private DestinationWatcher currentWatcher;

        [Networked]
        public float speed { get; set; }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_FireMoveOrderedEvents()
        {
            newMoveOrdered.Invoke();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_FireHaltedEvents()
        {
            halted.Invoke();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_FireReachedDestinationEvents()
        {
            reachedDestination.Invoke();
        }

        void Start()
        {
            navAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();

            navAgent.Warp(transform.position);
        }


        private void halt()
        {
            RPC_FireHaltedEvents();
        }

        public void stop()
        {
            moveOrdered = false;
            if (navAgent.isOnNavMesh)
            {
                navAgent.isStopped = true;
                navAgent.ResetPath();
                navAgent.isStopped = false;
            }
            if (currentWatcher != null)
            {
                currentWatcher.Destroy();
                currentWatcher = null;
            }
            halt();
        }

        public void goToSilently(Vector3 destination)
        {
            Vector3 validDestination = GameUtils.SnapToWalkableArea(destination);
            moveOrdered = true;
            CapsuleCollider col = GetComponent<CapsuleCollider>();
            if (col == null)
            {
                Debug.LogError("Buddy you fucked up. This movement needs a capsule collider or you need to write code that works with other collider.");
            }
            if (currentWatcher != null)
            {
                currentWatcher.Destroy();
                currentWatcher = null;
            }
            currentWatcher = DestinationWatcher.Create(validDestination, col.radius / 4, this);
            bool successfulPath = navAgent.SetDestination(validDestination);
            if (!successfulPath)
            {
                Debug.LogError("NavMeshAgent failed to find path on " + name);
            }
            debugNavMeshPath(navAgent.path.corners);
        }

        public void goTo(Vector3 destination)
        {
            RPC_FireMoveOrderedEvents();
            if (!Object.HasStateAuthority)
            {
                return;
            }
            
            goToSilently(destination);
        }
        public void faceTowards(Vector3 target)
        {
            Vector3 direction = (target - this.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookRotation, Time.deltaTime * navAgent.angularSpeed);
        }

        public float pathLength(Vector3 point)
        {
            NavMeshPath path = new NavMeshPath();
            bool isReachable = navAgent.CalculatePath(point, path);
            float dist = 0f;
            for (int j = 0; j < path.corners.Length - 1; j++)
            {
                dist += Vector3.Distance(path.corners[j], path.corners[j + 1]);
            }

            return dist;
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority) {
                speed = GetComponent<NavMeshAgent>().velocity.magnitude;
            }
        }

        public void OnDestroy()
        {
            CancelInvoke();
            if (currentWatcher != null)
            {
                currentWatcher.Destroy();
            }
        }

        public void OnDisable()
        {
            CancelInvoke();
            if (currentWatcher != null)
            {
                currentWatcher.Destroy();
            }
        }

        private void debugNavMeshPath(Vector3[] points)
        {
            var lineRenderer = gameObject.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }
    }
}
