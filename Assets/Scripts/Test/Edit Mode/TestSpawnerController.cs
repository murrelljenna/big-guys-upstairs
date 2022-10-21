using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using game.assets;
using game.assets.spawners;
using game.assets.utilities.resources;

namespace Tests
{
    #region IPlayerTransaction implementation
    class MockPlayerTransactionSuccess : IPlayerTransaction
    {

        public bool canAfford(ResourceSet resourceSet)
        {
            return true;
        }

        public void giveResources(ResourceSet resourceSet) {}

        public void takeResources(ResourceSet resourceSet) {}
        public ResourceSet resources() { return new ResourceSet(); }
    }

    class MockPlayerTransactionFailure : IPlayerTransaction
    {

        public bool canAfford(ResourceSet resourceSet)
        {
            return false;
        }

        public void giveResources(ResourceSet resourceSet) {}

        public void takeResources(ResourceSet resourceSet) {}
        public ResourceSet resources() { return new ResourceSet(); }
    }

    #endregion


    public class TestSpawnerController : IInstantiator
    {
        SpawnerController spawnerController;

        [SetUp]
        public void SetUp()
        {
            spawnerController = new SpawnerController();

            spawnerController.setInstantiator(this);
        }

        #region IInstantiator implementation
        public GameObject Instantiate(GameObject gameObject, Vector3 location, Quaternion rotation)
        {
            GameObject testObject = new GameObject(gameObject.name);

            testObject.transform.position = location;
            testObject.transform.rotation = rotation;

            return testObject;
        }
        public GameObject InstantiateAsMine(GameObject gameObject, Vector3 location, Quaternion rotation)
        {
            GameObject testObject = new GameObject(gameObject.name);

            testObject.transform.position = location;
            testObject.transform.rotation = rotation;

            return testObject;
        }

        public GameObject InstantiateAsPlayer(GameObject gameObject, Vector3 location, Quaternion rotation, game.assets.player.Player player)
        {
            GameObject testObject = new GameObject(gameObject.name);

            testObject.transform.position = location;
            testObject.transform.rotation = rotation;

            return testObject;
        }
        #endregion

        [Test]
        public void TestSpawnerControllerSpawnSuccess()
        {
            spawnerController.setTransactor(new MockPlayerTransactionSuccess());

            GameObject prefabToCreate = new GameObject("TestGameObject");
            ResourceSet resourceSet = new ResourceSet();
            Vector3 location = new Vector3(0, 0, 0);
            Quaternion rotation = Quaternion.identity;

            GameObject createdGameObject = spawnerController.Spawn(
                prefabToCreate,
                resourceSet,
                location,
                rotation
            );

            Assert.NotNull(createdGameObject);
        }

        [Test]
        public void TestSpawnerControllerSpawnFailure()
        {
            spawnerController.setTransactor(new MockPlayerTransactionFailure());

            GameObject prefabToCreate = new GameObject("TestGameObject");
            ResourceSet resourceSet = new ResourceSet();
            Vector3 location = new Vector3(0, 0, 0);
            Quaternion rotation = Quaternion.identity;

            GameObject createdGameObject = spawnerController.Spawn(
                prefabToCreate,
                resourceSet,
                location,
                rotation
            );

            Assert.Null(createdGameObject);
        }

        [Test]
        public void TestSpawnerControllerSpawnRotation()
        {
            spawnerController.setTransactor(new MockPlayerTransactionSuccess());

            GameObject prefabToCreate = new GameObject("TestGameObject");
            ResourceSet resourceSet = new ResourceSet();
            Vector3 location = new Vector3(0, 0, 0);
            Quaternion rotation = Quaternion.identity;

            GameObject createdGameObject = spawnerController.Spawn(
                prefabToCreate,
                resourceSet,
                location,
                rotation
            );

            Assert.AreEqual(
                createdGameObject.transform.rotation,
                rotation
            );
        }
    }
}
