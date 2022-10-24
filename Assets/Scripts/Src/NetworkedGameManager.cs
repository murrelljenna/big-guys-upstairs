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
    struct PlayerNetworkInput : INetworkInput
    {
        public const uint BUTTON_USE = 1 << 0;
        public const uint BUTTON_FIRE = 1 << 1;
        public const uint BUTTON_FIRE_ALT = 1 << 2;

        public const uint BUTTON_FORWARD = 1 << 3;
        public const uint BUTTON_BACKWARD = 1 << 4;
        public const uint BUTTON_LEFT = 1 << 5;
        public const uint BUTTON_RIGHT = 1 << 6;

        public const uint BUTTON_JUMP = 1 << 7;
        public const uint BUTTON_CROUCH = 1 << 8;
        public const uint BUTTON_WALK = 1 << 9;

        public const uint BUTTON_ACTION1 = 1 << 10;
        public const uint BUTTON_ACTION2 = 1 << 11;
        public const uint BUTTON_ACTION3 = 1 << 12;
        public const uint BUTTON_ACTION4 = 1 << 14;

        public const uint BUTTON_RELOAD = 1 << 15;

        public float cameraRotationX;
        public float cameraRotationY;

        public uint Buttons;
        public byte Weapon;
        public Angle Yaw;
        public Angle Pitch;
        public NetworkBool ePressed;

        public bool IsUp(uint button)
        {
            return IsDown(button) == false;
        }

        public bool IsDown(uint button)
        {
            return (Buttons & button) == button;
        }
    }

    public class NetworkedGameManager : GameManager, INetworkRunnerCallbacks
    {
        private NetworkRunner _runner;

        [SerializeField] private NetworkPrefabRef _playerPrefab;

        private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
        [SerializeField]
        private Scene targetScene;

        Fusion.GameMode networkMode;

        public override Scene Initialize(string mapName, Vector3[] spawnPoints)
        {
            if (_runner == null)
            {
                targetScene = SceneManager.LoadScene(mapName, new LoadSceneParameters(LoadSceneMode.Single));
                StartGame(targetScene, networkMode);
            }
            this.spawnPoints = spawnPoints;
            return targetScene;
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            // Create a unique position for the player
            players = new player.Player[spawnPoints.Length];

            for (int i = 0; i < players.Length; i++)
            {
                PlayerColour colour = pickFirstAvailableColour();
                players[i] = new player.Player();
                players[i].colour = pickFirstAvailableColour();
            }

            instantiateNetworkedPlayerStart(runner, player);
        }

        private void instantiateNetworkedPlayerStart(NetworkRunner runner, PlayerRef player)
        {
            Vector3 playerSpawn = randomPointOnUnitCircle(spawnPoints[0], MagicNumbers.PlayerSpawnRadius);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, playerSpawn, Quaternion.identity, player);
            // Keep track of the player avatars so we can remove it when they disconnect
            _spawnedCharacters.Add(player, networkPlayerObject);
        }

        async void StartGame(Scene scene, Fusion.GameMode mode)
        {
            // Create the Fusion runner and let it know that we will be providing user input
            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;

            // Start or join (depends on gamemode) a session with a specific name
            await _runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "TestRoom",
                Scene = scene.buildIndex,
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
            Debug.Log("Sending input");
            var frameworkInput = new PlayerNetworkInput();

            if (Input.GetKey(KeyCode.W))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_FORWARD;
            }

            if (Input.GetKey(KeyCode.S))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_BACKWARD;
            }

            if (Input.GetKey(KeyCode.A))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_LEFT;
            }

            if (Input.GetKey(KeyCode.D))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_RIGHT;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_JUMP;
            }

            if (Input.GetKey(KeyCode.C))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_CROUCH;
            }

            if (Input.GetKey(KeyCode.E))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ACTION1;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                frameworkInput.ePressed = true;
            }
            else
            {
                frameworkInput.ePressed = false;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ACTION2;
            }

            if (Input.GetKey(KeyCode.F))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ACTION3;
            }

            if (Input.GetKey(KeyCode.G))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ACTION4;
            }

            if (Input.GetKey(KeyCode.R))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_RELOAD;
            }

            if (Input.GetMouseButton(0))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_FIRE;
            }

            var localView = LocalPlayer.getView();
            if (localView != null)
            {
                frameworkInput.cameraRotationX = localView.getXRotation();
                frameworkInput.cameraRotationY = localView.getYRotation();
            }

            input.Set(frameworkInput);
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

        private void OnGUI()
        {
            if (_runner == null)
            {
                if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
                {
                    networkMode = Fusion.GameMode.Host;
                    Initialize("TwoPlayer", new Vector3[] {
                        new Vector3(8.79f, 1f, 11f)
                    });
                }
                if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
                {
                    networkMode = Fusion.GameMode.Client;
                    Initialize("TwoPlayer", new Vector3[] {
                        new Vector3(8.79f, 1f, 11f)
                    });
                }
            }
        }
    }
}
