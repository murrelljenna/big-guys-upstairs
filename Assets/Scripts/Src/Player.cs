using Fusion;
using game.assets.ai;
using game.assets.utilities.resources;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static game.assets.utilities.GameUtils;

namespace game.assets.player
{
    [System.Serializable]
    public class Player : NetworkBehaviour
    {
        public PlayerColour colour { get; set; }

        [Networked(OnChanged = nameof(GetRealColour))]
        public int playerColourIndex { get; set; } = -1;
        [Networked(OnChanged = nameof(updateUIOnChange))]
        public int popCount { get; set; } = 0;
        [Networked(OnChanged = nameof(updateUIOnChange))]
        public int maxCount { get; set; } = 10;
        [Networked]
        public PlayerRef networkPlayer { get; set; }
        [SerializeField]
        public string playerName;
        [Networked]
        public ResourceSet resources { get; set; }
        public Vector3 spawnPoint;

        public static Player AsDevCube()
        {
            GameObject devCube = new GameObject();
            Player player = (Player)devCube.AddComponent(typeof(Player));
            player.colour = PlayerColours.Blue;

            return player;
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                ResourceUIController.Get().player = this;
            }
            else
            {
                tryGetColour();
            }
        }

        public void tryGetColour()
        {
            if (playerColourIndex >= 0)
            {
                colour = PlayerColourManager.ColourAtIndex(playerColourIndex);
            }
        }

        public static void GetRealColour(Changed<Player> changed)
        {
            changed.Behaviour.tryGetColour();
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

        private void updateMaxUI()
        {
            if (!Object.HasInputAuthority)
            {
                return;
            }
            var maxText = GameObject.Find(MagicWords.GameObjectNames.MaxPopUI).GetComponent<Text>();
            if (maxText != null)
            {
                maxText.text = maxCount.ToString();
            }
        }

        private void updatePopUI()
        {
            if (!Object.HasInputAuthority)
            {
                return;
            }
            var popText = GameObject.Find(MagicWords.GameObjectNames.CurrentPopUI).GetComponent<Text>();
            if (popText != null)
            {
                popText.text = popCount.ToString();
            }
        }

        public static void updateUIOnChange(Changed<Player> player)
        {
            player.Behaviour.updateMaxUI();
            player.Behaviour.updatePopUI();
        }
    }
}
