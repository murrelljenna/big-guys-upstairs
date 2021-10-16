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

        private Coroutine replenishment;

        public AIUnitGrouping(player.Player player, int maxUnits, int recruitRateInSeconds, Vector3 startingLocation) {
            onMaxUnits = new UnityEvent();
            onNoUnits = new UnityEvent();
            this.maxUnits = maxUnits;
            this.recruitRateInSeconds = recruitRateInSeconds;
            this.player = player;
            this.location = startingLocation;
            recruiter = new AIUnitRecruiter(player);
            units.unitDead.AddListener(reportIfNoUnits);

            replenishment = LocalGameManager.Get().StartCoroutine(startReplenishment(recruitRateInSeconds, recruiter));
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
                if (units.Count() < maxUnits)
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

        public void Disband()
        {
            Debug.Log(replenishment);
            Debug.Log("Disbanding");
            LocalGameManager.Get().StopCoroutine(replenishment);
        }
    }
}
