using UnityEngine;
using Mirror;
using System.Collections;
using PropHunt.Environment.Checkpoint;

namespace PropHunt.Game.Flow
{
    /// <summary>
    /// Spawn manager for creating and managing player in the game. 
    /// </summary>
    public class PlayerSpawnManager : NetworkBehaviour
    {
        /// <summary>
        /// In game player for running through levels.
        /// </summary>
        public GameObject inGamePlayer;

        /// <summary>
        /// Lobby player for sitting in the lobby scene.
        /// </summary>
        public GameObject lobbyPlayer;

        /// <summary>
        /// Spectator player for watching other players in the game.
        /// </summary>
        public GameObject spectatorPlayer;

        public override void OnStartClient()
        {
            base.OnStartClient();
            NetworkClient.RegisterPrefab(inGamePlayer);
            NetworkClient.RegisterPrefab(lobbyPlayer);
            NetworkClient.RegisterPrefab(spectatorPlayer);
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
                ISpawnPointCollection defaultCheckpoint = SpawnPointUtilities.GetDefaultCheckpoint();
                Vector3 pos = Vector3.zero;
                Quaternion rot = Quaternion.identity;
                if (defaultCheckpoint != null)
                {
                    (pos, rot) = defaultCheckpoint.GetRandomSpawn();
                }
                StartCoroutine(SpawnPlayerCharacter(joinEvent.connection, spectatorPlayer, pos, rot));
            }
            else if (GameManager.gamePhase == GamePhase.Lobby)
            {
                StartCoroutine(SpawnPlayerCharacter(joinEvent.connection, lobbyPlayer));
            }
            else
            {
                // Default spawn a lobby player for the player character
                StartCoroutine(SpawnPlayerCharacter(joinEvent.connection, lobbyPlayer));
            }
        }

        public IEnumerator SpawnPlayerCharacter(NetworkConnection conn, GameObject playerPrefab)
        {
            return SpawnPlayerCharacter(conn, playerPrefab, Vector3.zero, Quaternion.identity);
        }

        public IEnumerator SpawnPlayerCharacter(NetworkConnection conn, GameObject playerPrefab, Vector3 pos, Quaternion rotation)
        {
            // if (!conn.isReady)
            // {
            yield return null;
            // }
            GameObject newPlayer = GameObject.Instantiate(playerPrefab);
            newPlayer.transform.rotation = rotation;
            newPlayer.transform.position = pos;
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
                    ISpawnPointCollection defaultCheckpoint = SpawnPointUtilities.GetDefaultCheckpoint();
                    var spawnEnum = defaultCheckpoint != null ?
                        defaultCheckpoint.GetRandomizedSpawns().GetEnumerator() : null;
                    foreach (NetworkConnection conn in NetworkServer.connections.Values)
                    {
                        Vector3 pos = Vector3.zero;
                        Quaternion rot = Quaternion.identity;
                        if (spawnEnum != null)
                        {
                            if (!spawnEnum.MoveNext())
                            {
                                spawnEnum = defaultCheckpoint.GetRandomizedSpawns().GetEnumerator();
                            }
                            (pos, rot) = spawnEnum.Current;
                        }
                        StartCoroutine(SpawnPlayerCharacter(conn, inGamePlayer, pos, rot));
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
