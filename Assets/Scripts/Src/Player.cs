using Fusion;
using game.assets.ai;
using game.assets.utilities.resources;
using System;
using System.Collections.Generic;
using UnityEngine;
using static game.assets.utilities.GameUtils;

namespace game.assets.player
{
    [System.Serializable]
    public class Player
    {
        public PlayerColour colour { get; set; }
        public int popCount { get; set; }
        public int maxCount { get; set; }
        public PlayerRef networkPlayer { get; set; }
        public ResourceSet resources = new ResourceSet();
        public Vector3 spawnPoint;

        public static object Deserialize(byte[] data)
        {
            var player = new Player();

            player.colour = PlayerColourManager.ColourAtIndex(BitConverter.ToInt32(data, 0));

            var resourceSetArr = new Byte[20];
            Array.ConstrainedCopy(data, 4, resourceSetArr, 0, 20);

            player.resources = (ResourceSet)ResourceSet.Deserialize(resourceSetArr);
            return player;
        }

        public static byte[] Serialize(object customType)
        {


            var c = (Player)customType;

            var serializedResources = ResourceSet.Serialize(c.resources);
            var serializedColourIndex = BitConverter.GetBytes(PlayerColourManager.IndexOfColour(c.colour));

            var arrConstructor = new List<Byte>();

            arrConstructor.AddRange(serializedColourIndex);
            arrConstructor.AddRange(serializedResources);
            return arrConstructor.ToArray();
        }

        public Player()
        {
            colour = PlayerColours.Blue;
            popCount = 0;
            maxCount = 20;
        }

        public Player(PlayerColour colour)
        {
            this.colour = colour;
            popCount = 0;
            maxCount = 20;
        }

        public Player withResources(int wood = 100, int food = 100)
        {
            this.resources = new ResourceSet(wood, food);
            return this;
        }

        public bool maxPop()
        {
            return popCount >= maxCount;
        }

        public GameObject[] getCities()
        {
            var myCities = new List<GameObject>();
            var cities = GameObject.FindObjectsOfType<Depositor>();
            for (int i = 0; i < cities.Length; i++)
            {
                var city = cities[i];

                if (city.BelongsTo(this))
                {
                    myCities.Add(city.gameObject);
                }
            }

            return myCities.ToArray();
        }

        public List<Health> getAllAttackables()
        {
            var myAttackables = new List<Health>();
            var attackables = GameObject.FindObjectsOfType<Health>();
            for (int i = 0; i < attackables.Length; i++)
            {
                var health = attackables[i];

                if (health.BelongsTo(this))
                {
                    myAttackables.Add(health);
                }
            }

            return myAttackables;
        }

        public void giveResources(ResourceSet resourceSet)
        {
            resources = resources + resourceSet;
        }
        public void takeResources(ResourceSet resourceSet)
        {
            resources = resources - resourceSet;
        }
        public bool canAfford(ResourceSet resourceSet) { return (resources >= resourceSet); }
    }
}
