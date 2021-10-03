using game.assets.ai;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAggregation : IAttack
{
    public List<Attack> units;

    public AttackAggregation(List<Attack> units)
    {
        this.units = units;
    }

    public AttackAggregation()
    {
        units = new List<Attack>();
    }

    public void attack(Health attackee)
    {
        units.ForEach(unit => unit.attack(attackee));
    }

    public void add(Attack unit)
    {
        if (!units.Contains(unit))
        {
            units.Add(unit);
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

    public void clear()
    {
        units.Clear();
    }
}
