using game.assets.player;
using game.assets.utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace game.assets.ai {
    public class AIUnitGrouping
    {
        private player.Player player;
        private int maxUnits;
        private int recruitRateInSeconds;

        private AttackAggregation units = new AttackAggregation();
        private AIUnitRecruiter recruiter;

        private Vector3 location;

        public UnityEvent onMaxUnits;
        public UnityEvent onNoUnits;

        private bool autoReplenish;

        private Coroutine replenishment;

        public UnityEvent<Vector3> reachedDestination = new UnityEvent<Vector3>();
        private bool destinationHasBeenReached = false;

        public AIUnitGrouping(player.Player player, int maxUnits, int recruitRateInSeconds, Vector3 startingLocation, bool autoReplenish = true) {
            onMaxUnits = new UnityEvent();
            onNoUnits = new UnityEvent();
            this.maxUnits = maxUnits;
            this.recruitRateInSeconds = recruitRateInSeconds;
            this.player = player;
            this.location = startingLocation;
            recruiter = new AIUnitRecruiter(player);
            units.unitDead.AddListener(reportIfNoUnits);
            this.autoReplenish = autoReplenish;

            replenishment = LocalGameManager.Get().StartCoroutine(startReplenishment(recruitRateInSeconds, recruiter));
        }

        public void stopReplenishing()
        {
            autoReplenish = false;
        }

        private void reportIfNoUnits(Attack attack)
        {
            if (units.Count() < 1)
            {
                onNoUnits.Invoke();
            }
        }

        public void guardCurrentLocation()
        {
            units.guard(groupLocation(), 3f);
        }

        private Attack replenishUnit(Vector3 location)
        {
            GameObject unit = recruiter.InvokeSpawn(location);
            Attack unitAttack = unit.GetComponent<Attack>();

            if (unitAttack != null)
            {
                return unitAttack;
            }
            else
            {
                Debug.LogError("AIUnitRecruiter recruiting unit gameobjects without Attack component");
                return null;
            }
        }

        private Vector3 groupLocation()
        {
            return units.location();
        }

        IEnumerator startReplenishment(float time, AIUnitRecruiter recruiter)
        {

            while (true)
            {
                yield return new WaitForSecondsRealtime(time);
                if (autoReplenish && units.Count() < maxUnits)
                {
                    Attack unit = replenishUnit(location);
                    units.add(unit);

                    if (units.Count() == maxUnits)
                    {
                        onMaxUnits.Invoke();
                    }

                }

                location = groupLocation();
            }
        }

        public void attackNearestEnemy()
        {
            attackNearestEnemy(null);
        }

        public void attackNearestEnemy(Health ignore = null)
        {
            LocalGameManager.Get().StartCoroutine(attackNearestEnemyInOneSecond());
        }

        private IEnumerator attackNearestEnemyInOneSecond()
        {
            yield return new WaitForSecondsRealtime(1);
            Health nearestEnemyThingy = getNearestEnemy();
            if (nearestEnemyThingy != null)
            {
                units.allIdleAttack(nearestEnemyThingy);
                nearestEnemyThingy.onZeroHP.AddListener(attackNearestEnemy);
            }
        }

        public void assaultRandomPlayer()
        {
            moveToAssault(LocalGameManager.Get().players.RandomElem());
        }

        public void moveToAssault(Player player)
        {
            var collider = player.getCities()[0]?.transform?.Find("AI")?.GetComponent<Collider>();
            if (collider == null) {
                Debug.LogError("Player's starting city either doesn't exist (unlikely) or doesn't have an AI child object with a collider");
            }
            getPathAndMoveAlong(collider.ClosestPointOnBounds(groupLocation()));
        }

        private Health getNearestEnemy()
        {
            Health[] healths = GameObject.FindObjectsOfType<Health>();
            Transform tMin = null;
            float minDist = Mathf.Infinity;
            Vector3 currentPos = groupLocation();
            foreach (Health health in healths)
            {
                if (health.IsEnemyOf(player))
                {
                    float dist = Vector3.Distance(health.transform.position, currentPos);
                    if (dist < minDist)
                    {
                        tMin = health.transform;
                        minDist = dist;
                    }
                }
            }
            return tMin?.GetComponent<Health>();
        }

        public void Disband()
        {
            LocalGameManager.Get().StopCoroutine(replenishment);
        }

        /// <summary>
        /// This should move guys along a more reasonable path
        /// </summary>
        /// 

        interface IAttackPlan {
            Vector3 moveToPoint();
        }

        private class CityAttackPlan : IAttackPlan {
            private Vector3 location;

            public CityAttackPlan(Vector3 location)
            {
                this.location = location;
            }

            public Vector3 moveToPoint()
            {
                return location;
            }
        }

        private class CitizenAttackPlan : IAttackPlan
        {
            private Vector3 location;
            private Health[] unitsToAttack;

            public CitizenAttackPlan(Vector3 location)
            {
                this.location = location;
            }

            public Vector3 moveToPoint()
            {
                return location;
            }

            public Health[] citizensToAttack()
            {
                return unitsToAttack;
            }
        }

        private class ArmyAttackPlan : IAttackPlan
        {
            private Vector3 location;
            private Health[] unitsToAttack;

            public ArmyAttackPlan(Vector3 location)
            {
                this.location = location;
            }

            public Vector3 moveToPoint()
            {
                return location;
            }

            public Health[] citizensToAttack()
            {
                return unitsToAttack;
            }
        }

        private void moveUnitsToLocation(Vector3 location) {
            destinationHasBeenReached = false;
            var agg = units.unitsThatCanMove();
            agg.goTo(location);
            agg.locationReached.AddListener((Vector3 v) =>
            {
                destinationHasBeenReached = true;
            });
        }

        public void getPathAndMoveAlong(Vector3 point)
        {
            var loc = units.location();
            NavMeshAgent agent = units.getMeSomeonesNavMeshAgent();
            NavMeshPath path = new NavMeshPath();
            bool isReachable = agent.CalculatePath(point, path);
            Debug.Log("Is reachable: " + isReachable);
            Vector3[] corners = path.corners;
            Debug.Log("Coroutine started!");
            LocalGameManager.Get().StartCoroutine(moveAlongPoints(corners));

        }

        private IEnumerator moveAlongPoints(Vector3[] points)
        {
            Debug.Log(points.Length);
            for (int i = 0; i < points.Length; i++)
            {
                var loc = points[i];
                Debug.Log("Going to: " + loc);
                moveUnitsToLocation(loc);
                destinationHasBeenReached = false; // Will be set once callback gets called

                // Not necessary to do this in a for loop but whatevs
                yield return new WaitUntil(() => destinationHasBeenReached);
            }
        }
    }
}
