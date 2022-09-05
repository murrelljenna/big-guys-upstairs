using UnityEngine;

using game.assets;
using game.assets.ai;
using NUnit.Framework;
using game.assets.player;
using game.assets.utilities;

namespace Tests
{
    public class TestOwnershipOps
    {
        private Health healthWithOwnership;
        private LocalGameManager localGameManager;
        private Ownership ownership;
        private GameObject gameObjectWithHealth;

        [SetUp]
        public void SetUp()
        {
            GameObject gameManager = new GameObject(GameUtils.MagicWords.GameObjectNames.GameManager);
            localGameManager = gameManager.AddComponent(typeof(LocalGameManager)) as LocalGameManager;
            localGameManager.players = new game.assets.player.Player[2] { new game.assets.player.Player(), new game.assets.player.Player() };

            gameObjectWithHealth = new GameObject("Healthyboi");
            healthWithOwnership = gameObjectWithHealth.AddComponent(typeof(Health)) as Health;
            ownership = gameObjectWithHealth.AddComponent(typeof(Ownership)) as Ownership;
        }

        [Test]
        public void TestSetAsMineWithMonoBehaviour()
        {
            healthWithOwnership.SetAsMine();
            Assert.True(healthWithOwnership.IsMine());
        }

        [Test]
        public void TestSetAsMineWithGameObject()
        {
            gameObjectWithHealth.SetAsMine();
            Assert.True(gameObjectWithHealth.GetComponent<Ownership>().IsMine());
        }

        [Test]
        public void TestSetAsEnemyWithGameObject()
        {
            gameObjectWithHealth.SetAsPlayer(localGameManager.players[1]);
            Assert.True(gameObjectWithHealth.GetComponent<Ownership>().IsEnemy());
        }


        [Test]
        public void TestSetAsEnemyWithGameObjectAttachesOwnership()
        {
            Object.DestroyImmediate(gameObjectWithHealth.GetComponent<Ownership>());
            gameObjectWithHealth.SetAsPlayer(localGameManager.players[1]);
            Assert.True(gameObjectWithHealth.GetComponent<Ownership>().IsEnemy());
        }
    }
}
