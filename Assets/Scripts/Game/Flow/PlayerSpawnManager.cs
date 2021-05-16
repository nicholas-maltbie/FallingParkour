using UnityEngine;
using Mirror;
using System.Collections;

namespace PropHunt.Game.Flow
{
    public class PlayerSpawnManager : NetworkBehaviour
    {
        public GameObject inGamePlayer;

        public GameObject lobbyPlayer;

        public override void OnStartClient()
        {
            base.OnStartClient();
            NetworkClient.RegisterPrefab(inGamePlayer);
            NetworkClient.RegisterPrefab(lobbyPlayer);
        }

        public override void OnStartServer()
        {
            UnityEngine.Debug.Log("Setting up spawn manager");
            CustomNetworkManager.OnPlayerConnect += HandlePlayerConnect;
            GameManager.OnGamePhaseChange += HandleGamePhaseChange;
        }

        public void OnDisable()
        {
            OnStopServer();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            CustomNetworkManager.OnPlayerConnect -= HandlePlayerConnect;
            GameManager.OnGamePhaseChange -= HandleGamePhaseChange;
        }

        public void Start()
        {
            HandleGamePhaseChange(this, new GamePhaseChange(GameManager.gamePhase, GameManager.gamePhase));
        }

        public void HandlePlayerConnect(object sender, PlayerConnectEvent joinEvent)
        {
            // If in game, spawn a player for them... debug behaviour yay
            if (GameManager.gamePhase == GamePhase.InGame || GameManager.gamePhase == GamePhase.Score)
            {
                StartCoroutine(SpawnPlayerCharacter(joinEvent.connection, inGamePlayer));
            }
            else if (GameManager.gamePhase == GamePhase.Lobby)
            {
                StartCoroutine(SpawnPlayerCharacter(joinEvent.connection, lobbyPlayer));
            }
        }

        public IEnumerator SpawnPlayerCharacter(NetworkConnection conn, GameObject playerPrefab)
        {
            if (!conn.isReady)
            {
                yield return null;
            }
            GameObject newPlayer = GameObject.Instantiate(playerPrefab);
            NetworkServer.DestroyPlayerForConnection(conn);
            NetworkServer.AddPlayerForConnection(conn, newPlayer);
        }

        public void HandleGamePhaseChange(object sender, GamePhaseChange change)
        {
            // Handle whenever the game state changes
            switch (change.next)
            {
                // Do things differently based on the new phase
                case GamePhase.Lobby:
                    // Spawn a player for each active connection (lobby player)
                    foreach (NetworkConnection conn in NetworkServer.connections.Values)
                    {
                        StartCoroutine(SpawnPlayerCharacter(conn, lobbyPlayer));
                    }
                    break;
                case GamePhase.Setup:
                    // Clean up existing players
                    foreach (NetworkConnection conn in NetworkServer.connections.Values)
                    {
                        NetworkServer.DestroyPlayerForConnection(conn);
                    }
                    break;
                case GamePhase.InGame:
                    UnityEngine.Debug.Log("Spawning characters");
                    // When in game starts, spawn a player for each connection
                    foreach (NetworkConnection conn in NetworkServer.connections.Values)
                    {
                        StartCoroutine(SpawnPlayerCharacter(conn, inGamePlayer));
                    }
                    break;
                case GamePhase.Score:
                    break;
                case GamePhase.Reset:
                    // Destory spawned players for each connection
                    foreach (NetworkConnection conn in NetworkServer.connections.Values)
                    {
                        NetworkServer.DestroyPlayerForConnection(conn);
                    }
                    // Once laoding is complete, go to lobby
                    break;
            }
        }
    }
}
