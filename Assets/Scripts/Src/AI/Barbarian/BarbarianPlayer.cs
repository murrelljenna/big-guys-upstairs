using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.player;
using static game.assets.utilities.GameUtils;
using game.assets.ai;
using game.assets.spawners;

public class BarbarianPlayer : Player
{
    private List<AIUnitGrouping> squads = new List<AIUnitGrouping>();
    public BarbarianPlayer()
    {
        this.colour = PlayerColours.Black;
    }

    public void Awake()
    {
        Spawner[] spawners = GameObject.FindObjectsOfType<Spawner>();

        for (int i = 0; i < spawners.Length; i++)
        {
            if (spawners[i].BelongsTo(this))
            {
                Vector3 location = spawners[i].transform.position;
                AIUnitGrouping squad = new AIUnitGrouping(this, 15, 10, location);
                squads.Add(squad);
                BarbarianOwnership ownership = spawners[i].GetComponent<BarbarianOwnership>();

                void guard() {
                    squad.guardCurrentLocation();
                }

                if (!ownership.fortified)
                {
                    squad.onMaxUnits.AddListener(ownership.fortify);
                }
                squad.onMaxUnits.AddListener(guard);
                registerDisbandListener(squad);
            }
        }
    }

    private void registerDisbandListener(AIUnitGrouping grouping) {
        void disbandGrouping()
        {
            grouping.Disband();
            squads.Remove(grouping);
        }

        grouping.onNoUnits.AddListener(disbandGrouping);
    }
}
