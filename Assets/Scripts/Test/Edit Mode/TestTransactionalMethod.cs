﻿using game.assets;
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
    public class TestTransactionalMethod : MonoBehaviour
    {
        Player player = new Player();
        TransactionalMethod transactionalMethod;

        [SetUp]
        public void SetUp()
        {
            ClearGameObjects();

            GameObject goWithTransactionalMethod = new GameObject("Whatevs");

            transactionalMethod = goWithTransactionalMethod.AddComponent<TransactionalMethod>();
        }

        [Test]
        public void TestTransactionalMethodCanAfford()
        {
            ResourceSet resourceSet = new ResourceSet(25);
            transactionalMethod.price = resourceSet;

            player.giveResources(new ResourceSet(26));

            var result = transactionalMethod.Try();

            Assert.True(result);
        }

        [Test]
        public void TestTransactionalMethodCantAfford()
        {
            ResourceSet resourceSet = new ResourceSet(25);
            transactionalMethod.price = resourceSet;

            var result = transactionalMethod.Try();

            Assert.False(result);
        }
    }
}
