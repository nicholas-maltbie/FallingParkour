using PropHunt.Game.Communication;
using System;
using PropHunt.Utils;

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
        Disabled,
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
    public static class GameManager
    {
        public static event EventHandler<GamePhaseChange> OnGamePhaseChange;

        public static GamePhase gamePhase { get; private set; }

        public static float phaseStart;

        public static IUnityService unityService = new UnityService();

        public static void ChangePhase(GamePhase newPhase)
        {
            if (newPhase == gamePhase)
            {
                return;
            }
            DebugChatLog.SendChatMessage(new ChatMessage("", $"Entering {newPhase.ToString()} Phase"));
            GamePhaseChange changeEvent = new GamePhaseChange(gamePhase, newPhase);
            gamePhase = newPhase;
            OnGamePhaseChange?.Invoke(null, changeEvent);
            phaseStart = unityService.time;
        }
    }
}
