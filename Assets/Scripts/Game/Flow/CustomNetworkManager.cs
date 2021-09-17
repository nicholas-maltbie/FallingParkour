using UnityEngine;
using PropHunt.Character;
using System.Collections.Generic;
using MLAPI;

namespace PropHunt.Game.Flow
{
    public class CustomNetworkManager : NetworkManager
    {
        private Dictionary<ulong, string> playerNameLookup = new Dictionary<ulong, string>();

        [Header("Game Prefabs")]
        public GameTimer timerPrefab;

        public void OnEnable()
        {
            CharacterName.OnPlayerNameChange += SetPlayerName;
        }

        public void OnDisable()
        {
            CharacterName.OnPlayerNameChange -= SetPlayerName;
        }

        // public override void OnServerReady(NetworkConnection conn)
        // {
        //     this.OnServerReady(conn);
        //     PlayerConnectEvent connectEvent = new PlayerConnectEvent(conn);
        //     OnPlayerConnect?.Invoke(this, connectEvent);
        // }

        // public override void OnServerConnect(NetworkConnection conn)
        // {
        //     if (Instance != null)
        //     {
        //         StartCoroutine(SendJoinMessage(conn));
        //     }
        //     this.OnServerConnect(conn);
        // }

        public void SetPlayerName(object sender, PlayerNameChange nameChange)
        {
            playerNameLookup[nameChange.connId] = nameChange.newName;
        }

        // public IEnumerator SendJoinMessage(NetworkConnection conn)
        // {
        //     // Setup a waiting period to listen for a player to connect with a timeout
        //     float timeout = 10.0f;
        //     bool sent = false;
        //     System.EventHandler<PlayerNameChange> listener = (object source, PlayerNameChange changeArgs) =>
        //     {
        //         if (!sent && changeArgs.connId == conn.connectionId)
        //         {
        //             DebugChatLog.SendChatMessage(new ChatMessage("", $"{changeArgs.newName} has connected to the server"));
        //             sent = true;
        //         }
        //     };
        //     CharacterName.OnPlayerNameChange += listener;
        //     // Wait for up to timeout period
        //     yield return new WaitForSeconds(timeout);

        //     // If a chat message has not been sent (and the player is still connected to the server)
        //     // by the time the timeout has completed, send a default message
        //     if (!sent && NetworkServer.connections.ContainsKey(conn.connectionId))
        //     {
        //         DebugChatLog.SendChatMessage(new ChatMessage("", $"Player {conn.connectionId} has connected to the server"));
        //     }

        //     // After the timeout has passed, delete the listener
        //     CharacterName.OnPlayerNameChange -= listener;
        // }

        // public override void OnStartServer()
        // {
        //     this.OnStartServer();
        //     playerNameLookup ;
        // }

        // public override void OnStartClient()
        // {
        //     // Register timer prefab if it is not already registered
        //     if (timerPrefab != null && !NetworkClient.prefabs.ContainsValue(timerPrefab.gameObject))
        //     {
        //         NetworkClient.RegisterPrefab(timerPrefab.gameObject);
        //     }

        //     this.OnStartClient();
        //     DebugChatLog.ClearChatLog();
        //     NetworkClient.RegisterHandler<ChatMessage>(DebugChatLog.OnMessage);
        //     NetworkClient.RegisterHandler<SoundEffectEvent>(SoundEffectManager.CreateSoundEffectAtPoint);
        //     NetworkClient.RegisterHandler<LevelSelectEvent>(LevelSelectList.LevelSelect);
        // }

        // public override void OnStopClient()
        // {
        //     this.OnStopClient();
        //     NetworkClient.UnregisterHandler<ChatMessage>();
        //     NetworkClient.UnregisterHandler<SoundEffectEvent>();
        //     NetworkClient.UnregisterHandler<LevelSelectEvent>();
        // }

        // public override void OnServerDisconnect(NetworkConnection conn)
        // {
        //     string playerName =
        //         playerNameLookup.ContainsKey(conn.connectionId) ?
        //         playerNameLookup[conn.connectionId] : "Player" + conn.connectionId;
        //     DebugChatLog.SendChatMessage(new ChatMessage("", $"{playerName} disconnected from server"));
        //     this.OnServerDisconnect(conn);
        // }
    }
}
