using System.Collections;
using Mirror;
using Mirror.Tests;
using NUnit.Framework;
using PropHunt.Game.Flow;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Game.Flow
{
    public class GameManagerTests : CustomNetworkManagerTestBase
    {
        GameManager gameManager;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            GameObject go = new GameObject();
            gameManager = go.AddComponent<GameManager>();
            gameManager.playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Tests/EditMode/TestPlayer.prefab");
            Assert.Throws<System.InvalidOperationException>(() => gameManager.Start());
        }

        [TearDown]
        public override void TearDown()
        {
            LogAssert.ignoreFailingMessages = true;
            base.TearDown();
            gameManager.OnDestory();
            GameObject.DestroyImmediate(gameManager.gameObject);
        }

        [Test]
        public void TestSetupBehaviour()
        {
            // Duplicate setup
            gameManager.Start();
        }

        [Test]
        public void HandlePlayerConnectInLobbyTest()
        {
            // Test connecting durring lobby phase
            gameManager.ChangePhase(GamePhase.Lobby);
            gameManager.HandlePlayerConnect(null, new PlayerConnectEvent(
                NetworkServer.connections[0]
            ));
        }

        [Test]
        public void HandlePlayerConnectInGameTest()
        {
            LogAssert.ignoreFailingMessages = true;
            // Evaluate connecting during in game phase
            gameManager.ChangePhase(GamePhase.InGame);
            gameManager.HandlePlayerConnect(null, new PlayerConnectEvent(
                NetworkServer.connections[0]
            ));
        }

        [Test]
        public void HandleVariousStateTransitions()
        {
            LogAssert.ignoreFailingMessages = true;

            gameManager.ChangePhase(GamePhase.Lobby);
            gameManager.Update();

            LogAssert.Expect(LogType.Error, "ServerChangeScene empty scene name");
            gameManager.ChangePhase(GamePhase.Setup);
            gameManager.Update();

            gameManager.ChangePhase(GamePhase.InGame);
            gameManager.Update();

            gameManager.ChangePhase(GamePhase.Score);
            gameManager.Update();

            LogAssert.Expect(LogType.Error, "ServerChangeScene empty scene name");
            gameManager.ChangePhase(GamePhase.Reset);
            gameManager.Update();
        }

        [Test]
        public void TestNonActiveServer()
        {
            NetworkServer.Shutdown();

            gameManager.ChangePhase(GamePhase.Lobby);
            gameManager.Update();
        }
    }
}