using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using game.assets;

namespace Tests
{
    public class TestInstantiatorFactory
    {
        [Test]
        public void TestInstantiatorFactoryNetworked()
        {
            bool networked = true;
            IInstantiator instantiator = InstantiatorFactory.getInstantiator(networked);

            Assert.AreEqual(typeof(NetworkedInstantiator), instantiator.GetType());
        }

        [Test]
        public void TestInstantiatorFactoryLocal()
        {
            bool networked = false;
            IInstantiator instantiator = InstantiatorFactory.getInstantiator(networked);

            Assert.AreEqual(typeof(LocalInstantiator), instantiator.GetType());
        }
    }
}
