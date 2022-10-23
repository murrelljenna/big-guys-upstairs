using System.Collections;
using UnityEngine;
using static game.assets.utilities.GameUtils;
using UnityEngine.SceneManagement;
using game.assets.player;
using System;
using game.assets.ai;

namespace game.assets
{
    public abstract class GameManager : MonoBehaviour
    {
        public const string version = "0.1.1-alpha";

        public enum GameMode
        {
            Wave
        }

        [Tooltip("Fireworks prefab to use when player wins")]
        public GameObject fireworksPrefab;
        private struct ColourAvailability
        {
            public PlayerColour colour;
            public bool available;

            public ColourAvailability(PlayerColour colour, bool available = false)
            {
                this.colour = colour;
                this.available = true;
            }
        }

        private ColourAvailability[] availableColours = new ColourAvailability[]{
            new ColourAvailability(PlayerColours.Blue),
            new ColourAvailability(PlayerColours.Red),
            new ColourAvailability(PlayerColours.Green),
            new ColourAvailability(PlayerColours.Pink),
            new ColourAvailability(PlayerColours.White),
            new ColourAvailability(PlayerColours.Yellow),
            new ColourAvailability(PlayerColours.Black)
        };

        [Tooltip("Prefab used for player")]
        public GameObject playerPrefab;

        [Tooltip("Prefab used for city")]
        public GameObject cityPrefab;

        [Tooltip("Prefab used for ClientSingleton")]
        public GameObject clientSingleton;

        public GameMode gameMode;

        protected Vector3[] spawnPoints;

        public BarbarianPlayer barbarianPlayer = new BarbarianPlayer();

        // Initialize always with an empty player - this makes testing easier, as the ownership will work.
        public player.Player[] players = new player.Player[1] { new player.Player() };

        public abstract Scene Initialize(string mapName, Vector3[] spawnPoints);

        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public player.Player getLocalPlayer()
        {
            return players[0];
        }

        protected PlayerColour pickFirstAvailableColour()
        {
            for (int i = 0; i < availableColours.Length; i++)
            {
                if (availableColours[i].available)
                {
                    return availableColours[i].colour;
                }
            }

            throw new ArgumentException("No available colours found", nameof(availableColours));
        }

        public static LocalGameManager Get()
        {
            return GameObject.Find(MagicWords.GameObjectNames.GameManager)?.GetComponent<LocalGameManager>();
        }

        protected void localPlayerWins()
        {
            playerWins(players[0]);
        }

        protected void localPlayerLoses(Health health)
        {
            playerLoses(getLocalPlayer());
        }

        private void playerLoses(Player player)
        {
            StartCoroutine(killAllUnits(player));
        }

        private IEnumerator killAllUnits(Player player)
        {
            var thingsToDie = player.getAllAttackables().ToArray();

            for (int i = 0; i < thingsToDie.Length; i++)
            {
                thingsToDie[i].kill();
                yield return null;
            }
        }

        protected void playerWins(Player player)
        {
            var cities = player.getCities();
            for (int i = 0; i < cities.Length; i++)
            {
                var city = cities[i];
                PlayAudio.PlayRandomSourceOnGameobject(city.transform?.Find("Audio")?.Find("CheerSounds").gameObject);
                fireworksAtCity(city);
            }
        }

        private void fireworksAtCity(GameObject go)
        {
            Instantiate(fireworksPrefab, go.transform);
        }
    }
}
