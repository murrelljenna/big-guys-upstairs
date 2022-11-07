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
    public class Player : NetworkBehaviour
    {
        public PlayerColour colour { get; set; }

        [Networked(OnChanged = nameof(GetRealColour))]
        public int playerColourIndex { get; set; } = -1;
        [Networked]
        public int popCount { get; set; }
        [Networked]
        public int maxCount { get; set; }
        [Networked]
        public PlayerRef networkPlayer { get; set; }
        [SerializeField]
        public string playerName;
        public ResourceSet resources = new ResourceSet();
        public Vector3 spawnPoint;

        public static Player AsDevCube()
        {
            GameObject devCube = new GameObject();
            Player player = (Player)devCube.AddComponent(typeof(Player));
            player.colour = PlayerColours.Blue;
            player.popCount = 0;
            player.maxCount = 20;

            return player;
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                ResourceUIController.Get().player = this;
            }
        }

        public static void GetRealColour(Changed<Player> changed)
        {
            Debug.Log("AC - WHAT WHY WONT YOUI PRINT");
            changed.Behaviour.colour = PlayerColourManager.ColourAtIndex(changed.Behaviour.playerColourIndex);
            Debug.Log("AC  Colour name : " + PlayerColourManager.ColourAtIndex(changed.Behaviour.playerColourIndex).name);
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
