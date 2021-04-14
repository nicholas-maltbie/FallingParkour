using Mirror;
using Mirror.Tests;
using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Character
{
    [TestFixture]
    public class ChangePlayerTeamTests
    {
        GameObject playerPrefab;

        PlayerTeam playerObj;

        ChangePlayerTeam changeTeam;

        NetworkManager manager;

        [SetUp]
        public void Setup()
        {
#if UNITY_EDITOR
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
#endif
            GameObject managerGo = new GameObject();
            Transport.activeTransport = managerGo.AddComponent<MemoryTransport>();
            manager = managerGo.AddComponent<NetworkManager>();
            manager.StartHost();

            GameObject go = new GameObject();
            GameObject playerGo = new GameObject();
            NetworkIdentity identity = playerGo.AddComponent<NetworkIdentity>();
            this.playerObj = playerGo.AddComponent<PlayerTeam>();
            NetworkServer.AddPlayerForConnection(NetworkServer.connections[0], playerObj.gameObject, identity.assetId);

            this.playerPrefab = new GameObject();
            this.playerPrefab.AddComponent<NetworkIdentity>();
            this.playerPrefab.AddComponent<PlayerTeam>().playerTeam = Team.Hunter;
            this.changeTeam = go.AddComponent<ChangePlayerTeam>();
            this.changeTeam.setTeam = Team.Prop;
            this.changeTeam.newPrefab = playerPrefab;
        }

        [TearDown]
        public void TearDown()
        {
            LogAssert.ignoreFailingMessages = true;
            manager.StopHost();
            // Destroy the game objects we created
            GameObject.DestroyImmediate(playerObj.gameObject);
            GameObject.DestroyImmediate(changeTeam.gameObject);
            GameObject.DestroyImmediate(manager.gameObject);
            GameObject.DestroyImmediate(playerPrefab);
        }

        [Test]
        public void TestPlayerChangeTeams()
        {
            LogAssert.ignoreFailingMessages = true;
            this.changeTeam.Start();
            Mock<INetworkService> networkServiceMock = new Mock<INetworkService>();
            this.changeTeam.networkService = networkServiceMock.Object;
            networkServiceMock.Setup(e => e.isServer).Returns(true);
            networkServiceMock.Setup(e => e.isClient).Returns(true);
            LogAssert.Expect(LogType.Error, "Can not Register 'New Game Object' because it had empty assetid. If this is a scene Object use RegisterSpawnHandler instead");
            this.changeTeam.OnTriggerEnter(this.playerObj.gameObject.AddComponent<BoxCollider>());
        }
    }
}