using game.assets;
using game.assets.player;
using game.assets.utilities.resources;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TestTools;
using static Test.TestUtils;

namespace Tests
{
    public class TestTransactionalMethod
    {
        Ownership ownership;
        TransactionalMethod transactionalMethod;

        [SetUp]
        public void SetUp()
        {
            ClearGameObjects();

            ownership.setOwner(Player.AsDevCube());

            GameObject goWithTransactionalMethod = new GameObject("Whatevs");

            transactionalMethod = goWithTransactionalMethod.AddComponent<TransactionalMethod>();
        }

        [Test]
        public void TestTransactionalMethodCanAfford()
        {
            ResourceSet resourceSet = new ResourceSet(25);
            transactionalMethod.price = resourceSet;
            transactionalMethod.ownership = ownership;

            ownership.owner.giveResources(new ResourceSet(26));

            var result = transactionalMethod.Try();

            Assert.True(result);
        }

        [Test]
        public void TestTransactionalMethodCantAfford()
        {
            ResourceSet resourceSet = new ResourceSet(25);
            transactionalMethod.price = resourceSet;
            transactionalMethod.ownership = ownership;

            var result = transactionalMethod.Try();

            Assert.False(result);
        }
    }
}
