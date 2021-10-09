using game.assets.ai;
using game.assets.economy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAggregation : IAttack
{
    public List<Attack> units;

    public AttackAggregation(List<Attack> units)
    {
        this.units = units;

        this.units.ForEach(unit => registerDeathCallbackIfCanDie(unit));
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
    }

    public void attack(Health attackee)
    {
        units.ForEach(unit => unit.attack(attackee));
    }

    public void removeOnUnitDied(Health health) {
        units.Remove(health.GetComponent<Attack>());
    }

    public void add(Attack unit)
    {
        if (!units.Contains(unit))
        {
            units.Add(unit);
            registerDeathCallbackIfCanDie(unit);
        }
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
        return new MovementAggregation(unitsThatCanMove);
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

    public int Count()
    {
        return units.Count;
    }

    public void clear()
    {
        units.Clear();
    }
}
