using game.assets.ai;
using game.assets.utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovementAggregation : IMovement
{
    public List<Movement> units = new List<Movement>();

    public UnityEvent<Vector3> locationReached = new UnityEvent<Vector3>();


    private InterruptibleJob job;

    public MovementAggregation(List<Movement> units)
    {
        this.units = units;
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
