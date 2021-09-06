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
        [Header("Scene Management")]

        public string offlineScene;

        public string lobbyScene;

        public string gameScene { get; private set; }

        public GameLevelLibrary levelLibrary;

        [SerializeField]
        private GameObject timerPrefab;


        private GameTimer gamePhaseTimer;

        private static bool loaded = false;

        /// <summary>
        /// Buffer time between setup and in game phases
        /// </summary>
        public float bufferTime = 1.0f;

        /// <summary>
        /// Buffer time that has elapsed so far
        /// </summary>
        private float bufferElapsed = 0.0f;

        /// <summary>
        /// Is the player online?
        /// </summary>
        private bool previousOnlineState = false;

        public void Start()
        {
            if (string.IsNullOrEmpty(gameScene))
            {
                gameScene = levelLibrary.DefaultLevel.levelName;
            }
            GameManager.OnGamePhaseChange += HandleGamePhaseChange;
        }

        public void ChangeGameScene(string newScene)
        {
            gameScene = newScene;
        }

        public void OnStartServer()
        {
            NetworkSceneManager.SwitchScene(lobbyScene);
            GameManager.ChangePhase(GamePhase.Lobby);
        }

        /// <summary>
        /// Stop and destory the current game phase timer
        /// </summary>
        public void ClearTimer()
        {
            if (this.gamePhaseTimer != null)
            {
                this.gamePhaseTimer.StopTimer();
                GameObject.Destroy(this.gamePhaseTimer.gameObject);
                this.gamePhaseTimer = null;
            }
        }

        /// <summary>
        /// Clear out previous timer and create a new one
        /// </summary>
        public void SetupTimer()
        {
            ClearTimer();
            this.gamePhaseTimer = GameObject.Instantiate(timerPrefab).GetComponent<GameTimer>();
            this.gamePhaseTimer.GetComponent<NetworkObject>().Spawn();
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
                    ClearTimer();
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
                    bufferElapsed = 0.0f;
                    // NetworkServer.SetAllClientsNotReady();
                    NetworkSceneManager.SwitchScene(gameScene);
                    // Once loading is complete, go to InGame
                    break;
                case GamePhase.InGame:
                    // Setup a timer for the game
                    SetupTimer();
                    // Transition to score phase upon completion of timer
                    this.gamePhaseTimer.OnFinish += (source, args) => GameManager.ChangePhase(GamePhase.Score);
                    // Start the timer with a time of 5 minutes
                    this.gamePhaseTimer.StartTimer(60 * 5);
                    break;
                case GamePhase.Score:
                    // Setup a timer for the score phase
                    SetupTimer();
                    // Transition to reset phase upon completion of timer
                    this.gamePhaseTimer.OnFinish += (source, args) => GameManager.ChangePhase(GamePhase.Reset);
                    // Start the timer with a time of 30 seconds
                    this.gamePhaseTimer.StartTimer(30);
                    break;
                case GamePhase.Reset:
                    // Start loading the lobby scene
                    UnityEngine.Debug.Log("Loading scene: " + lobbyScene);
                    NetworkSceneManager.SwitchScene(lobbyScene);
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
                    bool allReady = true;
                    foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                    {
                        // if (!client.isReady)
                        // {
                        //     allReady = false;
                        // }
                    }
                    if (allReady)
                    {
                        bufferElapsed += Time.deltaTime;
                    }
                    // As soon as scene is loaded, move to in game
                    if (bufferElapsed >= bufferTime &&
                        allReady
                        // (NetworkManager.loadingSceneAsync == null || NetworkManager.loadingSceneAsync.isDone)
                        )
                    {
                        GameManager.ChangePhase(GamePhase.InGame);
                    }
                    break;
                case GamePhase.Reset:
                    GameManager.ChangePhase(GamePhase.Lobby);
                    break;
            }
        }
    }
}
