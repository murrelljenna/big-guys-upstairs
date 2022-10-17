﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using System;
using game.assets.utilities;

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
                ourMovement.reachedDestination.Invoke();
                Destroy();
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

        private DestinationWatcher currentWatcher;

        void Start()
        {
            navAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        }


        private void halt()
        {
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
            if (currentWatcher != null)
            {
                currentWatcher.Destroy();
                currentWatcher = null;
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
            if (currentWatcher != null)
            {
                currentWatcher.Destroy();
                currentWatcher = null;
            }
            currentWatcher = DestinationWatcher.Create(destination, col.radius, this);
            navAgent.SetDestination(destination);
            debugNavMeshPath(navAgent.path.corners);
        }

        public void goTo(Vector3 destination)
        {
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
            Debug.Log("Destroying thingie. Currentwatcher == null? : " + (currentWatcher == null).ToString());
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
            lineRenderer.SetWidth(0.05f, 0.05f);
            lineRenderer.SetColors(Color.red, Color.red);
            lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }
    }
}