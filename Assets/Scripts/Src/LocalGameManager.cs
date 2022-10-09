using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static game.assets.utilities.GameUtils;
using UnityEngine.SceneManagement;
using game.assets.player;
using System;

public enum GameMode
{
    Wave
}

namespace game.assets
{
    public class LocalGameManager : MonoBehaviour
    {
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

        private Vector3[] spawnPoints;

        public BarbarianPlayer barbarianPlayer = new BarbarianPlayer();

        // Initialize always with an empty player - this makes testing easier, as the ownership will work.
        public player.Player[] players = new player.Player[1] { new player.Player() };

        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public Scene Initialize(string mapName, Vector3[] spawnPoints)
        {
            if (gameMode == GameMode.Wave)
            {
                var wavePlayer = BarbarianWavePlayer.Get();
                wavePlayer.lastBarbarianWaveDefeated.AddListener(localPlayerWins); // Note, if we have multiplayer wave games we cannot do this
                barbarianPlayer = wavePlayer;
            }
            this.spawnPoints = spawnPoints;
            SceneManager.sceneLoaded += onSceneLoaded;
            Scene scene = SceneManager.LoadScene(mapName, new LoadSceneParameters(LoadSceneMode.Single));
            players = new player.Player[spawnPoints.Length];
            for (int i = 0; i < players.Length; i++) {
                PlayerColour colour = pickFirstAvailableColour();
                players[i] = new player.Player();
                players[i].colour = pickFirstAvailableColour(); 
            }

            return scene;
        }

        private void onSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            instantiateClientSingleton();
            instantiateLocalPlayerStart();
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                // Spawn some ai shit or something
            }

            barbarianPlayer.Awake();
        }

        private void instantiateLocalPlayerStart()
        {
            IInstantiator instantiator = InstantiatorFactory.getInstantiator();
            GameObject localCityObject = instantiator.InstantiateAsMine(cityPrefab, fixHeight(spawnPoints[0]), Quaternion.identity);
            localCityObject.name = MagicWords.GameObjectNames.StartingCity;

            Vector3 playerSpawn = randomPointOnUnitCircle(spawnPoints[0], MagicNumbers.PlayerSpawnRadius);
            GameObject localPlayerObject = instantiator.Instantiate(playerPrefab, playerSpawn, Quaternion.identity);
            localPlayerObject.name = MagicWords.GameObjectNames.Player;
        }

        private void instantiateClientSingleton()
        {
            IInstantiator instantiator = InstantiatorFactory.getInstantiator();
            GameObject clientSingletonObj = instantiator.Instantiate(clientSingleton, spawnPoints[0], Quaternion.identity);
            clientSingletonObj.name = MagicWords.GameObjectNames.ClientSingleton;
        }

        public player.Player getLocalPlayer()
        {
            return players[0];
        }

        private PlayerColour pickFirstAvailableColour()
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

        private void localPlayerWins() {
            playerWins(players[0]);
        }

        private void playerWins(Player player) {
            var cities = player.getCities();
            for (int i = 0; i < cities.Length; i++)
            {
                var city = cities[i];

                fireworksAtCity(city);
            }
        }

        private void fireworksAtCity(GameObject go)
        {
            Instantiate(fireworksPrefab, go.transform);
        }
    }
}
