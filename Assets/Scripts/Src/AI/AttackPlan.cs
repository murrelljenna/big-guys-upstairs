using game.assets.ai;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        a();
    }
}
/*
public class CityAttackPlan : IArmyPlan
{
    private Vector3 location;

    public CityAttackPlan(Vector3 location)
    {
        this.location = location;
    }

    public Vector3 moveToPoint()
    {
        return location;
    }

    IArmyPlan[] possibleNextMoves()
    {
        return new IArmyPlan[]
        {

        };
    }
}

public class CitizenAttackPlan : IArmyPlan
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

    IArmyPlan[] possibleNextMoves()
    {
        return new IArmyPlan[]
        {
            new CityAttackPlan(location)
        };
    }

public class ArmyAttackPlan : IArmyPlan
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
*/
