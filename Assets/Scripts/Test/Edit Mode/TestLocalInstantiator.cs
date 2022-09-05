using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using game.assets;
using game.assets.utilities;
using game.assets.player;

namespace Tests
{
    public class TestLocalInstantiator
    {
        LocalGameManager localGameManager;
        [SetUp]
        public void SetUp() {
            GameObject gameManager = new GameObject(GameUtils.MagicWords.GameObjectNames.GameManager);
            localGameManager = gameManager.AddComponent(typeof(LocalGameManager)) as LocalGameManager;
            localGameManager.players = new game.assets.player.Player[2] { new game.assets.player.Player(), new game.assets.player.Player() };
        }

        [Test]
        public void TestLocalInstantiatorInstantiateNotNull()
        {
            GameObject prefab = new GameObject("Tester");
            Vector3 location = new Vector3(10, 10, 10);
            Quaternion rotation = Quaternion.identity;

            IInstantiator instantiator = new LocalInstantiator();
            GameObject gameObject = instantiator.Instantiate(prefab, location, rotation);

            Assert.NotNull(gameObject);
        }

        [Test]
        public void TestLocalInstantiatorInstantiatePosition()
        {
            GameObject prefab = new GameObject("Tester");
            Vector3 location = new Vector3(10, 10, 10);
            Quaternion rotation = Quaternion.identity;

            IInstantiator instantiator = new LocalInstantiator();

            GameObject gameObject = instantiator.Instantiate(prefab, location, rotation);

            Assert.AreEqual(gameObject.transform.position, location);
        }

        [Test]
        public void TestLocalInstantiatorInstantiateRotation()
        {
            GameObject prefab = new GameObject("Tester");
            Vector3 location = new Vector3(10, 10, 10);
            Quaternion rotation = Quaternion.identity;

            IInstantiator instantiator = new LocalInstantiator();

            GameObject gameObject = instantiator.Instantiate(prefab, location, rotation);

            Assert.AreEqual(gameObject.transform.rotation, rotation);
        }

        [Test]
        public void TestInstantiateAsMine()
        {
            GameObject prefab = new GameObject("Tester");
            Vector3 location = new Vector3(10, 10, 10);
            Quaternion rotation = Quaternion.identity;

            IInstantiator instantiator = new LocalInstantiator();

            GameObject gameObject = instantiator.InstantiateAsMine(prefab, location, rotation);

            Assert.True(gameObject.IsMine());
        }

        [Test]
        public void TestInstantiateAsEnemy()
        {
            GameObject prefab = new GameObject("Tester");
            Vector3 location = new Vector3(10, 10, 10);
            Quaternion rotation = Quaternion.identity;

            IInstantiator instantiator = new LocalInstantiator();

            GameObject gameObject = instantiator.InstantiateAsPlayer(prefab, location, rotation, localGameManager.players[1]);

            Assert.True(gameObject.GetComponent<Ownership>().IsEnemy());
        }
    }
}
