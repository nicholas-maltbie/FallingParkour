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
    public class CustomNetworkManagerTestBase
    {
        protected CustomNetworkManager networkManager;

        [SetUp]
        public virtual void SetUp()
        {
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
            GameObject go = new GameObject();
            Transport.activeTransport = go.AddComponent<MemoryTransport>();
            networkManager = go.AddComponent<CustomNetworkManager>();
            networkManager.playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Tests/EditMode/TestPlayer.prefabs");
            networkManager.Awake();

            networkManager.StartHost();

            NetworkClient.Ready();
        }

        [TearDown]
        public virtual void TearDown()
        {
            networkManager.StopHost();

            GameObject.DestroyImmediate(networkManager.playerPrefab);
            GameObject.DestroyImmediate(networkManager.gameObject);
        }
    }

    public class CustomNewtworkManagerFlowTest : CustomNetworkManagerTestBase
    {
        [Test]
        public void TestSingletonBehaviour()
        {
            LogAssert.ignoreFailingMessages = true;
            base.networkManager.Awake();
            base.networkManager.Awake();
        }

        [Test]
        public void TestHandleConnection()
        {
            int connects = 0;
            CustomNetworkManager.OnPlayerConnect += (object sender, PlayerConnectEvent connectEvent) => { connects++; };
            this.networkManager.OnServerReady(NetworkClient.connection);
            Assert.IsTrue(connects == 1);
        }

        [Test]
        public void TestStateChangeNotServer()
        {
            NetworkServer.Shutdown();
            base.networkManager.Update();
        }

        [Test]
        public void TestStateChangeAsServer()
        {
            base.networkManager.Update();
            LogAssert.Expect(LogType.Error, "ServerChangeScene empty scene name");
            GameManager.ChangePhase(GamePhase.Setup);
            while (GameManager.gamePhase == GamePhase.Setup)
            {
                base.networkManager.Update();
            }
            LogAssert.Expect(LogType.Error, "ServerChangeScene empty scene name");
            GameManager.ChangePhase(GamePhase.Reset);
            while (GameManager.gamePhase == GamePhase.Reset)
            {
                base.networkManager.Update();
            }
        }

        [Test]
        public void TestLoadScene()
        {
            LogAssert.Expect(LogType.Error, "ServerChangeScene empty scene name");
            this.networkManager.LoadLobbyScene();
            LogAssert.Expect(LogType.Error, "ServerChangeScene empty scene name");
            this.networkManager.LoadGameScene();
        }
    }
}