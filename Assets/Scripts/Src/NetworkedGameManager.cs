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
using ExitGames.Client.Photon;
using static NetworkedGameManagerState;
using game.assets.utilities;
using System.Threading.Tasks;

namespace game.assets
{
    public enum ConnectionError
    {
        None,
        NoServerFound
    }
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
        public const uint BUTTON_ESC = 1 << 25;
        public float MOUSE_SCROLLWHEEL;

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
        public NetworkRunner _runner;

        [SerializeField] private NetworkPrefabRef _playerPrefab;
        [SerializeField] private NetworkPrefabRef _startingCity;
        [SerializeField] private NetworkPrefabRef _gameManagerState;
        [SerializeField] private NetworkPrefabRef _victoryManager;

        private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

        private Dictionary<PlayerRef, List<NetworkObject>> _spawnedEntities = new Dictionary<PlayerRef, List<NetworkObject>>();

        public bool playerCharacterFrozen = false;

        [SerializeField]
        private Scene targetScene;
        private GameObject playerObj;

        private bool isHost;

        private NetworkedGameManagerState state;

        public void InitGame(GameMap map, string roomName)
        {
            if (_runner == null)
            {
                isHost = true;
                StartGame(map, Fusion.GameMode.Host, roomName);
            }
        }

        public void JoinGame(string sessionName, string mapName)
        {
            if (_runner == null)
            {
                JoinExistingGame(sessionName);
            }
        }

        private void InitNetworkGameState()
        {
            if (state != null)
            {
                DestroyNetworkGameState();
            }

            if (!_runner.IsRunning)
            {
                return;
            }
            state = _runner.Spawn(_gameManagerState, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<NetworkedGameManagerState>();
            state.Init();
        }

        private void DestroyNetworkGameState()
        {
            if (state != null)
            {
                _runner.Despawn(state.GetComponent<NetworkObject>());
            }
        }

        async void StartGame(GameMap map, Fusion.GameMode mode, string roomName)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;

            var targetScene = SceneManager.LoadScene(map.sceneName, new LoadSceneParameters(LoadSceneMode.Single));

            this.gameMode = GameMode.Versus;

            var result = await _runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = roomName,
                Scene = targetScene.buildIndex,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
                PlayerCount = map.maxPlayers
            });

            if (!result.Ok)
            {
                QuitToMainMenu();
            }
        }

        async void JoinExistingGame(string roomName)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;

            this.gameMode = GameMode.Versus;

            var joinTask = _runner.StartGame(new StartGameArgs()
            {
                GameMode = Fusion.GameMode.Client,
                SessionName = roomName,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            });

            if (await Task.WhenAny(joinTask, Task.Delay(6000)) == joinTask)
            {
                
            }
            else
            {
                QuitToMainMenuWithError(ConnectionError.NoServerFound);
            }
        }



        public override Scene Initialize(string mapName, Vector3[] spawnPoints)
        {
            throw new NotImplementedException();
        }
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef networkPlayer)
        {
            if (isHost)
            {
                PlayerSlot playerDeets = state.ReserveNewPlayer(networkPlayer);
                _spawnedEntities.Add(networkPlayer, new List<NetworkObject>());
                var playerObj = instantiateNetworkedPlayerStart(runner, playerDeets);
                Player player = playerObj.GetComponent<Player>();
                player.resources = new utilities.resources.ResourceSet(100, 100);
                player.playerColourIndex = PlayerColourManager.IndexOfColour(playerDeets.colour);
                player.colour = playerDeets.colour;
                player.maxCount = 10;
                playerObj.GetComponent<Ownership>().setOwnerRecursively(player);
            }
        }

        private GameObject instantiateNetworkedPlayerStart(NetworkRunner runner, PlayerSlot playerDeets)
        {
            Vector3 playerSpawn = randomPointOnUnitCircle(playerDeets.spawnPoint, MagicNumbers.PlayerSpawnRadius);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, new Vector3(playerSpawn.x, playerDeets.spawnPoint.y, playerSpawn.z), Quaternion.identity, playerDeets.player);
            networkPlayerObject.GetComponent<Player>().networkPlayer = (PlayerRef)playerDeets.player;
            _spawnedCharacters.Add((PlayerRef)playerDeets.player, networkPlayerObject);
            playerObj = networkPlayerObject.gameObject;

            Instantiation.SpawnNetwork(
                runner,
                _startingCity,
                GameUtils.fixHeight(playerDeets.spawnPoint),
                Quaternion.identity,
                playerObj.GetComponent<Player>()
            );

            lockPlayer();

            Invoke("unlockPlayer", 0.2f);
            return playerObj;
        }

        private void lockPlayer()
        {
            playerObj.GetComponent<CharacterController>().enabled = false;
        }

        private void unlockPlayer()
        {
            playerObj.GetComponent<CharacterController>().enabled = true;
        }

        private void SpawnUI()
        {
            GameObject clientSingletonObj = Instantiate(clientSingleton, new Vector3(0, 0, 0), Quaternion.identity);
            clientSingletonObj.name = MagicWords.GameObjectNames.UI;
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            // Find and remove the players avatar
            if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
            {
                runner.Despawn(networkObject);
                _spawnedCharacters.Remove(player);
            }

            if (_spawnedEntities.TryGetValue(player, out List<NetworkObject> networkObjects))
            {
                for (int i = 0; i < networkObjects.Count; i++)
                {
                    runner.Despawn(networkObjects[i]);
                }

                networkObjects.Clear();

                _spawnedEntities.Remove(player);
            }

            if (isHost)
            {
                state.freePlayerSlot(player);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var frameworkInput = new PlayerNetworkInput();
            if (!playerCharacterFrozen)
            {
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

                if (Input.GetKeyDown(KeyCode.E))
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

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ACTION2;
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ACTION3;
                }

                if (Input.GetKey(KeyCode.G))
                {
                    frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ACTION4;
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_ESC;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_FIRE;
                }

                if (Input.GetMouseButtonDown(1))
                {
                    frameworkInput.Buttons |= PlayerNetworkInput.BUTTON_FIRE_ALT;
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
            }

            var localView = LocalPlayer.getView();
            if (localView != null)
            {
                frameworkInput.cameraRotationX = localView.getXRotation();
                frameworkInput.cameraRotationY = localView.getYRotation();
            }

            frameworkInput.MOUSE_SCROLLWHEEL = Input.GetAxis("Mouse ScrollWheel");

            input.Set(frameworkInput);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectedToServer(NetworkRunner runner) {
            Debug.Log("Hey there");
        }
        public void OnDisconnectedFromServer(NetworkRunner runner) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {
            Debug.Log("Failed");
        }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner)
        {
            if (isHost)
            {
                runner.Spawn(_victoryManager);
                InitNetworkGameState();
            }
            SpawnUI();
        }
        public void OnSceneLoadStart(NetworkRunner runner) {

        }

        private void ExitNetworkGame()
        {
            DestroyNetworkGameState();

            if (_runner == null)
            {
                return;
            }
            _runner.Shutdown();
        }



        public void registerNetworkObject(Player player, NetworkObject obj) {
            if (_spawnedEntities.TryGetValue(player.networkPlayer, out var objects))
            {
                objects.Add(obj);
            }
            else
            {
                Debug.LogError("No spawned entities listed for player " + player);
            }
        }

        public new static NetworkedGameManager Get()
        {
            return GameObject.Find(MagicWords.GameObjectNames.GameManager)?.GetComponent<NetworkedGameManager>();
        }

        public void QuitToMainMenu()
        {
            ExitNetworkGame();
            SceneManager.LoadScene(0);
            SceneManager.sceneLoaded += (scene, mode) => { Cursor.lockState = CursorLockMode.None; };
        }

        public void QuitToMainMenuWithError(ConnectionError error)
        {
            ExitNetworkGame();
            SceneManager.LoadScene(0);
            SceneManager.sceneLoaded += (scene, mode) => { 
                Cursor.lockState = CursorLockMode.None;
                GameObject.Find(GameUtils.MagicWords.GameObjectNames.GameManager).GetComponent<MainMenu>().DisplayErrorMessage(error);
            };
        }

        public void Quit()
        {
            ExitNetworkGame();
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }

        public void Despawn(NetworkObject obj)
        {
            _runner.Despawn(obj);
        }
    }
}
