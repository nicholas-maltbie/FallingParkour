using UnityEngine;
using Mirror;
using PropHunt.Game.Communication;
using System;
using PropHunt.Environment.Sound;
using UnityEngine.SceneManagement;
using System.Collections;

namespace PropHunt.Game.Flow
{
    public class PlayerConnectEvent : EventArgs
    {
        public readonly NetworkConnection connection;

        public PlayerConnectEvent(NetworkConnection connection)
        {
            this.connection = connection;
        }
    }

    public class CustomNetworkManager : NetworkManager
    {
        public static event EventHandler<PlayerConnectEvent> OnPlayerConnect;

        [Scene]
        public string lobbyScene;

        [Scene]
        public string gameScene;

        public static CustomNetworkManager Instance;

        public override void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                // Only let one exist
                GameObject.Destroy(this);
                return;
            }

            base.Awake();
        }

        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);
            PlayerConnectEvent connectEvent = new PlayerConnectEvent(conn);
            OnPlayerConnect?.Invoke(this, connectEvent);
            GameManager.playerPrefab = playerPrefab;
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            DebugChatLog.SendChatMessage(new ChatMessage("", $"Player {conn.connectionId} connected to server"));
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            DebugChatLog.ClearChatLog();
            NetworkClient.RegisterHandler<ChatMessage>(DebugChatLog.OnMessage);
            NetworkClient.RegisterHandler<SoundEffectEvent>(SoundEffectManager.CreateSoundEffectAtPoint);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            NetworkClient.UnregisterHandler<ChatMessage>();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            GameManager.SetupHooks();
            GameManager.ChangePhase(GamePhase.Lobby);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            GameManager.DisableHooks();
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            DebugChatLog.SendChatMessage(new ChatMessage("", $"Player {conn.connectionId} disconnected from server"));
        }

        public void Update()
        {
            // Only run this on server
            if (!NetworkServer.active)
            {
                return;
            }
            switch (GameManager.gamePhase)
            {
                case GamePhase.Setup:
                    // As soon as scene is loaded, move to in game
                    if (NetworkManager.loadingSceneAsync == null || NetworkManager.loadingSceneAsync.isDone)
                    {
                        GameManager.ChangePhase(GamePhase.InGame);
                    }
                    break;
                case GamePhase.Reset:
                    // As soon as scene is loaded, move to in game
                    if (NetworkManager.loadingSceneAsync == null || NetworkManager.loadingSceneAsync.isDone)
                    {
                        GameManager.ChangePhase(GamePhase.Lobby);
                    }
                    break;
            }
        }

        /// <summary>
        /// Load the lobby scene for players
        /// </summary>
        public void LoadLobbyScene()
        {
            base.ServerChangeScene(lobbyScene);
        }

        /// <summary>
        /// Load the game scene for players
        /// </summary>
        public void LoadGameScene()
        {
            base.ServerChangeScene(gameScene);
        }
    }
}
