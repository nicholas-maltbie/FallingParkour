using UnityEngine;
using PropHunt.Game.Level;
using MLAPI;
using MLAPI.SceneManagement;
using MLAPI.Connection;
using UnityEngine.SceneManagement;

namespace PropHunt.Game.Flow
{
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Singleton;

        [Header("Scene Management")]

        public string offlineScene;

        public string lobbyScene;

        public string gameScene { get; private set; }

        public GameLevelLibrary levelLibrary;

        [Header("Manager Prefabs")]

        [SerializeField]
        private GameObject timerPrefab;

        [SerializeField]
        private GameObject gameStateManagerPrefab;

        [Header("Game Configuration")]

        public float scoreScreenLength = 1.0f;

        public GameTimer GamePhaseTimer { get; private set; }

        public InGameStateManager GameStateManager { get; private set; }

        private static bool loaded = false;

        /// <summary>
        /// Progress toward switching to next scene
        /// </summary>
        public SceneSwitchProgress Progress { get; private set; }

        /// <summary>
        /// Is the player online?
        /// </summary>
        private bool previousOnlineState = false;

        public void Start()
        {
            if (loaded == false)
            {
                loaded = true;
                Singleton = this;
            }

            if (string.IsNullOrEmpty(gameScene))
            {
                gameScene = levelLibrary.DefaultLevel.levelName;
            }
            GameManager.OnGamePhaseChange += HandleGamePhaseChange;
        }

        public void OnDestroy()
        {
            if (Singleton == this)
            {
                loaded = false;
                Singleton = null;
            }
        }

        public void OnSceneSwitch()
        {

        }

        public void ChangeGameScene(string newScene)
        {
            gameScene = newScene;
        }

        public void OnStartServer()
        {
            this.Progress = NetworkSceneManager.SwitchScene(lobbyScene);
            GameManager.ChangePhase(GamePhase.Lobby);
        }

        /// <summary>
        /// Stop and destory the current game phase timer
        /// </summary>
        public void ClearTimer()
        {
            if (this.GamePhaseTimer != null)
            {
                this.GamePhaseTimer.StopTimer();
                GameObject.Destroy(this.GamePhaseTimer.gameObject);
                this.GamePhaseTimer = null;
            }
        }

        /// <summary>
        /// Clear out previous timer and create a new one
        /// </summary>
        public void SetupTimer()
        {
            ClearTimer();
            this.GamePhaseTimer = GameObject.Instantiate(timerPrefab).GetComponent<GameTimer>();
            this.GamePhaseTimer.GetComponent<NetworkObject>().Spawn();
        }

        public void HandleGamePhaseChange(object sender, GamePhaseChange change)
        {
            // Cleanup from previous game state
            switch (change.previous)
            {
                // Do things differently based on the new phase
                case GamePhase.Lobby:
                    break;
                case GamePhase.Setup:
                    break;
                case GamePhase.InGame:
                    // Close the game state manager
                    this.GameStateManager.GetComponent<NetworkObject>().Despawn(true);
                    break;
                case GamePhase.Score:
                    ClearTimer();
                    break;
                case GamePhase.Reset:
                    break;
            }
            // Handle whenever the game state changes
            switch (change.next)
            {
                // Do things differently based on the new phase
                case GamePhase.Lobby:
                    break;
                case GamePhase.Setup:
                    // NetworkServer.SetAllClientsNotReady();
                    this.Progress = NetworkSceneManager.SwitchScene(gameScene);
                    // Once loading is complete, go to InGame
                    break;
                case GamePhase.InGame:
                    // Setup the InGameStateManager for this current game
                    GameObject gameStateGo = GameObject.Instantiate(gameStateManagerPrefab);
                    this.GameStateManager = gameStateGo.GetComponent<InGameStateManager>();

                    // Spawn object over network
                    gameStateGo.GetComponent<NetworkObject>().Spawn();

                    // Start game state manager
                    this.GameStateManager.StartGame();

                    break;
                case GamePhase.Score:
                    // Setup a timer for the score phase
                    SetupTimer();
                    // Transition to reset phase upon completion of timer
                    this.GamePhaseTimer.OnFinish += (source, args) => GameManager.ChangePhase(GamePhase.Reset);
                    // Start the timer with a time of 10 seconds
                    this.GamePhaseTimer.StartTimer(scoreScreenLength);
                    break;
                case GamePhase.Reset:
                    // Start loading the lobby scene
                    UnityEngine.Debug.Log("Loading scene: " + lobbyScene);
                    this.Progress = NetworkSceneManager.SwitchScene(lobbyScene);
                    // Once laoding is complete, go to lobby
                    break;
            }
        }

        public void Update()
        {
            bool onlineState = NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer;

            if (!onlineState && previousOnlineState)
            {
                SceneManager.LoadScene(offlineScene);
                GameManager.ChangePhase(GamePhase.Disabled);
            }

            previousOnlineState = onlineState;

            // Only run this on server
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }
            switch (GameManager.gamePhase)
            {
                case GamePhase.Disabled:
                    UnityEngine.Debug.Log("Game In Disabled State");
                    if (NetworkManager.Singleton.IsHost)
                    {
                        GameManager.ChangePhase(GamePhase.Reset);
                    }
                    break;
                case GamePhase.Setup:
                    // As soon as scene is loaded, move to in game
                    if (Progress.IsAllClientsDoneLoading && Progress.IsCompleted)
                    {
                        GameManager.ChangePhase(GamePhase.InGame);
                    }
                    break;
                case GamePhase.Reset:
                    if (Progress.IsAllClientsDoneLoading && Progress.IsCompleted)
                    {
                        GameManager.ChangePhase(GamePhase.Lobby);
                    }
                    break;
            }
        }
    }
}
