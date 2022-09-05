using game.assets;
using game.assets.ai;
using UnityEngine.Events;
using NUnit.Framework;
using UnityEngine;
using game.assets.player;
using game.assets.utilities;
using static Test.TestUtils;

namespace Tests
{
    public class TestOwnership
    {
        private Health healthWithOwnership;
        private LocalGameManager localGameManager;
        private Ownership ownership;

        private bool callBackCalled;
        private void callback(game.assets.player.Player player)
        {
            callBackCalled = true;
        }

        [SetUp]
        public void SetUp()
        {
            ClearGameObjects();
            GameObject gameManager = new GameObject(GameUtils.MagicWords.GameObjectNames.GameManager);
            localGameManager = gameManager.AddComponent(typeof(LocalGameManager)) as LocalGameManager;
            localGameManager.players = new game.assets.player.Player[1] { new game.assets.player.Player() };

            GameObject gameObjectWithHealth = new GameObject("Healthyboi");
            healthWithOwnership = gameObjectWithHealth.AddComponent(typeof(Health)) as Health;
            ownership = gameObjectWithHealth.AddComponent(typeof(Ownership)) as Ownership;
            ownership.onNewOwner = new UnityEvent<game.assets.player.Player>();

            callBackCalled = false;
        }

        [Test]
        public void TestOwnershipIsMine()
        {
            ownership.setOwner(LocalPlayer.get());
            Assert.True(healthWithOwnership.IsMine());
        }

       [Test]
        public void TestSetOwnerInvokesEvent()
        {
            ownership.onNewOwner.AddListener(callback);
            ownership.setOwner(LocalPlayer.get());
            Assert.True(callBackCalled);
        }


        [Test]
        public void TestOwnershipClearOwner()
        {
            ownership.setOwner(LocalPlayer.get());
            ownership.clearOwner();
            Assert.False(healthWithOwnership.IsMine());
        }

        [Test]
        public void TestOwnershipNotMineByDefault()
        {
            Assert.False(healthWithOwnership.IsMine());
        }
    }
}
