using game.assets.ai;
using game.assets.economy;
using game.assets.utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IArmyPlan
{
    IArmyPlan[] possibleNextMoves();
    bool possible();
    void execute();
    void onComplete(Action a);
}

public class PositionArmyToAssaultPlan : IArmyPlan {
    private AIUnitGrouping army;
    private GameObject city;
    private Vector3 targetPosition;

    public PositionArmyToAssaultPlan(AIUnitGrouping army, GameObject city)
    {
        this.city = city;
        this.army = army;

        this.targetPosition = getPositionForCity(city);
    }

    public IArmyPlan[] possibleNextMoves()
    {
        return new IArmyPlan[]
        {
            new AttackArmyAroundCityPlan(army, city),
            new AttackCitizensAroundCityPlan(army, city),
            new CityAttackPlan(army, city)
        };
    }

    public void execute()
    {
        army.getPathAndMoveAlong(targetPosition);
    }

    public bool possible()
    {
        return (city != null);
    }

    private Vector3 getPositionForCity(GameObject city)
    {
        var collider = city?.transform?.Find("AI")?.GetComponent<Collider>();

        if (collider == null)
        {
            Debug.LogError("Player's starting city either doesn't exist (unlikely) or doesn't have an AI child object with a collider");
        }

        return collider.ClosestPointOnBounds(army.groupLocation());
    }

    public void onComplete(Action a)
    {
        army.reachedDestination.AddListener((Vector3 v) => a());
    }
}

public class CityAttackPlan : IArmyPlan
{
    private AIUnitGrouping army;
    private GameObject city;

    public CityAttackPlan(AIUnitGrouping army, GameObject city)
    {
        this.army = army;
        this.city = city;
    }

    public IArmyPlan[] possibleNextMoves()
    {
        return new IArmyPlan[]
        {

        };
    }

    public bool possible()
    {
        return (city != null);
    }

    public void onComplete(Action a)
    {
        city.GetComponent<Health>().onZeroHP.AddListener((Health _) => a());
    }

    public void execute()
    {
        army.attack(city.GetComponent<Health>());
    }
}

public class AttackCitizensAroundCityPlan : IArmyPlan
{
    private AIUnitGrouping army;
    private GameObject city;
    private List<Health> units;

    private const float CITY_RANGE = 15f;
    private const int UNIT_THRESHOLD = 9;

    public AttackCitizensAroundCityPlan(AIUnitGrouping army, GameObject city)
    {
        this.army = army;
        this.city = city;
    }

    public IArmyPlan[] possibleNextMoves()
    {
        return new IArmyPlan[]
        {
            new AttackArmyAroundCityPlan(army, city),
            new CityAttackPlan(army, city)
        };
    }

    public bool possible()
    {
        units = getUnits(city);
        return (city != null && units.Count > UNIT_THRESHOLD);
    }

    private List<Health> getUnits(GameObject city)
    {
        return new List<Health>(GameUtils.findEnemyUnitsInRange(city.transform.position, CITY_RANGE).thatBelongTo(city).filterFor<Worker, Health>());
    }

    public void onComplete(Action a)
    {
        army.enemyKilled.AddListener((Attack unit, Health health) =>
        {
            units.Remove(health);
            if (units.Count == 0)
            {
                a();
            }
            else
            {
                unit.attack(units.RandomElem());
            }
        }

      );
    }

    public void execute()
    {
        units = getUnits(city);
        army.attack(units.ToArray());
    }
}
    
public class AttackArmyAroundCityPlan : IArmyPlan
{
    private AIUnitGrouping army;
    private GameObject city;
    private List<Health> units;

    private const float CITY_RANGE = 15f;
    private const int UNIT_THRESHOLD = 7;

    public AttackArmyAroundCityPlan(AIUnitGrouping army, GameObject city)
    {
        this.army = army;
        this.city = city;
    }

    public IArmyPlan[] possibleNextMoves()
    {
        return new IArmyPlan[]
        {
            new AttackArmyAroundCityPlan(army, city),
            new AttackCitizensAroundCityPlan(army, city),
            new CityAttackPlan(army, city)
        };
    }

    public bool possible()
    {
        units = getUnits(city);
        return (city != null && units.Count > UNIT_THRESHOLD);
    }

    private List<Health> getUnits(GameObject city)
    {
        return new List<Health>(GameUtils.findEnemyUnitsInRange(city.transform.position, CITY_RANGE).thatBelongTo(city).filterAgainsts<Worker, Health>());
    }

    public void onComplete(Action a)
    {
        army.enemyKilled.AddListener((Attack unit, Health health) =>
        {
            units.Remove(health);
            if (units.Count == 0)
            {
                a();
            }
            else
            {
                unit.attack(units.RandomElem());
            }
        }
      );
    }

    public void execute()
    {
        units = getUnits(city);
        army.attack(units.ToArray());
    }
}
