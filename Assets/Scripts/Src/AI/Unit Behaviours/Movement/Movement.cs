﻿using System.Collections;
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

        void Start()
        {
            navAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        void Update()
        {
            if (moveOrdered && navAgent.remainingDistance <= navAgent.stoppingDistance) {
                if (!navAgent.hasPath || Mathf.Abs(navAgent.velocity.sqrMagnitude) < float.Epsilon)
                {
                    reachDestination();
                }
            }
        }

        private void reachDestination()
        {
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
            navAgent.isStopped = true;
            navAgent.ResetPath();
            navAgent.isStopped = false;
            halt();
        }

        public void goToSilently(Vector3 destination)
        {
            moveOrdered = true;
            Debug.Log("Go to silently!");
            navAgent.SetDestination(destination);
        }

        public void goTo(Vector3 destination)
        {
            Debug.Log("Goto! : " + destination);
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
    }
}
