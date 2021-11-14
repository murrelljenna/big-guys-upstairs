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
    public class TestTransactionalMethod : MonoBehaviour
    {
        PlayerDepositor playerDepositor;
        TransactionalMethod transactionalMethod;
        private bool callbackCalled = false;
        void callback()
        {
            callbackCalled = true;
        }

        [SetUp]
        public void SetUp()
        {
            callbackCalled = false;

            FakeClientSingleton();

            playerDepositor = LocalPlayer.getPlayerDepositor();

            GameObject goWithTransactionalMethod = new GameObject("Whatevs");

            transactionalMethod = goWithTransactionalMethod.AddComponent<TransactionalMethod>();
            transactionalMethod.canAfford = new UnityEvent();
            transactionalMethod.cannotAfford = new UnityEvent();
        }

        [UnityTest]
        public IEnumerator TestTransactionalMethodCanAfford()
        {
            ResourceSet resourceSet = new ResourceSet(25);
            transactionalMethod.price = resourceSet;

            playerDepositor.giveResources(new ResourceSet(26));

            transactionalMethod.canAfford.AddListener(callback);
            transactionalMethod.Try();
            yield return null;

            Assert.True(callbackCalled);
        }

        [UnityTest]
        public IEnumerator TestTransactionalMethodCantAfford()
        {
            ResourceSet resourceSet = new ResourceSet(25);
            transactionalMethod.price = resourceSet;

            transactionalMethod.cannotAfford.AddListener(callback);
            transactionalMethod.Try();

            yield return null;

            Assert.False(callbackCalled);
        }
    }
}
