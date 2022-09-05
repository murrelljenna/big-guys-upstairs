using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using game.assets.utilities.resources;

namespace Tests
{
    public class TestResourceSet
    {
        [Test]
        public void TestResourceSetMinusOperatorPos()
        {
            ResourceSet left = new ResourceSet(
                wood: 20,
                food: 20,
                stone: 10
            );

            ResourceSet right = new ResourceSet(
                wood: 20,
                food: 10,
                stone: 0
            );

            Assert.AreEqual(
                left - right,
                new ResourceSet(
                    wood: 0,
                    food: 10,
                    stone: 10
                )
            );
        }

        [Test]
        public void TestResourceSetMinusOperatorNeg()
        {
            ResourceSet left = new ResourceSet(
                wood: 20,
                food: 20,
                stone: 10
            );

            ResourceSet right = new ResourceSet(
                wood: 20,
                food: 10,
                stone: 20 // Greater than left.stone
            );

            Assert.AreEqual(
                left - right,
                new ResourceSet(
                    wood: 0,
                    food: 10,
                    stone: -10
                )
            );
        }

        [Test]
        public void TestResourceSetPlusOperator()
        {
            ResourceSet left = new ResourceSet(
                wood: 20,
                food: 20,
                stone: 10
            );

            ResourceSet right = new ResourceSet(
                wood: 20,
                food: 10,
                stone: 0
            );

            Assert.AreEqual(
                left + right,
                new ResourceSet(
                    wood: 40,
                    food: 30,
                    stone: 10
                )
            );
        }

        [Test]
        public void TestResourceSetIsEmpty()
        {
            // Initialize with empty constructor.
            ResourceSet resourceSet = new ResourceSet();

            Assert.True(resourceSet.empty());
        }

        [Test]
        public void TestSetResourceSetEmpty()
        {

            ResourceSet resourceSet = new ResourceSet(
                wood: 20,
                food: 10,
                gold: 10,
                stone: 10,
                iron: 10,
                horse: 10
            );

            resourceSet.setEmpty();
            Debug.Log(resourceSet.wood);

            Assert.True(resourceSet.empty());
        }

        [Test]
        public void TestResourceSetAnyValOverNegEdge()
        {
            ResourceSet resourceSet = new ResourceSet(
                wood: 20,
                food: 10,
                gold: 10,
                stone: 10,
                iron: 10,
                horse: 10
            );

            Assert.False(resourceSet.anyValOver(20));
        }

        [Test]
        public void TestResourceSetAnyValOverNeg()
        {
            ResourceSet resourceSet = new ResourceSet(
                wood: 20,
                food: 10,
                stone: 10
            );

            Assert.False(resourceSet.anyValOver(25));
        }

        [Test]
        public void TestResourceSetAnyValOverPos()
        {
            ResourceSet resourceSet = new ResourceSet(
                wood: 20,
                food: 10,
                stone: 10
            );

            Assert.True(resourceSet.anyValOver(15));
        }

        [Test]
        public void TestResourceSetGreaterThanOrEqTrue()
        {
            ResourceSet left = new ResourceSet(
                wood: 25,
                food: 10,
                stone: 15
            );

            ResourceSet right = new ResourceSet(
                wood: 20,
                food: 10,
                stone: 10
            );

            Assert.True(left >= right);
        }

        [Test]
        public void TestResourceSetGreaterThanOrEqFalse()
        {
            ResourceSet left = new ResourceSet(
                wood: 25,
                food: 10,
                stone: 15
            );

            ResourceSet right = new ResourceSet(
                wood: 20,
                food: 10,
                stone: 10
            );

            Assert.False(right >= left);
        }

        [Test]
        public void TestResourceSetLessThanOrEqEdge()
        {
            ResourceSet left = new ResourceSet(
                wood: 20,
                food: 10,
                stone: 10
            );

            ResourceSet right = new ResourceSet(
                wood: 20,
                food: 10,
                stone: 10
            );



            Assert.True(left <= right);
        }

        [Test]
        public void TestResourceSetGreaterThanOrEqEdge()
        {
            ResourceSet left = new ResourceSet(
                wood: 20,
                food: 10,
                stone: 10
            );

            ResourceSet right = new ResourceSet(
                wood: 20,
                food: 10,
                stone: 10
            );

            Assert.True(left >= right);
        }

    }
}
