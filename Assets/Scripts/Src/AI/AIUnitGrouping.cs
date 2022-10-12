using game.assets.utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                units.attack(nearestEnemyThingy);
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
