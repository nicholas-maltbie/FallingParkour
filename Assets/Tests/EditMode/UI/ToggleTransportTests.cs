using NUnit.Framework;
using Moq;
using PropHunt.UI;
using PropHunt.Utils;
using UnityEngine;
using kcp2k;
using Mirror.FizzySteam;
using Mirror;

namespace Tests.EditMode.UI
{
    /// <summary>
    /// Tests the behaviour of the network action for manipulating the network manager
    /// </summary>
    [TestFixture]
    public class ToggleTransportTests
    {
        [Test]
        public void TestToggleTransportSettingsChanges()
        {
#if UNITY_EDITOR
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
#endif
            // Create game object to hold our toggle transporter
            GameObject go = new GameObject();
            ToggleTransport toggle = go.AddComponent<ToggleTransport>();
            // Set default mode to kcp
            toggle.currentMode = MultiplayerMode.KcpTransport;

            // Setup mocked data for toggle to do unit testing
            // Create a mock network service for passing network service data
            Mock<INetworkService> networkServiceMock = new Mock<INetworkService>();
            // attach the mock to our toggle object
            toggle.networkService = networkServiceMock.Object;

            // Setup values for KCP Transport and Fizzy Steamworks settings
            KcpTransport kcpTransport = go.AddComponent<KcpTransport>();
            FizzySteamworks fizzySteamworks = go.AddComponent<FizzySteamworks>();

            // Attach fake settings for transports to our toggle object
            toggle.kcpTransportPrefab = kcpTransport.gameObject;
            toggle.fizzySteamworksPrefab = fizzySteamworks.gameObject;

            // Test the start method for initial setup
            toggle.Start();
            // Test to ensure setup doesn't duplicate and cause errors
            toggle.Start();

            // Assert that the current mode matches the selected transport
            Assert.IsTrue(Transport.activeTransport.GetComponent<KcpTransport>() != null);

            // Assert that the mode does not change when we update the mode to the current mode
            toggle.SetMultiplayerMode(toggle.currentMode);
            Assert.IsTrue(Transport.activeTransport.GetComponent<KcpTransport>() != null);

            // Assert that mode can change when we chant to a new mode
            toggle.SetMultiplayerMode(MultiplayerMode.FizzySteamworks);
            Assert.IsTrue(toggle.currentMode == MultiplayerMode.FizzySteamworks);
            Assert.IsTrue(Transport.activeTransport.GetComponent<FizzySteamworks>() != null);

            // Set multiplayer mode to KCP from string command
            toggle.SetMultiplayerMode(MultiplayerMode.KcpTransport, true);
            Assert.IsTrue(toggle.currentMode == MultiplayerMode.KcpTransport);
            Assert.IsTrue(Transport.activeTransport.GetComponent<KcpTransport>() != null);

            // Cleanup created game objects
            GameObject.DestroyImmediate(go);
        }
    }
}