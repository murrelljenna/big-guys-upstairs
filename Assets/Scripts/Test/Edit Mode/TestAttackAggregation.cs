using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using game.assets.ai;
using UnityEngine.Events;

namespace Tests
{
    public class TestAttackAggregation
    {
        const int MAGIC_NUMBER = 10;
        private AttackAggregation attackAggregation;
        private GameObject[] gameObjects;
        private Attack[] attacks;
        private Movement[] movements;

        private bool callBackCalledCorrectly = false;



        [SetUp]
        public void SetUp()
        {
            gameObjects = new GameObject[MAGIC_NUMBER];
            attacks = new Attack[MAGIC_NUMBER];
            movements = new Movement[MAGIC_NUMBER / 2];
            for (int i = 0; i < gameObjects.Length / 2; i++)
            {
                gameObjects[i] = new GameObject("DOESNT MATTER");
                Health ignore = gameObjects[i].AddComponent(typeof(Health)) as Health;
                ignore.onZeroHP = new UnityEvent<Health>();
                attacks[i] = gameObjects[i].AddComponent(typeof(Attack)) as Attack;
                attacks[i].onSelect = new UnityEvent();
            }

            for (int i = gameObjects.Length / 2; i < gameObjects.Length; i++)
            {
                gameObjects[i] = new GameObject("DONT MATTER");
                attacks[i] = gameObjects[i].AddComponent(typeof(Attack)) as Attack;
                attacks[i].onSelect = new UnityEvent();
                movements[i - (gameObjects.Length / 2)] = gameObjects[i].AddComponent(typeof(Movement)) as Movement;
            }
            attackAggregation = new AttackAggregation();
            GameObject gameObject = new GameObject("TestObject");
            callBackCalledCorrectly = false;
        }

        [Test]
        public void TestAddUnit() {
            attackAggregation.add(attacks[1]);
            Assert.True(attackAggregation.units.Contains(attacks[1]));
        }

        [Test]
        public void TestAddUnitDoesNotDuplicate()
        {
            attackAggregation.add(attacks[1]);
            attackAggregation.add(attacks[1]);
            Assert.True(attackAggregation.units.Count == 1);
        }

        [Test]
        public void TestCanGrabAllUnitsThatMove()
        {
            attackAggregation = new AttackAggregation(new List<Attack>(attacks));
            MovementAggregation movementAggregation = attackAggregation.unitsThatCanMove();
            for (int i = 0; i < movements.Length; i++)
            {
                Assert.True(movementAggregation.units.Contains(movements[i]));
            }
        }

        [Test]
        public void TestRemovesDeadUnits()
        {
            Attack attack = attacks[0];
            Health health = attack.GetComponent<Health>();

            attackAggregation = new AttackAggregation(new List<Attack>(attacks));
            health.onZeroHP.Invoke(health);

            Assert.False(attackAggregation.contains(attack));

        }

        [Test]
        public void TestCommunicatesDeadUnitsUpwards()
        {
            Attack attack = attacks[0];
            Health health = attack.GetComponent<Health>();

            attackAggregation = new AttackAggregation(new List<Attack>(attacks));
            attackAggregation.unitDead.AddListener(callback);
            health.onZeroHP.Invoke(health);

            void callback(Attack deadUnit)
            {
                if (deadUnit == attack)
                {
                    callBackCalledCorrectly = true;
                }
            }

            Assert.True(callBackCalledCorrectly);

        }

        [Test]
        public void TestClear()
        {
            attackAggregation = new AttackAggregation(new List<Attack>(attacks));
            attackAggregation.clear();
            Assert.True(attackAggregation.units.Count == 0);
        }
    }
}
