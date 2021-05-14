using UnityEngine;
using Mirror;
using PropHunt.Game.Communication;
using System;
using PropHunt.Utils;
using PropHunt.Prop;

namespace PropHunt.Game.Flow
{
    /// <summary>
    /// Phases that the game can be in. Each of these phases acts as a state of 
    /// a state machine in which the initial state is Lobby. When the game starts, 
    /// the game will go into setup and game phases. Once a round ends, the game
    /// will move into Score screen and Reset.
    /// </summary>
    public enum GamePhase
    {
        Lobby,
        Setup,
        InGame,
        Score,
        Reset,
    }

    /// <summary>
    /// Game phase change event that saves the previous and next game states
    /// </summary>
    public class GamePhaseChange : EventArgs
    {
        /// <summary>
        /// Previous game state
        /// </summary>
        public readonly GamePhase previous;
        /// <summary>
        /// Next game state
        /// </summary>
        public readonly GamePhase next;

        public GamePhaseChange(GamePhase previous, GamePhase next)
        {
            this.previous = previous;
            this.next = next;
        }
    }

    /// <summary>
    /// Game manager for managing phases of the game
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static event EventHandler<GamePhaseChange> OnGamePhaseChange;

        public GamePhase gamePhase;

        /// <summary>
        /// How long have we been in the current phase
        /// </summary>
        public float phaseTime;

        private CustomNetworkManager networkManager;

        public GameObject playerPrefab;

        public IUnityService unityService = new UnityService();

        public INetworkService networkService = new NetworkService(null);

        public static GameManager Instance;

        public void Start()
        {
            if (GameManager.Instance == null)
            {
                GameManager.Instance = this;
            }
            else
            {
                return;
            }
            CustomNetworkManager.OnPlayerConnect += HandlePlayerConnect;
            OnGamePhaseChange += HandleGamePhaseChange;

            this.networkManager = GameObject.FindObjectOfType<CustomNetworkManager>();

            if (!NetworkClient.prefabs.ContainsValue(playerPrefab))
            {
                NetworkClient.RegisterPrefab(playerPrefab);
            }
            DontDestroyOnLoad(gameObject);
        }

        public void OnDestory()
        {
            CustomNetworkManager.OnPlayerConnect -= HandlePlayerConnect;
            Instance = null;
        }

        public void HandlePlayerConnect(object sender, PlayerConnectEvent joinEvent)
        {
            // If in game, spawn a player for them... debug behaviour yay
            if (gamePhase == GamePhase.InGame || gamePhase == GamePhase.Score)
            {
                GameObject newPlayer = GameObject.Instantiate(playerPrefab);
                NetworkServer.DestroyPlayerForConnection(joinEvent.connection);
                NetworkServer.AddPlayerForConnection(joinEvent.connection, newPlayer);
            }
        }

        public void ChangePhase(GamePhase newPhase)
        {
            GamePhaseChange changeEvent = new GamePhaseChange(gamePhase, newPhase);
            OnGamePhaseChange?.Invoke(this, changeEvent);
            gamePhase = newPhase;
        }

        public void Update()
        {
            if (!networkService.activeNetworkServer)
            {
                return;
            }

            // Increment current phase time
            phaseTime += unityService.deltaTime;

            switch (gamePhase)
            {
                // Do things differently based on phase
                case GamePhase.Lobby:

                    break;
                case GamePhase.Setup:
                    // Once loading is complete, go to InGame
                    if (NetworkManager.loadingSceneAsync == null || NetworkManager.loadingSceneAsync.isDone)
                    {
                        ChangePhase(GamePhase.InGame);
                    }
                    break;
                case GamePhase.InGame:
                    // Check for conditions to end in game phase
                    //   i. Game timeout (time runs out)
                    //  ii. Hunters win (enough props were caught)
                    // iii. Props win (all props finished objectives)
                    break;
                case GamePhase.Score:
                    // Display score screen to players
                    //  End phase either when players have all hit continue or timeout has ocurred
                    break;
                case GamePhase.Reset:
                    // Once laoding is complete, go to lobby
                    if (NetworkManager.loadingSceneAsync == null || NetworkManager.loadingSceneAsync.isDone)
                    {
                        ChangePhase(GamePhase.Lobby);
                    }
                    break;
            }
        }

        public void HandleGamePhaseChange(object sender, GamePhaseChange change)
        {
            if (!networkService.activeNetworkServer)
            {
                return;
            }

            // Reset phase timer
            phaseTime = 0;

            // Handle whenever the game state changes
            switch (change.next)
            {
                // Do things differently based on the new phase
                case GamePhase.Lobby:
                    DebugChatLog.SendChatMessage(new ChatMessage("", "Entering Lobby Phase"));

                    break;
                case GamePhase.Setup:
                    DebugChatLog.SendChatMessage(new ChatMessage("", "Entering Setup Phase"));
                    networkManager.LoadGameScene();
                    // Once loading is complete, go to InGame
                    break;
                case GamePhase.InGame:
                    DebugChatLog.SendChatMessage(new ChatMessage("", "Entering In Game Phase"));
                    // When in game starts, spawn a player for each connection
                    foreach (NetworkConnection conn in NetworkServer.connections.Values)
                    {
                        GameObject newPlayer = GameObject.Instantiate(playerPrefab);
                        NetworkServer.DestroyPlayerForConnection(conn);
                        NetworkServer.AddPlayerForConnection(conn, newPlayer);
                    }
                    break;
                case GamePhase.Score:
                    DebugChatLog.SendChatMessage(new ChatMessage("", "Entering Score Phase"));
                    break;
                case GamePhase.Reset:
                    DebugChatLog.SendChatMessage(new ChatMessage("", "Entering Reset Phase"));
                    // Destory the network clients for each player
                    foreach (NetworkConnection conn in NetworkServer.connections.Values)
                    {
                        NetworkServer.DestroyPlayerForConnection(conn);
                    }
                    // Start loading the lobby scene
                    UnityEngine.Debug.Log("Resetting game state back to lobby");
                    networkManager.LoadLobbyScene();
                    // Once laoding is complete, go to lobby
                    break;
            }
        }
    }
}
