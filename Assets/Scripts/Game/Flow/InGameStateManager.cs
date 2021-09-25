using System;
using System.Linq;
using MLAPI;
using MLAPI.NetworkVariable;

namespace PropHunt.Game.Flow
{

    /// <summary>
    /// Various states of the game once play has started.
    /// </summary>
    public enum InGameState
    {
        Uninitialized = 0,
        Setup = 10,
        Countdown = 20,
        Running = 30,
        Reset = 40,
        Complete = 50,
    }

    /// <summary>
    /// Events that occur when in game state changes.
    /// </summary>
    public class InGameStateChange : EventArgs
    {
        public InGameState previousState;
        public InGameState currentState;
    }

    /// <summary>
    /// State machine for controlling for of events In Game.
    /// </summary>
    public class InGameStateManager : NetworkBehaviour
    {
        /// <summary>
        /// Singleton instance of InGameStateManager.
        /// </summary>
        public static InGameStateManager Singleton;

        /// <summary>
        /// Changes in game state.
        /// </summary>
        public static EventHandler<InGameStateChange> InGameStateChange;

        /// <summary>
        /// Network variable for synchronizing in game state.
        /// </summary>
        private NetworkVariable<InGameState> currentGameState = new NetworkVariable<InGameState>(InGameState.Uninitialized);

        /// <summary>
        /// Previous state of in game phase.
        /// </summary>
        private InGameState previousState = InGameState.Uninitialized;

        /// <summary>
        /// Current state of InGameStateManager as of now.
        /// </summary>
        public InGameState CurrentState => currentGameState.Value;

        /// <summary>
        /// Max game time to set for current game mode (in seconds)
        /// </summary>
        public float MaxGameLength = 60 * 5;

        /// <summary>
        /// Time to spend in cooldown before game starts!
        /// </summary>
        public float CooldownTime = 5.0f;

        public void StartGame()
        {
            if (this.currentGameState.Value == InGameState.Uninitialized)
            {
                this.currentGameState.Value = InGameState.Setup;
            }
        }

        public void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
            }
        }

        public void OnDestroy()
        {
            if (Singleton == this)
            {
                Singleton = null;
            }
        }

        public void OnEnable()
        {
            InGameStateChange += OnUpdateState;
        }

        public void OnDisable()
        {
            InGameStateChange -= OnUpdateState;
        }

        private void OnUpdateState(object source, InGameStateChange change)
        {
            if (IsServer)
            {
                switch (change.currentState)
                {
                    case InGameState.Countdown:
                        GameSceneManager.Singleton.SetupTimer();
                        GameSceneManager.Singleton.GamePhaseTimer.OnFinish +=
                            (source, args) => currentGameState.Value = InGameState.Running;
                        GameSceneManager.Singleton.GamePhaseTimer.StartTimer(CooldownTime);
                        break;
                    case InGameState.Running:
                        // Start timer for game phase
                        GameSceneManager.Singleton.SetupTimer();
                        GameSceneManager.Singleton.GamePhaseTimer.OnFinish +=
                            (source, args) => currentGameState.Value = InGameState.Running;
                        GameSceneManager.Singleton.GamePhaseTimer.StartTimer(MaxGameLength);
                        break;
                }
            }
        }

        public void Update()
        {
            if (CurrentState != previousState)
            {
                InGameStateChange?.Invoke(
                    this,
                    new Flow.InGameStateChange
                    {
                        previousState = previousState,
                        currentState = CurrentState
                    });
            }
            previousState = CurrentState;

            // Only update game state manager on server
            if (!IsServer)
            {
                return;
            }

            switch (CurrentState)
            {
                case InGameState.Setup:
                    // Ensure setup has completed.
                    bool setupDone = true;

                    // Once setup is done, switch to countdown.
                    if (setupDone && GameSceneManager.Singleton.Progress.IsAllClientsDoneLoading)
                    {
                        GameSceneManager.Singleton.ClearTimer();
                        currentGameState.Value = InGameState.Countdown;
                    }
                    break;
                case InGameState.Countdown:
                    // Wait for countdown to finish... nothing to be done here
                    break;
                case InGameState.Running:
                    // If the game is running, check if we can exit early!
                    bool endEarly = false;

                    // If the number of "completed" states is equal to the number of players, we have completed the
                    // level
                    int completed = NetworkManager.ConnectedClientsList
                        .Select(client => client.PlayerObject)
                        .Where(playerObj => playerObj != null)
                        .Select(playerObj => playerObj.GetComponent<CompletedLevel>())
                        .Where(completionStatus => completionStatus != null)
                        .Where(completionStatus => completionStatus.Status.Value == CompletedStatus.Completed)
                        .Select(status => 1)
                        .Sum();

                    if (completed == NetworkManager.ConnectedClientsList.Count)
                    {
                        endEarly = true;
                    }


                    // Otherwise, wait for round to end normally
                    if (endEarly)
                    {
                        // Transition to reset phase and ensure timer has been cleared
                        GameSceneManager.Singleton.ClearTimer();
                        currentGameState.Value = InGameState.Reset;
                    }
                    break;
                case InGameState.Reset:
                    // Check to see if game state can change out of running
                    bool resetDone = true;

                    if (resetDone)
                    {
                        // Close state machine
                        currentGameState.Value = InGameState.Complete;

                        // Transition to score phase
                        GameManager.ChangePhase(GamePhase.Score);
                    }
                    break;
            }
        }
    }
}
