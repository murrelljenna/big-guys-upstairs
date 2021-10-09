using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using game.assets.ai;
using UnityEngine.TestTools;
using game.assets;
using static game.assets.utilities.GameUtils;


namespace Tests
{
    public class TestRangedAttack
    {
        private string sceneName = "TestRangedAttack.unity";

        private Attack attacker;
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

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            EditorSceneManager.LoadSceneInPlayMode(
                Test.TestUtils.testSceneDirPath + sceneName,
                new LoadSceneParameters(LoadSceneMode.Single)
            );

            yield return null;

            attackeeTookDamage = false;

            attacker = GameObject.Find("Attacker").GetComponent<Attack>();
            attackee = GameObject.Find("Attackee").GetComponent<Health>();

            LocalGameManager gameManager = GameObject.Find(MagicWords.GameObjectNames.GameManager).GetComponent<LocalGameManager>();
            gameManager.players = new game.assets.player.Player[2] {
                new game.assets.player.Player(),
                new game.assets.player.Player()
            };

            attackee.gameObject.SetAsPlayer(gameManager.players[1]);
            attacker.SetAsMine();

            yield return new EnterPlayMode();
        }

        [UnityTest, Order(1)]
        public IEnumerator testRangedAttackSpawnsProjectile()
        {
            attacker.attack(attackee);
            yield return null;
            yield return null;
            yield return null;
            Assert.True(GameObject.Find("Projectile(Clone)") != null);
        }

        [UnityTest, Order(2)]
        public IEnumerator testRangedAttackProjectileCausesDamage()
        {
            attacker.attack(attackee);
            attackee.onLowerHP.AddListener(onTookDamageCallback);
            yield return new WaitForSeconds(1);
            Assert.True(attackeeTookDamage);
        }

        [UnityTest, Order(3)]
        public IEnumerator testRangedAttackDoesNotCauseItsOwnDamage()
        {
            attacker.attack(attackee);
            attackee.onLowerHP.AddListener(onTookDamageCallback);
            yield return null;
            yield return null;
            yield return null;
            Assert.False(attackeeTookDamage);
        }

        [UnityTest, Order(4)]
        public IEnumerator testRangedAttackProjectileDisapearsAfterColl()
        {
            attacker.attack(attackee);
            yield return null;
            yield return null;
            yield return null;
            GameObject projectile = GameObject.Find("Projectile(Clone)");
            yield return new WaitForSeconds(1);
            Assert.True(projectile == null);
        }
    }
}
