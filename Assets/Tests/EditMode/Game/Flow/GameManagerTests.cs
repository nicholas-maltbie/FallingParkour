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
    [TestFixture]
    public class GameManagerTests : CustomNetworkManagerTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            GameManager.playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Tests/EditMode/TestPlayer.prefab");
        }

        [TearDown]
        public override void TearDown()
        {
            LogAssert.ignoreFailingMessages = true;
            base.TearDown();
            GameManager.playerPrefab = null;
        }

        [Test]
        public void HandleVariousPhaseChanges()
        {
            GameManager.HandleGamePhaseChange(this, new GamePhaseChange(GamePhase.Lobby, GamePhase.Lobby));
            LogAssert.Expect(LogType.Error, "ServerChangeScene empty scene name");
            GameManager.HandleGamePhaseChange(this, new GamePhaseChange(GamePhase.Lobby, GamePhase.Setup));
            GameManager.HandleGamePhaseChange(this, new GamePhaseChange(GamePhase.Lobby, GamePhase.InGame));
            GameManager.HandleGamePhaseChange(this, new GamePhaseChange(GamePhase.Lobby, GamePhase.Score));
            LogAssert.Expect(LogType.Error, "ServerChangeScene empty scene name");
            GameManager.HandleGamePhaseChange(this, new GamePhaseChange(GamePhase.Lobby, GamePhase.Reset));
        }

        [Test]
        public void SpawnCharacterOnJoin()
        {
            GameManager.ChangePhase(GamePhase.InGame);
            GameManager.HandlePlayerConnect(this, new PlayerConnectEvent(NetworkServer.connections[0]));
        }

        [Test]
        public void HandleSpawningCharacter()
        {
            NetworkConnection conn = NetworkServer.connections[0];
            IEnumerator spawnCharacter = GameManager.SpawnPlayerCharacter(conn);
            conn.isReady = false;
            int i = 0;
            while (spawnCharacter.MoveNext()) { i++; conn.isReady = i > 10; }
        }
    }
}