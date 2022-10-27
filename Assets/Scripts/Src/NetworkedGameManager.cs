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
    public struct PlayerNetworkInput : INetworkInput
    {
        public const uint BUTTON_USE = 1 << 0;
        public const uint BUTTON_FIRE = 1 << 1;
        public const uint BUTTON_FIRE_ALT = 1 << 2;

        public const uint BUTTON_FORWARD = 1 << 3;
        public const uint BUTTON_BACKWARD = 1 << 4;
        public const uint BUTTON_LEFT = 1 << 5;
        public const uint BUTTON_RIGHT = 1 << 6;

        public const uint BUTTON_JUMP = 1 << 7;

        public const uint BUTTON_ACTION1 = 1 << 10;
        public const uint BUTTON_ACTION2 = 1 << 11;
        public const uint BUTTON_ACTION3 = 1 << 12;
        public const uint BUTTON_ACTION4 = 1 << 14;

        public const uint BUTTON_ALPHA1 = 1 << 15;
        public const uint BUTTON_ALPHA2 = 1 << 16;
        public const uint BUTTON_ALPHA3 = 1 << 17;
        public const uint BUTTON_ALPHA4 = 1 << 18;
        public const uint BUTTON_ALPHA5 = 1 << 19;
        public const uint BUTTON_ALPHA6 = 1 << 20;
        public const uint BUTTON_ALPHA7 = 1 << 21;
        public const uint BUTTON_ALPHA8 = 1 << 22;
        public const uint BUTTON_ALPHA9 = 1 << 23;
        public const uint BUTTON_ALPHA0 = 1 << 24;

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
        [SerializeField] private NetworkPrefabRef _gameManagerState;

        private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
        [SerializeField]
        private Scene targetScene;
        Fusion.GameMode networkMode;


        private Player localPlayer;

        private bool isHost;

        Vector3[] spawns;

        private NetworkedGameManagerState state;
        public void InitGame(string mapName, Vector3[] spawnPoints)
        {
            if (_runner == null)
            {
                isHost = true;
                var targetScene = SceneManager.LoadScene(mapName, new LoadSceneParameters(LoadSceneMode.Single));
                StartGame(targetScene, Fusion.GameMode.Host);
            }

            this.spawns = spawnPoints;
        }

        public void JoinGame(string sessionName, string mapName, Vector3[] spawnPoints)
        {
            if (_runner == null)
            {
                var targetScene = SceneManager.LoadScene(mapName, new LoadSceneParameters(LoadSceneMode.Single));
                StartGame(targetScene, Fusion.GameMode.Client);
            }
        }

        private void InitNetworkGameState(Vector3[] spawnPoints)
        {
            state = _runner.Spawn(_gameManagerState, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<NetworkedGameManagerState>();
            state.Init(spawnPoints);
        }

        async void StartGame(Scene scene, Fusion.GameMode mode)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;

            await _runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "TestRoom",
                Scene = scene.buildIndex,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
        }



        public override Scene Initialize(string mapName, Vector3[] spawnPoints)
        {
            if (_runner == null)
            {
                targetScene = SceneManager.LoadScene(mapName, new LoadSceneParameters(LoadSceneMode.Single));
                StartGame(targetScene,  networkMode);
            }
            this.spawns = spawnPoints;
            players = new player.Player[spawnPoints.Length];
            Debug.Log("AA - Initializing. Player length: " + players.Length);
            return targetScene;
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (isHost)
            {
                Player gamePlayer = state.reserveNewPlayer(player.PlayerId);

                var playerObj = instantiateNetworkedPlayerStart(runner, player);
                playerObj.GetComponent<Ownership>().setOwnerRecursively(gamePlayer);

            }
        }

        private GameObject instantiateNetworkedPlayerStart(NetworkRunner runner, PlayerRef player)
        {
            Vector3 playerSpawn = randomPointOnUnitCircle(spawns[0], MagicNumbers.PlayerSpawnRadius);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, playerSpawn + new Vector3(0, 10, 0), Quaternion.identity, player);
            Debug.Log("AA - Am I right? - " + networkPlayerObject == null);
            // Keep track of the player avatars so we can remove it when they disconnect
            _spawnedCharacters.Add(player, networkPlayerObject);

            GameObject clientSingletonObj = Instantiate(clientSingleton, spawns[0], Quaternion.identity);
            clientSingletonObj.name = MagicWords.GameObjectNames.ClientSingleton;

            return networkPlayerObject.gameObject;
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

            if (Input.GetMouseButton(0))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_FIRE;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ALPHA1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ALPHA2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ALPHA3;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ALPHA4;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ALPHA5;
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ALPHA6;
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ALPHA7;
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ALPHA8;
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ALPHA9;
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ALPHA0;
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
            if (isHost)
            {
                InitNetworkGameState(spawns);
            }
        }
        public void OnSceneLoadStart(NetworkRunner runner) {

        }

        private void OnGUI()
        {
            if (_runner == null)
            {
                if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
                {
                    networkMode = Fusion.GameMode.Host;
                    InitGame("TwoPlayer", new Vector3[] {
                        new Vector3(8.79f, 1f, 11f),
                                                new Vector3(8.79f, 1f, 11f)
                    });
                }
                if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
                {
                    networkMode = Fusion.GameMode.Client;
                    JoinGame("TestRoom", "TwoPlayer", new Vector3[] {
                        new Vector3(8.79f, 1f, 11f),
                                                new Vector3(8.79f, 1f, 11f)
                    });
                }
            }
        }
    }
}
