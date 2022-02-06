using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.player;
using game.assets.ai;
using static game.assets.utilities.GameUtils;
using game.assets.spawners;

public class BarbarianWavePlayer : BarbarianPlayer
{
    public BarbarianWavePlayer()
    {
        this.colour = PlayerColours.Black;
    }

    override public void Awake()
    {
        Spawner[] spawners = GameObject.FindObjectsOfType<Spawner>();

        int index = Random.RandomRange(0, spawners.Length);
        Spawner spawnPoint = spawners[index];

        if (spawnPoint.GetComponent<BarbarianOwnership>())
        {
            spawnUnitGroupToAttackNearestEnemy(spawnPoint.transform.position);
        }
    }

    private void spawnUnitGroupToAttackNearestEnemy(Vector3 location)
    {
        AIUnitGrouping attackSquad = new AIUnitGrouping(this, 10, 1, location);
        attackSquad.onMaxUnits.AddListener(attackSquad.attackNearestEnemy);
        registerDisbandListener(attackSquad);
    }

    private void registerDisbandListener(AIUnitGrouping grouping)
    {
        void disbandGrouping()
        {
            grouping.Disband();
        }

        grouping.onNoUnits.AddListener(disbandGrouping);
    }
}
