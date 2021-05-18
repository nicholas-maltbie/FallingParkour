using Mirror;
using Mirror.Tests;
using Mirror.Tests.RemoteAttributeTest;
using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Utils;
using Tests.EditMode.Game.Flow;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Character
{
    [TestFixture]
    public class ChangePlayerTeamTests : RemoteTestBase
    {
        GameObject playerPrefab;

        PlayerTeam playerObj;

        ChangePlayerTeam changeTeam;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            GameObject go = new GameObject();
            GameObject playerGo = new GameObject();
            NetworkIdentity identity = playerGo.AddComponent<NetworkIdentity>();
            this.playerObj = playerGo.AddComponent<PlayerTeam>();
            NetworkServer.AddPlayerForConnection(NetworkServer.connections[0], playerObj.gameObject, identity.assetId);

            this.playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Tests/EditMode/TestPlayer.prefab");
            this.changeTeam = go.AddComponent<ChangePlayerTeam>();
            this.changeTeam.setTeam = Team.Prop;
            this.changeTeam.newPrefab = this.playerPrefab;
            this.changeTeam.Awake();
            LogAssert.Expect(LogType.Error, "Can not Register 'New Game Object' because it had empty assetid. If this is a scene Object use RegisterSpawnHandler instead");
            this.changeTeam.OnStartClient();
        }

        [TearDown]
        public override void TearDown()
        {
            LogAssert.ignoreFailingMessages = true;
            base.TearDown();

            // Destroy the game objects we created
            GameObject.DestroyImmediate(playerObj.gameObject);
            GameObject.DestroyImmediate(changeTeam.gameObject);
            GameObject.DestroyImmediate(playerPrefab);
        }

        [Test]
        public void TestNotServer()
        {
            LogAssert.ignoreFailingMessages = true;
            Mock<INetworkService> networkServiceMock = new Mock<INetworkService>();
            this.changeTeam.networkService = networkServiceMock.Object;
            networkServiceMock.Setup(e => e.isServer).Returns(false);
            networkServiceMock.Setup(e => e.isClient).Returns(true);
            this.changeTeam.OnTriggerEnter(this.playerObj.gameObject.AddComponent<BoxCollider>());
        }

        [Test]
        public void TestPlayerChangeTeams()
        {
            LogAssert.ignoreFailingMessages = true;
            Mock<INetworkService> networkServiceMock = new Mock<INetworkService>();
            this.changeTeam.networkService = networkServiceMock.Object;
            networkServiceMock.Setup(e => e.isServer).Returns(true);
            networkServiceMock.Setup(e => e.isClient).Returns(true);
            this.changeTeam.OnTriggerEnter(this.playerObj.gameObject.AddComponent<BoxCollider>());
        }
    }
}