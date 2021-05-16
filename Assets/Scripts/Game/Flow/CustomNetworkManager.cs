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
            PlayerConnectEvent connectEvent = new PlayerConnectEvent(conn);
            OnPlayerConnect?.Invoke(this, connectEvent);
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            DebugChatLog.SendChatMessage(new ChatMessage("", $"Player {conn.connectionId} connected to server"));
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
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
            NetworkClient.UnregisterHandler<SoundEffectEvent>();
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            DebugChatLog.SendChatMessage(new ChatMessage("", $"Player {conn.connectionId} disconnected from server"));
        }
    }
}
