using Moq;
using NUnit.Framework;
using PropHunt.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;
using Mirror;
using PropHunt.Utils;
using Tests.Common.Utils;
using Mirror.Tests;
using UnityEngine.UI;
using PropHunt.Character;

namespace Tests.EditMode.UI
{
    /// <summary>
    /// Tests for various NetworkActionsController in the UI to manage and manipulate the network manager from
    /// the screen with buttons
    /// </summary>
    [TestFixture]
    public class NetworkActionsTests
    {
        private NetworkActions networkActions;

        private Mock<INetworkService> networkServiceMock;

        [UnitySetUp]
        public IEnumerator Setup()
        {
#if UNITY_EDITOR
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
#endif
            // Setup defalt player name
            CharacterNameManagement.playerName = "Player";

            GameObject networkActionsGo = new GameObject();
            this.networkActions = networkActionsGo.AddComponent<NetworkActions>();
            // Setup connect address
            this.networkActions.connectAddress = networkActionsGo.AddComponent<InputField>();
            this.networkActions.connectAddress.text = "localhost";

            // Setup network services mock
            this.networkServiceMock = new Mock<INetworkService>();
            this.networkActions.networkService = this.networkServiceMock.Object;

            // setup transport with default kcp transport
            Transport.activeTransport = networkActionsGo.AddComponent<MemoryTransport>();
            // Do initial setup of network manager
            networkActionsGo.AddComponent<NetworkManager>();
            yield return null;

            // Setup network manager
            this.networkActions.Update();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            GameObject.DestroyImmediate(this.networkActions.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator StartActionsWithInvalidName()
        {
            CharacterNameManagement.playerName = "";
            this.networkActions.StartClient();
            yield return new WaitClientActiveState(true, newTimeout: 1);
            Assert.IsFalse(NetworkClient.active);

            this.networkActions.StartHost();
            yield return new WaitClientActiveState(true, newTimeout: 1);
            Assert.IsFalse(NetworkClient.active);
        }

        [UnityTest]
        public IEnumerator StartStopClientTest()
        {
            this.networkActions.StartClient();
            yield return new WaitClientActiveState(true);
            Assert.IsTrue(NetworkClient.active);
            networkServiceMock.Setup(e => e.activeNetworkClient).Returns(true);
            // This should eventually time out so just stop it
            this.networkActions.StopClientConnecting();
            yield return new WaitForConnected(false);
            Assert.IsFalse(NetworkClient.active);
        }

        [UnityTest]
        public IEnumerator StopConnectedClient()
        {
            this.networkActions.StartClient();
            yield return new WaitClientActiveState(true);
            Assert.IsTrue(NetworkClient.active);
            networkServiceMock.Setup(e => e.activeNetworkClient).Returns(false);
            networkServiceMock.Setup(e => e.isConnectedNetworkClient).Returns(true);
            networkServiceMock.Setup(e => e.activeNetworkServer).Returns(false);
            // This should eventually time out... but let's pretend that we connected and just stop it the same way
            this.networkActions.StopClient();
            yield return new WaitClientActiveState(false);
            Assert.IsFalse(NetworkClient.active);
        }

        [UnityTest]
        public IEnumerator StartStopHostTest()
        {
            this.networkActions.StartHost();
            // Wait for connected
            yield return new WaitForConnected();
            // Assert that we are now connected
            Assert.IsTrue(NetworkClient.isConnected);
            Assert.IsTrue(NetworkClient.active);
            networkServiceMock.Setup(e => e.activeNetworkClient).Returns(true);
            networkServiceMock.Setup(e => e.isConnectedNetworkClient).Returns(true);
            networkServiceMock.Setup(e => e.activeNetworkServer).Returns(true);
            // Wait until connected then stop host
            this.networkActions.StopClient();
            // Wait for disconnected
            yield return new WaitForConnected(false);
            Assert.IsFalse(NetworkClient.isConnected);
            Assert.IsFalse(NetworkClient.active);
        }

        [UnityTest]
        public IEnumerator StartStopServer()
        {
            this.networkActions.StartServer();
            // Wait for network server to start up
            yield return new WaitForServerActiveState();
            // Assert that we are now active network server
            Assert.IsTrue(NetworkServer.active);
            networkServiceMock.Setup(e => e.activeNetworkClient).Returns(false);
            networkServiceMock.Setup(e => e.isConnectedNetworkClient).Returns(false);
            networkServiceMock.Setup(e => e.activeNetworkServer).Returns(true);
            // Wait until connected then stop server
            this.networkActions.StopClient();
            // Wait for network server to shut down
            yield return new WaitForServerActiveState(false);
            Assert.IsFalse(NetworkServer.active);
        }
    }
}