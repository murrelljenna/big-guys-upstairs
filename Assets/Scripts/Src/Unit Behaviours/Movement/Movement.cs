using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using System;

namespace game.assets.ai

{
    public interface IMovement
    {
        void goTo(Vector3 destination);
        void stop();
    }

    public class DestinationWatcher : MonoBehaviour
    {
        private Movement ourMovement;
        public void SetState(Movement movement)
        {
            ourMovement = movement;
        }

        public static void Create(Vector3 pos, float radius, Movement movement) {
            var obj = new GameObject("Movement Collider for unit");
            SphereCollider col = obj.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = radius;
            obj.transform.position = pos;
            var watcher = obj.AddComponent<DestinationWatcher>();
            watcher.SetState(movement);
        }

        private void OnTriggerEnter(Collider other)
        {
            var otherMovementMaybe = other.gameObject.GetComponent<Movement>();
            if (otherMovementMaybe != null && otherMovementMaybe == ourMovement) {
                ourMovement.reachedDestination.Invoke();
                Destroy(this.gameObject);
            }
        }
    }

    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    public class Movement : MonoBehaviour, IMovement
    {
        private UnityEngine.AI.NavMeshAgent navAgent;

        public bool moveOrdered = false;

        [Tooltip("Invoked when destination reached")]
        public UnityEvent reachedDestination;

        [Tooltip("Invoked when halted")]
        public UnityEvent halted;

        [Tooltip("Invoked when ordered to move to new position, but before actual orders are set.")]
        public UnityEvent newMoveOrdered;

        void Awake()
        {
            var ev = new UnityEvent();
            var otherfuckingthing = new UnityEvent();

            void shouldNotRun()
            {
                Debug.Log("THIS SHOULD BE PRINTING");
            }

            ev.AddListener(shouldNotRun);
            otherfuckingthing.AddListener(() => fuckyou(ev, new UnityAction(shouldNotRun)));
            otherfuckingthing.Invoke();
            ev.Invoke();
        }

        private void fuckyou(UnityEvent ev, UnityAction a)
        {
            ev.RemoveListener(a);
        }

        void Start()
        {
            navAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        private static int fuckyouasshole = 0;
        private static int andfuckyoumostofall = 0;

        void Update()
        {
            /*if (moveOrdered && navAgent.remainingDistance <= navAgent.stoppingDistance) {
                if (!navAgent.hasPath || Mathf.Abs(navAgent.velocity.sqrMagnitude) < float.Epsilon)
                {
                    fuckyouasshole++;
                    reachDestination();
                }
            }*/
        }

        private void reachDestination()
        {
            Debug.Log(" AB - Reached destination: " + fuckyouasshole);
            moveOrdered = false;
            reachedDestination.Invoke();
        }

        private void halt()
        {
            moveOrdered = false;
            halted.Invoke();
        }

        public void stop()
        {
            if (navAgent.isOnNavMesh)
            {
                navAgent.isStopped = true;
                navAgent.ResetPath();
                navAgent.isStopped = false;
            }
            halt();
        }

        public void goToSilently(Vector3 destination)
        {
            moveOrdered = true;
            CapsuleCollider col = GetComponent<CapsuleCollider>();
            if (col == null)
            {
                Debug.LogError("Buddy you fucked up. This movement needs a capsule collider or you need to write code that works with other collider.");
            }
            DestinationWatcher.Create(destination, col.radius, this);
            navAgent.SetDestination(destination);
            debugNavMeshPath(navAgent.path.corners);
        }

        public void goTo(Vector3 destination)
        {
            andfuckyoumostofall++;
            Debug.Log("AB Goto! : " + andfuckyoumostofall);
            newMoveOrdered.Invoke();
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

        public float speed()
        {
            return GetComponent<NavMeshAgent>().velocity.magnitude;
        }

        public void OnDestroy()
        {
            CancelInvoke();
        }

        public void OnDisable()
        {
            CancelInvoke();
        }

        private void debugNavMeshPath(Vector3[] points)
        {
            var lineRenderer = gameObject.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }
            lineRenderer.SetWidth(0.05f, 0.05f);
            lineRenderer.SetColors(Color.red, Color.red);
            lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }
    }
}
