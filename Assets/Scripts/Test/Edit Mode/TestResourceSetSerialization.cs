using game.assets.utilities.resources;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.Serialization
{
    public class TestResourceSetSerialization
    {
        [Test]
        public void TestResourceSetSerializeNeutrality()
        {
            ResourceSet resources = new ResourceSet(50, 50);

            var serialized = ResourceSet.Serialize(resources);

            var deserializedResources = ResourceSet.Deserialize(serialized);

            Assert.AreEqual(deserializedResources, resources);
        }
    }
}
