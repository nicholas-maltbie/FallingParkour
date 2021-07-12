using Mirror;
using Mirror.Tests;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Game.Flow;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Game.Flow
{
    public class CustomNetworkManagerTestBase
    {
        protected CustomNetworkManager networkManager;

        protected bool reloadScene = true;

        [SetUp]
        public virtual void SetUp()
        {
#if UNITY_EDITOR
            if (reloadScene)
            {
                var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
                    UnityEditor.SceneManagement.NewSceneMode.Single);
            }
#endif
            GameObject go = new GameObject();
            Transport.activeTransport = go.AddComponent<MemoryTransport>();
            networkManager = go.AddComponent<CustomNetworkManager>();
            networkManager.autoCreatePlayer = false;
            networkManager.playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Tests/EditMode/TestPlayer.prefabs");
            networkManager.timerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Networking/Timer/GameTimer.prefab").GetComponent<GameTimer>();
            networkManager.Awake();

            networkManager.StartHost();

            if (!NetworkClient.ready)
            {
                NetworkClient.Ready();
            }
        }

        [TearDown]
        public virtual void TearDown()
        {
            networkManager.StopHost();
            networkManager.OnDestory();

            // Cleanup created prefabs
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
            this.networkManager.Awake();
        }

        [Test]
        public void TestEventFlow()
        {
            this.networkManager.OnStartClient();
            this.networkManager.OnStartServer();
            this.networkManager.OnStopClient();
            this.networkManager.OnStopServer();
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
        public void TestCustomJoinMessage()
        {
            NetworkConnection playerConn = NetworkServer.connections[0];
            var connectHandler = this.networkManager.SendJoinMessage(playerConn);
            // Try sending a custom join message
            CharacterName.OnPlayerNameChange?.Invoke(this, new PlayerNameChange("previous", "new", playerConn.connectionId));
            // Finish timeout
            connectHandler.MoveNext();
        }

        [Test]
        public void TestCustomJoinMessageTimeout()
        {
            NetworkConnection playerConn = NetworkServer.connections[0];
            var connectHandler = this.networkManager.SendJoinMessage(playerConn);
            // Do NOT send a custom join message of name change
            // Finish timeout
            while (connectHandler.MoveNext()) ;
        }
    }
}