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
    public static float WAVE_TIME_BASE = 10f;
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

    private int wave = 1;

    private BarbarianWavePlayer()
    {
        this.colour = PlayerColours.Black;
    }
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

        int index = Random.RandomRange(0, spawners.Length);
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

    private void spawnUnitGroupToAttackNearestEnemy(Vector3 location, int amt)
    {
        AIUnitGrouping attackSquad = new AIUnitGrouping(this, amt, 1, location);

        // THIS IS WHERE WE ACTUALLY TELL THE SQUAD TO ATTACK
        //attackSquad.onMaxUnits.AddListener(attackSquad.attackNearestEnemy);
        attackSquad.onMaxUnits.AddListener(attackSquad.assaultRandomPlayer);
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
            singleton = new BarbarianWavePlayer();
        }

        return singleton;
    }
}
