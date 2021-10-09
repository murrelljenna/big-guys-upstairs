using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.player;
using static game.assets.utilities.GameUtils;
using game.assets.ai;
using game.assets.spawners;

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
        Spawner[] spawners = GameObject.FindObjectsOfType<Spawner>();

        for (int i = 0; i < spawners.Length; i++)
        {
            if (spawners[i].BelongsTo(this))
            {
                Vector3 location = spawners[i].transform.position;
                units = new AIUnitGrouping(this, 15, 10, location);

                units.onMaxUnits.AddListener(fortify);
            }
        }
    }

    private void fortify()
    {

    }
}
