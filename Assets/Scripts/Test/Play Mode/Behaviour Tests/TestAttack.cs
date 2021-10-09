using System.Collections;

using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using game.assets.ai;
using static game.assets.utilities.GameUtils;
using game.assets;

namespace Tests
{
    public class TestAttack
    {
        private string sceneName = "TestAttack.unity";

        private Attack stationaryAttacker;
        private Attack movingAttacker;
        private Health attackee;

        private bool attackeeTookDamage = false;
        public void onTookDamageCallback(float hp, float maxHp)
        {
            attackeeTookDamage = true;
        }

        public void putAttackerInRangeOfAttackee(Attack attacker)
        {
            attacker.gameObject.transform.position = attackee.gameObject.transform.position;
        }

        public void putAttackerOutOfRangeOfAttackee(Attack attacker)
        {
            Vector3 outOfRangePosition = attackee.gameObject.transform.position;
            outOfRangePosition.x += stationaryAttacker.attackRange + 2;
            attacker.gameObject.transform.position = outOfRangePosition;
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            EditorSceneManager.LoadSceneInPlayMode(
                Test.TestUtils.testSceneDirPath + sceneName,
                new LoadSceneParameters(LoadSceneMode.Single)
            );

            yield return null;

            attackeeTookDamage = false;

            stationaryAttacker = GameObject.Find("StationaryAttacker").GetComponent<Attack>();
            movingAttacker = GameObject.Find("MovingAttacker").GetComponent<Attack>();
            attackee = GameObject.Find("Attackee").GetComponent<Health>();

            LocalGameManager gameManager = GameObject.Find(MagicWords.GameObjectNames.GameManager).GetComponent<LocalGameManager>();
            gameManager.players = new game.assets.player.Player[2] { 
                new game.assets.player.Player(),
                new game.assets.player.Player() 
            };

            attackee.gameObject.SetAsPlayer(gameManager.players[1]);
            stationaryAttacker.SetAsMine();
            movingAttacker.SetAsMine();

            yield return new EnterPlayMode();
        }

        [UnityTest, Order(1)]
        public IEnumerator testStationaryInRangeAttack()
        {
            attackee.onLowerHP.AddListener(onTookDamageCallback);
            putAttackerInRangeOfAttackee(stationaryAttacker);
            stationaryAttacker.attack(attackee);
            yield return new WaitForSeconds(2);
            Assert.True(attackeeTookDamage);
        }

        [UnityTest, Order(1)]
        public IEnumerator testStationaryInRangeAttacksAutomatically()
        {
            attackee.onLowerHP.AddListener(onTookDamageCallback);
            putAttackerInRangeOfAttackee(stationaryAttacker);
            stationaryAttacker.attack(attackee);
            yield return new WaitForSeconds(2);
            Assert.True(attackeeTookDamage);
        }


        [UnityTest, Order(2)]
        public IEnumerator testStationaryOutOfRangeAttack()
        {
            attackee.onLowerHP.AddListener(onTookDamageCallback);
            putAttackerOutOfRangeOfAttackee(stationaryAttacker);
            stationaryAttacker.attack(attackee);
            yield return new WaitForSeconds(2);
            yield return null;
            Assert.False(attackeeTookDamage);
        }

        [UnityTest, Order(3)]
        public IEnumerator testAttackRate()
        {
            attackee.onLowerHP.AddListener(onTookDamageCallback);
            putAttackerInRangeOfAttackee(stationaryAttacker);
            stationaryAttacker.attack(attackee);
            Assert.False(attackeeTookDamage);
            yield return new WaitForSeconds(stationaryAttacker.attackRate + 1);
            Assert.True(attackeeTookDamage);
            attackeeTookDamage = false;
            yield return new WaitForSeconds(stationaryAttacker.attackRate + 1);
            Assert.True(attackeeTookDamage);
        }

        [UnityTest, Order(4)]
        public IEnumerator testCancelOrdersStopsAttack()
        {
            attackee.onLowerHP.AddListener(onTookDamageCallback);
            putAttackerInRangeOfAttackee(stationaryAttacker);
            stationaryAttacker.attack(attackee);
            yield return new WaitForSeconds(stationaryAttacker.attackRate + 1);
            Assert.True(attackeeTookDamage);
            attackeeTookDamage = false;
            stationaryAttacker.cancelOrders();
            yield return new WaitForSeconds(stationaryAttacker.attackRate + 1);
            Assert.False(attackeeTookDamage);
        }

        [UnityTest, Order(5)]
        public IEnumerator testStationaryWaitsTillInRange()
        {
            attackee.onLowerHP.AddListener(onTookDamageCallback);
            putAttackerOutOfRangeOfAttackee(stationaryAttacker);
            stationaryAttacker.attack(attackee);
            yield return new WaitForSeconds(2);
            Assert.False(attackeeTookDamage);
            putAttackerInRangeOfAttackee(stationaryAttacker);
            yield return new WaitForSeconds(2);
            Assert.True(attackeeTookDamage);
        }

        [UnityTest, Order(6)]
        public IEnumerator testMovingAttackerMovesInRange()
        {
            attackee.onLowerHP.AddListener(onTookDamageCallback);
            putAttackerOutOfRangeOfAttackee(movingAttacker);
            movingAttacker.attack(attackee);
            yield return new WaitForSeconds(2);
            Assert.True(attackeeTookDamage);
        }

        [UnityTest, Order(7)]
        public IEnumerator testUnitAutoAttacksInRange()
        {
            attackee.onLowerHP.AddListener(onTookDamageCallback);
            putAttackerInRangeOfAttackee(movingAttacker);
            yield return new WaitForSeconds(3);
            Assert.True(attackeeTookDamage);
        }

        [UnityTest, Order(8)]
        public IEnumerator testStopsWhenAttackeeHPIsZero()
        {
            attackee.onLowerHP.AddListener(onTookDamageCallback);
            putAttackerInRangeOfAttackee(movingAttacker);
            yield return new WaitForSeconds(3);
            Assert.True(attackeeTookDamage);
            attackee.HP = 0;
            attackeeTookDamage = false;
            yield return new WaitForSeconds(1);
            Assert.False(attackeeTookDamage);
        }
        
        [UnityTest, Order(9)]
        public IEnumerator testStopsWhenAttackeeNoLongerExists()
        {
            attackee.onLowerHP.AddListener(onTookDamageCallback);
            putAttackerInRangeOfAttackee(movingAttacker);
            yield return new WaitForSeconds(3);
            Assert.True(attackeeTookDamage);
            Object.Destroy(attackee.gameObject);
            attackeeTookDamage = false;
            yield return new WaitForSeconds(1);
            Assert.False(attackeeTookDamage);
        }
    }
}
