using game.assets.player;
using game.assets.utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace game.assets.ai {
    public class AIUnitGrouping
    {
        public player.Player player;
        private int maxUnits;
        private int recruitRateInSeconds;

        public AttackAggregation units = new AttackAggregation();
        private AIUnitRecruiter recruiter;

        private Vector3 location;

        public UnityEvent onMaxUnits;
        public UnityEvent onNoUnits;

        private bool autoReplenish;

        private Coroutine replenishment;

        public UnityEvent<Vector3> reachedDestination = new UnityEvent<Vector3>();
        private bool destinationHasBeenReached = false;

        private Stack<IArmyPlan> orders = new Stack<IArmyPlan>();
        public UnityEvent<IArmyPlan> newOrder = new UnityEvent<IArmyPlan>();
        public UnityEvent<Attack, Health> enemyKilled = new UnityEvent<Attack, Health>();
        public UnityEvent<Attack> unitIdled = new UnityEvent<Attack>();

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
            newOrder.AddListener((IArmyPlan _) => nextOrder());
            units.enemyKilled.AddListener((Attack a, Health h) => enemyKilled.Invoke(a, h));
            units.attacked.AddListener((Attack atker) =>
            {
                Interrupt(
                    new DefendAgainstAttackPlan(this, atker)
                    );
            });
            units.unitIdled.AddListener(bubbleIdleInvoke);
        }

        private void bubbleIdleInvoke(Attack unit) {
            unitIdled.Invoke(unit);
        }

        public MovementAggregation unitsThatCanMove()
        {
            return units.unitsThatCanMove();
        }

        public void attack(Health target)
        {
            units.attack(target);
        }

        public void attack(Health[] targets)
        {
            units.Attack(targets);
        }

        /*
         * Processing orders
         */

        public void Order(IArmyPlan order) {
            if (!order.possible())
            {
                return;
            }

            Debug.Log("AA - New order: " + order.name());
            Debug.Log("AA - Current orders: ");
            orders.debugPrintPlans();

            orders.Push(order);
            newOrder.Invoke(order);
            order.onComplete(orderComplete);
        }

        private void Interrupt(IArmyPlan order)
        {
            if (orders.Count > 0 && !orders.Peek().interruptible())
            {
                return;
            }

            Debug.Log("AA - Order interrupted, adding this one on the stack: " + order.name());
            Debug.Log("AA - Current orders now consist of: ");
            orders.debugPrintPlans();

            orders.Peek().cleanup();

            Order(order);
        }

        private void nextOrder()
        {
            if (orders.Count == 0)
            {
                return;
            }

            IArmyPlan order = orders.Peek();
            orders.debugPrintPlans();
            if (order.possible())
            {
                order.execute();
            }
        }

        private void orderComplete(IArmyPlan completedPlan)
        {
            Debug.Log("AA - Order complete!");
            if (orders.Count > 0)
            {
                IArmyPlan order = orders.Pop();
                Debug.Log("AA - Order complete, removing this one off the stack: " + order.name());
                Debug.Log("AA - Current orders now consist of: ");
                orders.debugPrintPlans();
            }
            completedPlan.cleanup();
            var subPlan = getPossibleNextMove(completedPlan);
            if (subPlan != null)
            {
                Debug.Log("AA - Extrapolating this from a recently completed plan: " + subPlan.name());
                Debug.Log("AA - Current orders consist of: ");
                orders.debugPrintPlans();
                Order(subPlan);
            }
            else
            {
                nextOrder();
            }
        }

        private IArmyPlan getPossibleNextMove(IArmyPlan plan) {
            var nextPlans = plan.possibleNextMoves();
            for (int i = 0; i < nextPlans.Length; i++)
            {
                var candidate = nextPlans[i];
                if (candidate.possible())
                {
                    return candidate;
                }
            }

            return null;
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

        public Vector3 groupLocation()
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
    }
}
