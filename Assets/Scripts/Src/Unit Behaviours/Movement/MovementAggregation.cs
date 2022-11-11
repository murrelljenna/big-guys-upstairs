using game.assets.ai;
using game.assets.routines;
using game.assets.utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class MovementAggregation
{
    public List<Movement> units = new List<Movement>();

    public UnityEvent<Vector3> locationReached = new UnityEvent<Vector3>();


    private InterruptibleJob job;

    public MovementAggregation(List<Movement> units)
    {
        this.units = units;
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

    public void goTo(Vector3 destination)
    {
        if (units.Count > 0)
        {
            haltPlaceUnits();
            job = new UnitPlacementJob(destination, this);
            job.Execute();
        }
    }



    public void stop()
    {
        units.ForEach(unit => unit.stop());
        haltPlaceUnits();
    }

    private void haltPlaceUnits()
    {
        if (job != null)
        {
            job.Interrupt();
        }
    }
}
