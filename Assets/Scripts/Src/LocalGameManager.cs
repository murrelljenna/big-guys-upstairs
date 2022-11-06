using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static game.assets.utilities.GameUtils;
using UnityEngine.SceneManagement;
using game.assets.player;
using System;
using game.assets.ai;

namespace game.assets
{
    public class LocalGameManager : GameManager
    {
        [Tooltip("Prefab used for player")]
        public GameObject playerPrefab;

        public override Scene Initialize(string mapName, Vector3[] spawnPoints)
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
            for (int i = 0; i < players.Length; i++)
            {
                PlayerColour colour = pickFirstAvailableColour();
                //players[i] = Player.AsDevCube();
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

            var capital = getLocalPlayer().getCities()[0];
            var health = capital.GetComponent<Health>();
            health.onZeroHP.AddListener(localPlayerLoses);
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
    }
}
