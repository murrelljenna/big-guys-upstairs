using System.Collections;
using UnityEngine;
using game.assets.ai;
using static game.assets.utilities.GameUtils;
using game.assets.spawners;
using game.assets;
using UnityEngine.Events;
using game.assets.utilities;

public static class BarbarianWaveSettings
{
    public static float WAVE_TIME_BASE = 60f;
    public static int BARBARIAN_WAVE_UNIT_COUNT_BASE = 10;
    public static int BARBARIAN_WAVE_COUNT = 10;
    public static int CALCULATE_NEW_UNIT_COUNT(int wave)
    {
        CURRENT_UNIT_COUNT = (BARBARIAN_WAVE_UNIT_COUNT_BASE + wave) * wave;
        return CURRENT_UNIT_COUNT;
    }

    public static int CURRENT_UNIT_COUNT = BARBARIAN_WAVE_UNIT_COUNT_BASE;
} 

public class BarbarianWavePlayer : BarbarianPlayer
{
    private static BarbarianWavePlayer singleton;

    public UnityEvent<int, int, int> nextWaveReady = new UnityEvent<int, int, int>();

    public UnityEvent lastBarbarianWaveDefeated = new UnityEvent();

    public new static BarbarianWavePlayer AsDevCube()
    {
        GameObject devCube = new GameObject();
        BarbarianWavePlayer player = (BarbarianWavePlayer)devCube.AddComponent(typeof(BarbarianWavePlayer));
        player.colour = PlayerColours.Black;

        return player;
    }

    private int wave = 1;

    IEnumerator waitToAttack(float delayTime, Spawner spawnPoint, int amt)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);

        spawnUnitGroupToAttackNearestEnemy(spawnPoint.transform.position, amt);
        //Do the action after the delay time has finished.
    }

    void attackIn30Seconds(Spawner spawnPoint, int amt) {
        // TODO: Update our wait values and unit count here
        nextWaveReady.Invoke(wave, (int)BarbarianWaveSettings.WAVE_TIME_BASE, amt);

        LocalGameManager.Get().StartCoroutine(waitToAttack(BarbarianWaveSettings.WAVE_TIME_BASE, spawnPoint, amt));
    }

    void AttackIn30SecondsFromRandomSpawnPoint(int amt)
    {
        if (wave > BarbarianWaveSettings.BARBARIAN_WAVE_COUNT)
        {
            lastBarbarianWaveDefeated.Invoke();
            return;
        }
        Spawner[] spawners = GameObject.FindObjectsOfType<Spawner>().filterFor<BarbarianOwnership, Spawner>();

        int index = Random.Range(0, spawners.Length);
        Spawner spawnPoint = spawners[index];

        if (spawnPoint.GetComponent<BarbarianOwnership>())
        {
            attackIn30Seconds(spawnPoint, amt);
            wave++;
        }
    }

    override public void Awake()
    {
        nextWave();
    }
    private void nextWave()
    {
        AttackIn30SecondsFromRandomSpawnPoint(BarbarianWaveSettings.CALCULATE_NEW_UNIT_COUNT(wave));
    }

    private void attackRandomPlayersRandomCity(AIUnitGrouping squad) {
        var city = LocalGameManager.Get().players.RandomElem().getCities().RandomElem();
        PositionArmyToAssaultPlan plan = new PositionArmyToAssaultPlan(squad, city);
        squad.Order(plan);
    }

    private void spawnUnitGroupToAttackNearestEnemy(Vector3 location, int amt)
    {
        AIUnitGrouping attackSquad = new AIUnitGrouping(this, amt, 0, location);

        // THIS IS WHERE WE ACTUALLY TELL THE SQUAD TO ATTACK
        //attackSquad.onMaxUnits.AddListener(attackSquad.attackNearestEnemy);
        attackSquad.onMaxUnits.AddListener(() => attackRandomPlayersRandomCity(attackSquad));

        attackSquad.ordersEmpty.AddListener(() => attackRandomPlayersRandomCity(attackSquad));

        void stopReplenishing()
        {
            attackSquad.stopReplenishing();
        }
        attackSquad.onMaxUnits.AddListener(stopReplenishing);

        registerDisbandListener(attackSquad);
    }

    private void registerDisbandListener(AIUnitGrouping grouping)
    {
        void disbandGrouping()
        {
            grouping.Disband();
        }

        grouping.onNoUnits.AddListener(disbandGrouping);
        grouping.onNoUnits.AddListener(nextWave);
    }

    public static BarbarianWavePlayer Get()
    {
        if (singleton == null)
        {
            singleton = BarbarianWavePlayer.AsDevCube();
        }

        return singleton;
    }
}
