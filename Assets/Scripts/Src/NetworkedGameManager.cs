using System.Collections;
using UnityEngine;
using static game.assets.utilities.GameUtils;
using UnityEngine.SceneManagement;
using game.assets.player;
using System;
using game.assets.ai;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;

namespace game.assets
{
    public class NetworkedGameManager : GameManager, INetworkRunnerCallbacks
    {
        private NetworkRunner _runner;

        [SerializeField] private NetworkPrefabRef _playerPrefab;

        private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();


        public override Scene Initialize(string mapName, Vector3[] spawnPoints)
        {
            if (_runner == null)
            {
                StartGame(Fusion.GameMode.Host);
            }

            return SceneManager.GetActiveScene();
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3(0, 0, 0);
            //runner.Spawn(_ballPrefab, new Vector3(0, 0, 0), Quaternion.identity, player);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars so we can remove it when they disconnect
            _spawnedCharacters.Add(player, networkPlayerObject);
        }

        async void StartGame(Fusion.GameMode mode)
        {
            // Create the Fusion runner and let it know that we will be providing user input
            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;

            // Start or join (depends on gamemode) a session with a specific name
            await _runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "TestRoom",
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            // Find and remove the players avatar
            if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
            {
                runner.Despawn(networkObject);
                _spawnedCharacters.Remove(player);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {

        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner)
        {

        }
        public void OnSceneLoadStart(NetworkRunner runner) { }

    }
}
