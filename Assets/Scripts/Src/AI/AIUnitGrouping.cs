using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets.ai {
    public class AIUnitGrouping
    {
        private player.Player player;
        private int maxUnits;
        private int recruitRateInSeconds;

        private AttackAggregation units = new AttackAggregation();
        private AIUnitRecruiter recruiter;

        private Vector3 location;

        public AIUnitGrouping(player.Player player, int maxUnits, int recruitRateInSeconds, Vector3 startingLocation) {
            this.maxUnits = maxUnits;
            this.recruitRateInSeconds = recruitRateInSeconds;
            this.player = player;
            this.location = startingLocation;
            recruiter = new AIUnitRecruiter(player);

            LocalGameManager.Get().StartCoroutine(startReplenishment(recruitRateInSeconds, recruiter));
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
                Attack unit = replenishUnit(location);
                units.add(unit);

                location = groupLocation();
            }
        }
    }
}
