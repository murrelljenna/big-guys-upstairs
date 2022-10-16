using game.assets;
using game.assets.ai;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using game.assets.utilities;



interface IInterruptibleJob
{
    void Execute();
    void Interrupt();
}

public abstract class InterruptibleJob : IInterruptibleJob {
    protected UnityEvent interrupted = new UnityEvent();
    private LocalGameManager gameManager;
    private Coroutine routine;

    public void Execute() {
        gameManager = LocalGameManager.Get();
        routine = gameManager.StartCoroutine(execute_impl());
    }

    protected abstract IEnumerator execute_impl();
    public void Interrupt()
    {
        Debug.Log("AB - Interrupted");
        gameManager.StopCoroutine(routine);
        interrupted.Invoke();
    }

    protected void markForCleanup(UnityEvent ev, UnityAction action)
    {
        interrupted.AddListener(() => ev.RemoveListener(action));
    }

    protected void markForCleanup<T>(UnityEvent<T> ev, UnityAction<T> action)
    {
        interrupted.AddListener(() => ev.RemoveListener(action));
    }

    ~InterruptibleJob()
    {
        Debug.Log("AB - Destructor");
        Interrupt();
    }
}

public class UnitPlacementJob : InterruptibleJob
{
    private int reachedLocation = 0;
    private int lastSent = 0;
    private Vector3 center;
    private Queue<Movement> units;
    private MovementAggregation movAgg;
    private const float unitSize = 0.2f;
    private const float gapSize = 0.3f;

    private bool fuckOff = false;

    public UnitPlacementJob(Vector3 center, MovementAggregation movAgg)
    {
        this.center = center;
        this.units = new Queue<Movement>(movAgg.units);
        this.movAgg = movAgg;

    }

    protected override IEnumerator execute_impl()
    {
        Queue<Vector3> points = new Queue<Vector3>();
        List<Vector3> taken = new List<Vector3>();
        points.Enqueue(center);

        float branchSize = unitSize / 2f + gapSize;
        Vector3[] positionMods = new Vector3[8] {
            new Vector3(0, 0, branchSize),
            new Vector3(branchSize, 0, branchSize),
            new Vector3(branchSize, 0, 0),
            new Vector3(branchSize, 0, -branchSize),
            new Vector3(0, 0, -branchSize),
            new Vector3(-branchSize, 0, -branchSize),
            new Vector3(-branchSize, 0, 0),
            new Vector3(-branchSize, 0, branchSize)
        };

        int runCount = 0;
        int j = 0;
        while (units.Count > 0)
        {
            Movement unit = units.Dequeue();

            if (unit == null)
            {
                continue;
            }


            Vector3 destination = points.Dequeue();


            void destinationReached()
            {
                reachedLocation++;
                if (reachedLocation == lastSent && units.Count == 0 && !fuckOff)
                {
                    j++;
                    movAgg.locationReached.Invoke(center);
                    Interrupt();
                    fuckOff = true;
                }
                unit.reachedDestination.RemoveListener(destinationReached);
            }

            void lessLastSent(Health _)
            {
                lastSent--;
                unit.GetComponent<Health>()?.onZeroHP?.RemoveListener(lessLastSent);
            }

            unit.goTo(destination);
            taken.Add(destination);

            unit.reachedDestination.AddListener(destinationReached);
            var zeroHpEvent = unit.GetComponent<Health>()?.onZeroHP;
            markForCleanup(unit.reachedDestination, destinationReached);

            if (zeroHpEvent != null)
            {
                var action = new UnityAction<Health>(lessLastSent);
                zeroHpEvent.AddListener(lessLastSent);
                markForCleanup(zeroHpEvent, action);
            }

            lastSent++;

            for (int i = 0; i < positionMods.Length; i++)
            {
                Vector3 modifiedPosition = destination + positionMods[i];
                float height = GameUtils.getTerrainHeight(modifiedPosition);

                NavMeshHit hit;
                // TODO: Fix magic 0.1f float
                var isOnMesh = NavMesh.SamplePosition(modifiedPosition, out hit, 0.1f, NavMesh.AllAreas);
                if (!alreadyTaken(taken, modifiedPosition) && modifiedPosition != destination && Math.Abs(height) - Math.Abs(center.y) < 3 && Math.Abs(height) - Math.Abs(center.y) > -3 && isOnMesh)
                {
                    points.Enqueue(modifiedPosition);
                    taken.Add(modifiedPosition);
                }
            }

            runCount++;
            if (runCount % 2 == 0)
            {
                yield return null;
            }
        }
    }
    private static bool alreadyTaken(List<Vector3> vectors, Vector3 target)
    {
        return !vectors.TrueForAll((Vector3 v) => Vector3.Distance(v, target) > 0.3f);
    }
}

public class UnitGroupingMovementJob : InterruptibleJob
{
    private MovementAggregation units;
    private Vector3 destination;
    private bool destinationHasBeenReached = false;
    public UnityEvent<Vector3> reachedDestination = new UnityEvent<Vector3>();

    public UnitGroupingMovementJob(Vector3 point, MovementAggregation units)
    {
        this.units = units;
        this.destination = point;
    }

    protected override IEnumerator execute_impl()
    {
        return getPathAndMoveAlong(destination);
    }

    private void moveUnitsToLocation(Vector3 location)
    {
        destinationHasBeenReached = false;
        units.goTo(location);
        units.locationReached.AddListener(setDestinationReached);
        markForCleanup(units.locationReached, setDestinationReached);
    }

    private void setDestinationReached(Vector3 _)
    {
        destinationHasBeenReached = true;
        units.locationReached.RemoveListener(setDestinationReached);
    }

    private IEnumerator getPathAndMoveAlong(Vector3 point)
    {
        var loc = units.location();
        NavMeshAgent agent = units.getMeSomeonesNavMeshAgent();
        NavMeshPath path = new NavMeshPath();
        bool isReachable = agent.CalculatePath(point, path);
        Vector3[] corners = path.corners;
        return moveAlongPoints(corners);

    }

    private IEnumerator moveAlongPoints(Vector3[] points)
    {
        debugNavMeshPath(points);
        for (int i = 0; i < points.Length; i++)
        {
            var loc = points[i];
            moveUnitsToLocation(loc);
            destinationHasBeenReached = false; // Will be set once callback gets called

            // Not necessary to do this in a for loop but whatevs
            yield return new WaitUntil(() => destinationHasBeenReached);
        }

        reachedDestination.Invoke(points[points.Length - 1]);
    }

    private void debugNavMeshPath(Vector3[] points)
    {
        Debug.Log("Debugging nav mesh path for AIUnitGrouping. Point count: " + points.Length);
        var lineRenderer = LocalGameManager.Get().gameObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = LocalGameManager.Get().gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.SetWidth(0.2f, 0.2f);
        lineRenderer.SetColors(Color.yellow, Color.yellow);
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }
}

public class ManyAttackManyJob : InterruptibleJob
{
    private List<Attack> attackers;
    private List<Health> attackees;
    private int killCount = 0;
    private int attackerCount = 0;

    public UnityEvent allInvadersDead = new UnityEvent();

    public ManyAttackManyJob(List<Attack> attackers, List<Health> attackees)
    {
        this.attackers = attackers;
        this.attackees = attackees;
        this.attackerCount = attackees.Count;
    }

    private void attackRandomUnit(Attack unit)
    {
        var aliveUnits = attackees.filterNulls(); // TODO: Shouldn't be necessary???
        if (aliveUnits.Count == 0)
        {
            return;
        }
        unit.attack(aliveUnits.RandomElem());
    }

    protected override IEnumerator execute_impl()
    {
        attackers.ForEach((Attack unit) => {
            void updateUnitCount(Health health)
            {
                if (!attackees.Contains(health)) {
                    return;
                }
                Debug.Log("AC - Killed " + health.gameObject.name);
                Debug.Log("AC - Enemies left " + attackees.Count);
                attackees.Remove(health);
                killCount++;
                Debug.Log("AC - Killcount : " + killCount);
                Debug.Log("AC - attackerCount : " + attackerCount);
                if (attackees.Count == 0)
                {
                    allInvadersDead.Invoke();
                    Debug.Log("AC - All invaders dead");
                }
                else
                {
                    attackRandomUnit();
                }
            }

            void attackRandomUnit() {
                this.attackRandomUnit(unit);
            }
            unit.idled.AddListener(attackRandomUnit);
            markForCleanup(unit.idled, attackRandomUnit);

            unit.enemyKilled.AddListener(updateUnitCount);
            markForCleanup(unit.enemyKilled, updateUnitCount);
        });

        if (attackers.Count > attackees.Count)
        {
            var ourStack = new Stack<Attack>(attackers);
            var theirStack = new Stack<Health>(attackees);

            int divisor = attackees.Count;
            int quotient = attackers.Count / divisor;
            int remainder = attackers.Count % divisor;

            for (int i = 0; i < divisor; i++)
            {
                var target = theirStack.Pop();
                for (int j = 0; j < quotient; j++)
                {
                    var attacker = ourStack.Pop();
                    attacker.attack(target);
                }
            }

            AttackRemainder(ourStack, attackees);
        }
        else
        {
            AttackRemainder(new Stack<Attack>(attackers), attackees);
        }

        yield return null;
    }

    private void AttackRemainder(Stack<Attack> attackers, List<Health> attackees)
    {
        for (int i = 0; i < attackers.Count; i++)
        {
            var attacker = attackers.Pop();
            attacker.attack(attackees[i]);
        }
    }
}
