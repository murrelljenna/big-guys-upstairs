using game.assets.ai;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using game.assets.utilities;

namespace game.assets.routines
{
    public class ManyAttackManyJob : InterruptibleJob
    {
        private List<Attack> attackers;
        private List<Health> attackees;
        private int killCount = 0;
        private int attackerCount = 0;

        public UnityEvent allInvadersDead = new UnityEvent();

        public ManyAttackManyJob(List<Attack> attackers, List<Health> attackees)
        {
            this.attackers = attackers;
            this.attackees = attackees;
            this.attackerCount = attackees.Count;
        }

        private void attackRandomUnit(Attack unit)
        {
            Debug.Log("AC - Attacking random unit: ");
            Debug.Log("AC - Picking from one of " + attackees.Count + " random units");
            unit.attack(attackees.RandomElem());
        }

        protected override IEnumerator execute_impl()
        {
            attackers.ForEach((Attack unit) =>
            {
                void updateUnitCount(Health health)
                {
                    if (!attackees.Contains(health))
                    {
                        return;
                    }
                    Debug.Log("AC - Killed " + health.gameObject.name);
                    Debug.Log("AC - Enemies left " + attackees.Count);
                    attackees.Remove(health);
                    killCount++;
                    Debug.Log("AC - Killcount : " + killCount);
                    Debug.Log("AC - attackerCount : " + attackerCount);
                    if (attackees.Count == 0)
                    {
                        allInvadersDead.Invoke();
                        Debug.Log("AC - All invaders dead");
                    }
                    else
                    {
                        attackRandomUnit();
                    }
                }

                void attackRandomUnit()
                {
                    this.attackRandomUnit(unit);
                }

                // Give direction if unit is without something to do

                unit.idled.AddListener(attackRandomUnit);
                markForCleanup(unit.idled, attackRandomUnit);

                unit.enemyKilled.AddListener(updateUnitCount);
                markForCleanup(unit.enemyKilled, updateUnitCount);

                // Cleanup if this unit dies

                unit.GetComponent<Health>()?.onZeroHP.AddListener((Health h) =>
                {
                    unit.enemyKilled.RemoveListener(updateUnitCount);
                    unit.idled.RemoveListener(attackRandomUnit);
                    attackers.Remove(unit);
                });
            });

            if (attackers.Count > attackees.Count)
            {
                var ourStack = new Stack<Attack>(attackers);
                var theirStack = new Stack<Health>(attackees);

                int divisor = attackees.Count;
                int quotient = attackers.Count / divisor;
                int remainder = attackers.Count % divisor;

                for (int i = 0; i < divisor; i++)
                {
                    var target = theirStack.Pop();
                    for (int j = 0; j < quotient; j++)
                    {
                        var attacker = ourStack.Pop();
                        attacker.attack(target);
                    }
                }

                AttackRemainder(ourStack, attackees);
            }
            else
            {
                Debug.Log("AA - Outnumbered");
                Debug.Log("AA - Attacker count: " + attackers.Count);
                AttackRemainder(new Stack<Attack>(attackers), attackees);
            }

            yield return null;
        }

        private void AttackRemainder(Stack<Attack> attackers, List<Health> attackees)
        {
            while (attackers.Count > 0)
            {
                var attacker = attackers.Pop();
                attacker.attack(attackees.RandomElem());
            }
        }
    }
}