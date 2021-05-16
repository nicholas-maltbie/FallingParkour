using UnityEngine;
using Mirror;
using PropHunt.Game.Communication;
using System;
using PropHunt.Environment.Sound;
using UnityEngine.SceneManagement;
using System.Collections;
using PropHunt.Utils;

namespace PropHunt.Game.Flow
{
    public class GameSceneManager : NetworkBehaviour
    {
        [Header("Scene Management")]

        [Scene]
        public string lobbyScene;

        [Scene]
        public string gameScene;

        public INetworkService newtworkService;

        public void Start()
        {
            newtworkService = new NetworkService(this);
        }

        public override void OnStartServer()
        {
            GameManager.OnGamePhaseChange += HandleGamePhaseChange;
            GameManager.ChangePhase(GamePhase.Reset);
        }

        public override void OnStopServer()
        {
            GameManager.OnGamePhaseChange -= HandleGamePhaseChange;
            GameManager.ChangePhase(GamePhase.Disabled);
        }

        public void HandleGamePhaseChange(object sender, GamePhaseChange change)
        {
            // Handle whenever the game state changes
            switch (change.next)
            {
                // Do things differently based on the new phase
                case GamePhase.Lobby:
                    break;
                case GamePhase.Setup:
                    CustomNetworkManager.Instance.ServerChangeScene(gameScene);
                    // Once loading is complete, go to InGame
                    break;
                case GamePhase.InGame:
                    break;
                case GamePhase.Score:
                    break;
                case GamePhase.Reset:
                    // Start loading the lobby scene
                    CustomNetworkManager.Instance.ServerChangeScene(lobbyScene);
                    // Once laoding is complete, go to lobby
                    break;
            }
        }

        public void Update()
        {
            // Only run this on server
            if (!newtworkService.activeNetworkServer)
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
    }
}
