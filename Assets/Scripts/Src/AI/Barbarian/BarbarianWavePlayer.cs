using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.assets.player;
using game.assets.ai;
using static game.assets.utilities.GameUtils;
using game.assets.spawners;
using game.assets;
using UnityEngine.Events;

public static class BarbarianWaveSettings
{
    public static float WAVE_TIME_BASE = 30f;
    public static int BARBARIAN_WAVE_COUNT_BASE = 10;
} 

public class BarbarianWavePlayer : BarbarianPlayer
{
    private static BarbarianWavePlayer singleton;

    public UnityEvent<int, int> nextWaveReady = new UnityEvent<int, int>();

    private BarbarianWavePlayer()
    {
        this.colour = PlayerColours.Black;
    }
    IEnumerator waitToAttack(float delayTime, Spawner spawnPoint)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);

        spawnUnitGroupToAttackNearestEnemy(spawnPoint.transform.position);
        //Do the action after the delay time has finished.
    }

    void attackIn30Seconds(Spawner spawnPoint) {
        // TODO: Update our wait values and unit count here
        nextWaveReady.Invoke((int)BarbarianWaveSettings.WAVE_TIME_BASE, BarbarianWaveSettings.BARBARIAN_WAVE_COUNT_BASE);

        LocalGameManager.Get().StartCoroutine(waitToAttack(30f, spawnPoint));
    }

    void AttackIn30SecondsFromRandomSpawnPoint()
    {
        Spawner[] spawners = GameObject.FindObjectsOfType<Spawner>();

        int index = Random.RandomRange(0, spawners.Length);
        Spawner spawnPoint = spawners[index];

        if (spawnPoint.GetComponent<BarbarianOwnership>())
        {
            attackIn30Seconds(spawnPoint);
        }
    }

    override public void Awake()
    {
        AttackIn30SecondsFromRandomSpawnPoint();
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
        grouping.onNoUnits.AddListener(AttackIn30SecondsFromRandomSpawnPoint);
    }

    public static BarbarianWavePlayer Get()
    {
        if (singleton == null)
        {
            singleton = new BarbarianWavePlayer();
        }

        return singleton;
    }
}
