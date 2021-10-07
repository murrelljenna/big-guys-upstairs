using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.player;
using static game.assets.utilities.GameUtils;
using game.assets.ai;

public class BarbarianPlayer : Player
{
    private AIUnitGrouping units;
    public BarbarianPlayer()
    {
        this.colour = PlayerColours.Black;
        // Find Resource where we can spawn a thing
    }

    public void Awake()
    {
        units = new AIUnitGrouping(this, 15, 10, new Vector3(0, 0, 0));
    }
}
