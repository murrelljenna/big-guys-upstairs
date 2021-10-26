using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Events;

using game.assets.ai;

namespace Tests
{
    public class TestHealth
    {
        private Health health;
        private bool listenerInvoked = false;

        public void listener() {
            listenerInvoked = true;
        }

        public void listener(float hp, float maxhp)
        {
            listenerInvoked = true;
        }

        public void listener(Health health)
        {
            listenerInvoked = true;
        }

        [SetUp]
        public void SetUp()
        {
            GameObject gameObject = new GameObject("TestObject");
            health = gameObject.AddComponent(typeof(Health)) as Health;

            health.onLowerHP = new UnityEvent<float, float>();
            health.onZeroHP = new UnityEvent<Health>();
            health.onRaiseHP = new UnityEvent<float, float>();
            health.onMaxHP = new UnityEvent();
            health.onUnderHalfHP = new UnityEvent();

            health.HP = 5;
            health.maxHP = 20;

            listenerInvoked = false;
        }

        [Test]
        public void TestHealthLowerHP()
        {
            health.HP = 10;
            health.lowerHP(5);

            Assert.AreEqual(5, health.HP);
        }

        [Test]
        public void TestHealthRaiseHP()
        {
            health.maxHP = 20;
            health.HP = 10;
            health.raiseHP(5);

            Assert.AreEqual(15, health.HP);
        }

        [Test]
        public void TestHealthMaxRoundDown()
        {
            health.maxHP = 10;
            health.HP = 5;
            health.raiseHP(10);

            Assert.AreEqual(10, health.HP);
        }
        [Test]
        public void TestHealthMinRoundUp()
        {
            health.HP = 5;
            health.lowerHP(10); 

            Assert.AreEqual(0, health.HP);
        }

        [Test]
        public void TestHealthMaxed()
        {
            health.maxHP = 10;
            health.HP = 10;

            Assert.True(health.maxed());
        }

        [Test]
        public void TestHealthZero()
        {
            health.HP = 0;

            Assert.True(health.zero());
        }

        [Test]
        public void TestHealthMoreThanHalf()
        {
            health.HP = 2;
            health.maxHP = 3;

            Assert.False(health.underHalf());
        }

        [Test]
        public void TestHealthHalf()
        {
            health.HP = 1;
            health.maxHP = 2;

            Assert.True(health.underHalf());
        }

        [Test]
        public void TestHealthLessThanHalf()
        {
            health.HP = 1;
            health.maxHP = 2;

            Assert.True(health.underHalf());
        }

        [UnityTest]
        public IEnumerator TestHealthOnLowerHPInvoke()
        {
            health.onLowerHP.AddListener(listener);
            health.lowerHP(1);
            yield return null;

            Assert.True(listenerInvoked);
        }

        [UnityTest]
        public IEnumerator TestHealthOnZeroHPInvoke()
        {
            health.onZeroHP.AddListener(listener);

            health.HP = 1;
            health.lowerHP(1);
            yield return null;

            Assert.True(listenerInvoked);
        }

        [UnityTest]
        public IEnumerator TestHealthOnRaiseHPInvoke()
        {
            health.onRaiseHP.AddListener(listener);
            health.raiseHP(1);

            yield return null;

            Assert.True(listenerInvoked);
        }

        [UnityTest]
        public IEnumerator TestHealthOnMaxHPInvoke()
        {
            health.onMaxHP.AddListener(listener);

            health.HP = 1;
            health.maxHP = 2;
            health.raiseHP(1);
            yield return null;

            Assert.True(listenerInvoked);
        }

        [UnityTest]
        public IEnumerator TestHealthHalfInvoked()
        {
            health.onUnderHalfHP.AddListener(listener);

            health.maxHP = 2;
            health.HP = 2;
            health.lowerHP(1);
            yield return null;

            Assert.True(listenerInvoked);
        }

        [UnityTest]
        public IEnumerator TestHealthLessThanHalfInvoked()
        {
            health.onUnderHalfHP.AddListener(listener);

            health.maxHP = 3;
            health.HP = 3;
            health.lowerHP(2);
            Debug.Log(health.maxHP);
            Debug.Log(health.HP);
            yield return null;

            Assert.True(listenerInvoked);
        }

        [UnityTest]
        public IEnumerator TestHealthHalfNotInvoked()
        {
            health.onUnderHalfHP.AddListener(listener);

            health.maxHP = 3;
            health.HP = 3;
            health.lowerHP(1);
            yield return null;

            Assert.False(listenerInvoked);
        }
    }
}
