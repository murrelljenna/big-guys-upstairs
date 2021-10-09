using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using game.assets.ai;
using game.assets.utilities;

namespace Tests
{
    public class TestUnitAggregation
    {
        private string sceneName = "TestUnitAggregation.unity";
        List<Attack> attackers;
        List<Movement> movers;
        private Health attackee;
        private Vector3 destination;
        private bool attackeeTookDamage = false;

        public void onTookDamageCallback(float hp, float maxHP)
        {
            attackeeTookDamage = true;
        }

        private List<Attack> getAttackers()
        {
            List<Attack> ret = new List<Attack>();
            GameObject[] units = GameObject.FindGameObjectsWithTag("unit");
            for (int i = 0; i < units.Length; i++)
            {
                Attack attack = units[i].GetComponent<Attack>();
                if (attack != null)
                {
                    ret.Add(attack);
                }
            }

            return ret;
        }

        private List<Movement> getMovers()
        {
            List<Movement> ret = new List<Movement>();
            GameObject[] units = GameObject.FindGameObjectsWithTag("unit");
            for (int i = 0; i < units.Length; i++)
            {
                Movement movement = units[i].GetComponent<Movement>();
                if (movement != null)
                {
                    movement.SetAsMine();
                    ret.Add(movement);
                }
            }

            return ret;
        }

        private void createGameObjectWithColliderAt(Vector3 position)
        {

        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            EditorSceneManager.LoadSceneInPlayMode(
                Test.TestUtils.testSceneDirPath + sceneName,
                new LoadSceneParameters(LoadSceneMode.Single)
            );

            yield return null;

            attackers = getAttackers();
            movers = getMovers();
            attackee = GameObject.Find("Attackee").GetComponent<Health>();
            destination = GameObject.Find("Destination").GetComponent<Transform>().position;

            yield return new EnterPlayMode();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return new ExitPlayMode();
        }

        [UnityTest, Order(1)]
        public IEnumerator testMovementAggGoToSendsAllUnits()
        {
            MovementAggregation moversAgg = new MovementAggregation(movers);
            moversAgg.goTo(destination);
            yield return new WaitForSeconds(3);
            Attack[] inRange = GameUtils.findGameObjectsInRange(destination, 3f).GetComponents<Attack>();
            Assert.True(inRange.Length == moversAgg.units.Count);
        }

        [UnityTest, Order(1)]
        public IEnumerator testAttackersAggAttacks()
        {
            AttackAggregation attackersAgg = new AttackAggregation(attackers);
            attackee.onLowerHP.AddListener(onTookDamageCallback);
            attackersAgg.attack(attackee);
            yield return new WaitForSeconds(4);
            Assert.True(attackeeTookDamage);
        }
    }
}
