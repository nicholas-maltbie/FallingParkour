using UnityEngine;
using System.Collections;
using PropHunt.Environment.Checkpoint;
using MLAPI;
using MLAPI.Connection;

namespace PropHunt.Game.Flow
{
    /// <summary>
    /// Spawn manager for creating and managing player in the game. 
    /// </summary>
    public class PlayerSpawnManager : MonoBehaviour
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

        public void OnEnable()
        {
            UnityEngine.Debug.Log("Setting up spawn manager");
            NetworkManager.Singleton.OnClientConnectedCallback += HandlePlayerConnect;
            GameManager.OnGamePhaseChange += HandleGamePhaseChange;
        }

        public void OnDisable()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandlePlayerConnect;
            GameManager.OnGamePhaseChange -= HandleGamePhaseChange;
        }

        public void Start()
        {
            HandleGamePhaseChange(this, new GamePhaseChange(GameManager.gamePhase, GameManager.gamePhase));
        }

        public void HandlePlayerConnect(ulong connectionId)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }
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
                StartCoroutine(SpawnPlayerCharacter(connectionId, spectatorPlayer, pos, rot));
            }
            else if (GameManager.gamePhase == GamePhase.Lobby)
            {
                StartCoroutine(SpawnPlayerCharacter(connectionId, lobbyPlayer));
            }
            else
            {
                // Default spawn a lobby player for the player character
                StartCoroutine(SpawnPlayerCharacter(connectionId, lobbyPlayer));
            }
        }

        public IEnumerator SpawnPlayerCharacter(ulong connectionId, GameObject playerPrefab)
        {
            return SpawnPlayerCharacter(connectionId, playerPrefab, Vector3.zero, Quaternion.identity);
        }

        public IEnumerator SpawnPlayerCharacter(ulong connectionId, GameObject playerPrefab, Vector3 pos, Quaternion rotation)
        {
            yield return null;
            NetworkObject oldPlayer =
                NetworkManager.Singleton.ConnectedClients[connectionId].PlayerObject;
            if (oldPlayer != null)
            {
                oldPlayer.Despawn(true);
            }
            GameObject newPlayer = NetworkManager.Instantiate(playerPrefab);
            newPlayer.transform.rotation = rotation;
            newPlayer.transform.position = pos;
            newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(connectionId);
        }

        public void HandleGamePhaseChange(object sender, GamePhaseChange change)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }
            // Handle whenever the game state changes
            switch (change.next)
            {
                // Do things differently based on the new phase
                case GamePhase.Lobby:
                    // Spawn a player for each active connection (lobby player)
                    foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                    {
                        StartCoroutine(SpawnPlayerCharacter(client.ClientId, lobbyPlayer));
                    }
                    break;
                case GamePhase.Setup:
                    // Clean up existing players
                    foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                    {
                        NetworkManager.Destroy(client.PlayerObject);
                    }
                    break;
                case GamePhase.InGame:
                    UnityEngine.Debug.Log("Spawning characters");
                    // When in game starts, spawn a player for each connection
                    ISpawnPointCollection defaultCheckpoint = SpawnPointUtilities.GetDefaultCheckpoint();
                    var spawnEnum = defaultCheckpoint != null ?
                        defaultCheckpoint.GetRandomizedSpawns().GetEnumerator() : null;
                    foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
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
                        StartCoroutine(SpawnPlayerCharacter(client.ClientId, inGamePlayer, pos, rot));
                    }
                    break;
                case GamePhase.Score:
                    break;
                case GamePhase.Reset:
                    // Destory spawned players for each connection
                    foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                    {
                        if (client.PlayerObject != null)
                        {
                            client.PlayerObject.Despawn(true);
                        }
                    }
                    // Once laoding is complete, go to lobby
                    break;
            }
        }
    }
}
