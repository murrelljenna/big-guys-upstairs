using System.Collections;
using UnityEngine;
using game.assets.ai;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using game.assets;

public class TestAttackAggregationGuards : MonoBehaviour
{
    private const string sceneName = "TestGuard.unity";
    private Guard guard;
    private Health intruder;

    private Vector3 pointInRadius;
    private Vector3 pointOutsideRadius;
    private Vector3 guardCenter;
    private AttackAggregation agg = new AttackAggregation();

    public void putIntruderInRadius(Health intruder)
    {
        intruder.gameObject.transform.position = pointInRadius;
    }

    public void putIntruderOutsideRadius(Health intruder)
    {
        intruder.gameObject.transform.position = pointOutsideRadius;
    }

    private bool attackeeTookDamage = false;
    public void onTookDamageCallback(float hp, float maxHp)
    {
        Debug.Log(attackeeTookDamage);
        attackeeTookDamage = true;
    }

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        EditorSceneManager.LoadSceneInPlayMode(
            Test.TestUtils.testSceneDirPath + sceneName,
            new LoadSceneParameters(LoadSceneMode.Single)
        );

        yield return null;

        guard = GameObject.Find("Guard").GetComponent<Guard>();
        intruder = GameObject.Find("Intruder").GetComponent<Health>();

        agg.add(guard.GetComponent<Attack>());

        guard.SetAsMine();
        intruder.SetAsPlayer(LocalGameManager.Get().barbarianPlayer);

        pointInRadius = GameObject.Find("InsideRadius").transform.position;
        pointOutsideRadius = GameObject.Find("OutsideRadius").transform.position;
        guardCenter = GameObject.Find("GuardCenter").transform.position;

        attackeeTookDamage = false;

        yield return new EnterPlayMode();
    }

    [UnityTest, Order(1)]
    public IEnumerator testIntruderAttackedInsideRadius()
    {
        agg.guard(guardCenter, 5f);
        intruder.onLowerHP.AddListener(onTookDamageCallback);
        putIntruderInRadius(intruder);
        Debug.Log(intruder.HP);
        yield return new WaitForSeconds(10);
        Debug.Log(intruder.HP);
        Assert.True(attackeeTookDamage);
    }

    [UnityTest, Order(2)]
    public IEnumerator testIntruderNotAttackedOutsideRadius()
    {
        agg.guard(guardCenter, 2f);
        intruder.onLowerHP.AddListener(onTookDamageCallback);
        putIntruderOutsideRadius(intruder);
        yield return new WaitForSeconds(3);
        Assert.False(attackeeTookDamage);
    }

    [UnityTest, Order(3)]
    public IEnumerator testGuardReturnsToPointWhenIntruderLeaves()
    {
        agg.guard(guardCenter, 5f);
        intruder.onLowerHP.AddListener(onTookDamageCallback);
        putIntruderInRadius(intruder);
        yield return new WaitForSeconds(5);
        Assert.True(attackeeTookDamage);
        putIntruderOutsideRadius(intruder);
        yield return new WaitForSeconds(5);
        Assert.True(Vector3.Distance(guard.transform.position, guardCenter) < 0.2f);
    }
}
