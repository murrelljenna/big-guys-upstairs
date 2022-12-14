using game.assets;
using game.assets.ai;
using game.assets.economy;
using game.assets.utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AttackAggregation
{
    public List<Attack> units;
    public UnityEvent<Attack> unitDead;

    public UnityEvent<Vector3> locationReached = new UnityEvent<Vector3>();
    public UnityEvent<Attack> attacked = new UnityEvent<Attack>();
    public UnityEvent<Attack, Health> enemyKilled = new UnityEvent<Attack, Health>();
    public UnityEvent<Attack> unitIdled = new UnityEvent<Attack>();

    public AttackAggregation(List<Attack> units)
    {
        this.units = units;
        unitDead = new UnityEvent<Attack>();
        this.units.ForEach(addListeners);
    }

    private void addListeners(Attack unit) {
        registerDeathCallbackIfCanDie(unit);
        unit.GetComponent<Health>()?.onAttacked.AddListener((Attack atker) => attacked.Invoke(atker));
        unit.enemyKilled.AddListener((Health h) => enemyKilled.Invoke(unit, h));
        unit.idled.AddListener(() => unitIdled.Invoke(unit));
    }

    private void registerDeathCallbackIfCanDie(Attack unit) {
        Health health = unit.GetComponent<Health>();

        if (health != null)
        {
            health.onZeroHP.AddListener(removeOnUnitDied);
        }
    }

    public AttackAggregation()
    {
        units = new List<Attack>();
        unitDead = new UnityEvent<Attack>();
    }

    public void attack(Health attackee)
    {
        LocalGameManager.Get().StartCoroutine(attackPar(attackee));
    }

    public void Attack(Health[] attackees)
    {
        units.ForEach((Attack unit) => unit.idled.AddListener(() => attackRandom(unit, attackees)));

        if (units.Count > attackees.Length)
        {
            var ourStack = new Stack<Attack>(units);
            var theirStack = new Stack<Health>(attackees);

            int divisor = attackees.Length;
            int quotient = units.Count / divisor;
            int remainder = units.Count % divisor;

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
            AttackRemainder(new Stack<Attack>(units), attackees);
        }
    }

    private void attackRandom(Attack attacker, Health[] attackees)
    {
        var aliveUnits = attackees.filterNulls();
        if (aliveUnits.Length == 0)
        {
            return;
        }
        attacker.attack(aliveUnits.RandomElem());
    }

    private void AttackRemainder(Stack<Attack> attackers, Health[] attackees)
    {
        Debug.Log("AB - Attacking remainder: " + attackers.Count);
        for (int i = 0; i < attackers.Count; i++)
        {
            var attacker = attackers.Pop();
            attacker.attack(attackees[i]);
        }
    }

    public void allIdleAttack(Health attackee)
    {
        LocalGameManager.Get().StartCoroutine(attackParIfNotBusy(attackee));
    }

    private IEnumerator attackPar(Health attackee)
    {
        const int par = 3;
        for (int i = 0; i < units.Count; i++)
        {
            var unit = units[i];
            unit.attack(attackee);
            if (i % par == 0)
            {
                yield return null;
            }
        }
    }
    private IEnumerator attackParIfNotBusy(Health attackee)
    {
        const int par = 5;
        for (int i = 0; i < units.Count; i++)
        {
            var unit = units[i];
            if (!unit.isCurrentlyAttacking())
            {
                unit.attack(attackee);
            }
            if (i % par == 0)
            {
                yield return null;
            }
        }
    }

    private void removeOnUnitDied(Health health) {
        Attack attack = health.GetComponent<Attack>();
        units.Remove(attack);
        unitDead.Invoke(attack);
    }

    public void add(Attack unit)
    {
        if (!units.Contains(unit))
        {
            units.Add(unit);
            addListeners(unit);
        }
    }

    public void guard(Vector3 point, float radius)
    {
        unitsThatCanGuard().ForEach(unit => unit.guard(point, radius));
        unitsThatCanMove().goTo(point);
    }

    public void remove(Attack unit)
    {
        units.Remove(unit);
    }

    public bool contains(Attack unit)
    {
        return units.Contains(unit);
    }

    public MovementAggregation unitsThatCanMove()
    {
        List<Movement> unitsThatCanMove = new List<Movement>();
        units.ForEach(unit =>
        {
            Movement movement = unit.GetComponent<Movement>();
            if (movement != null)
            {
                unitsThatCanMove.Add(movement);
            }
        });

        var agg = new MovementAggregation(unitsThatCanMove);

        agg.locationReached.AddListener((Vector3 v) => locationReached.Invoke(v));

        return agg;
    }

    public List<Worker> unitsThatCanWork()
    {
        List<Worker> unitsThatCanWork = new List<Worker>();
        units.ForEach(unit =>
        {
            Worker movement = unit.GetComponent<Worker>();
            if (movement != null)
            {
                unitsThatCanWork.Add(movement);
            }
        });

        return unitsThatCanWork;
    }

    public List<Guard> unitsThatCanGuard()
    {
        List<Guard> unitsThatCanGuard = new List<Guard>();
        units.ForEach(unit =>
        {
            Guard guard = unit.GetComponent<Guard>();
            if (guard != null)
            {
                unitsThatCanGuard.Add(guard);
            }
        });

        return unitsThatCanGuard;
    }

    public Vector3 location()
    {
        if (units.Count > 0)
        {
            return units[0].transform.position;
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    public NavMeshAgent getMeSomeonesNavMeshAgent()
    {
        return units[0].GetComponent<NavMeshAgent>();
    }

    public int Count()
    {
        return units.Count;
    }

    public void clear()
    {
        units.ForEach((Attack unit) =>
        {
            unit.deselect();
        });
        units.Clear();
    }
}
