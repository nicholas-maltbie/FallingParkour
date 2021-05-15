using UnityEngine;
using Mirror;
using PropHunt.Game.Communication;
using System;
using PropHunt.Utils;
using PropHunt.Prop;
using System.Collections;

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
    public static class GameManager
    {
        public static event EventHandler<GamePhaseChange> OnGamePhaseChange;

        public static GamePhase gamePhase { get; private set; }

        public static float phaseStart;

        public static GameObject playerPrefab;

        public static IUnityService unityService = new UnityService();

        public static void SetupHooks()
        {
            CustomNetworkManager.OnPlayerConnect += HandlePlayerConnect;
            OnGamePhaseChange += HandleGamePhaseChange;
        }

        public static void DisableHooks()
        {
            CustomNetworkManager.OnPlayerConnect -= HandlePlayerConnect;
            OnGamePhaseChange -= HandleGamePhaseChange;
        }

        public static void HandlePlayerConnect(object sender, PlayerConnectEvent joinEvent)
        {
            // If in game, spawn a player for them... debug behaviour yay
            if (gamePhase == GamePhase.InGame || gamePhase == GamePhase.Score)
            {
                GameObject newPlayer = GameObject.Instantiate(playerPrefab);
                NetworkServer.DestroyPlayerForConnection(joinEvent.connection);
                NetworkServer.AddPlayerForConnection(joinEvent.connection, newPlayer);
            }
        }

        public static void ChangePhase(GamePhase newPhase)
        {
            GamePhaseChange changeEvent = new GamePhaseChange(gamePhase, newPhase);
            gamePhase = newPhase;
            OnGamePhaseChange?.Invoke(null, changeEvent);
        }

        public static IEnumerator SpawnPlayerCharacter(NetworkConnection conn)
        {
            yield return null;
            while (!conn.isReady)
            {
                yield return null;
            }
            GameObject newPlayer = GameObject.Instantiate(playerPrefab);
            NetworkServer.DestroyPlayerForConnection(conn);
            NetworkServer.AddPlayerForConnection(conn, newPlayer);
        }

        public static void HandleGamePhaseChange(object sender, GamePhaseChange change)
        {
            // Reset phase timer
            phaseStart = unityService.time;

            // Handle whenever the game state changes
            switch (change.next)
            {
                // Do things differently based on the new phase
                case GamePhase.Lobby:
                    DebugChatLog.SendChatMessage(new ChatMessage("", "Entering Lobby Phase"));

                    break;
                case GamePhase.Setup:
                    DebugChatLog.SendChatMessage(new ChatMessage("", "Entering Setup Phase"));
                    CustomNetworkManager.Instance.LoadGameScene();
                    // Once loading is complete, go to InGame
                    break;
                case GamePhase.InGame:
                    DebugChatLog.SendChatMessage(new ChatMessage("", "Entering In Game Phase"));
                    // When in game starts, spawn a player for each connection
                    foreach (NetworkConnection conn in NetworkServer.connections.Values)
                    {
                        SpawnPlayerCharacter(conn);
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
                    CustomNetworkManager.Instance.LoadLobbyScene();
                    // Once laoding is complete, go to lobby
                    break;
            }
        }
    }
}
