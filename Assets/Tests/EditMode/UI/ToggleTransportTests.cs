using NUnit.Framework;
using Moq;
using PropHunt.UI;
using PropHunt.Utils;
using UnityEngine;
using kcp2k;
using Mirror.FizzySteam;
using Mirror;
using UnityEngine.TestTools;
using System.Collections;

namespace Tests.EditMode.UI
{
    /// <summary>
    /// Tests the behaviour of the toggle transport for changing transport types
    /// </summary>
    [TestFixture]
    public class ToggleTransportTests
    {
        [Test]
        public void TestToggleTransportSettingsChanges()
        {
            GameObject uiElements = new GameObject();
            // Create game object to hold our toggle transporter
            GameObject go = new GameObject();
            ToggleTransport toggle = go.AddComponent<ToggleTransport>();
            // Set default mode to kcp
            toggle.currentMode = MultiplayerMode.KcpTransport;
            // Connect fake UI elements to toggle element
            toggle.controlButtons = uiElements;

            // Setup mocked data for toggle to do unit testing
            // Create a mock network service for passing network service data
            Mock<INetworkService> networkServiceMock = new Mock<INetworkService>();
            // attach the mock to our toggle object
            toggle.networkService = networkServiceMock.Object;

            // Setup values for KCP Transport and Fizzy Steamworks settings
            KcpTransport kcpTransport = go.AddComponent<KcpTransport>();
            FizzySteamworks fizzySteamworks = go.AddComponent<FizzySteamworks>();

            // Attach fake settings for transports to our toggle object
            toggle.kcpTransportSettings = kcpTransport;
            toggle.fizzySteamworksSettings = fizzySteamworks;

            // Test the start method for initial setup
            toggle.Start();

            // Assert that the current mode matches the selected transport
            Assert.IsTrue(Transport.activeTransport == toggle.transportSettingsLookup[toggle.currentMode]);

            // Assert that the mode does not change when we update the mode to the current mode
            toggle.SetMultiplayerMode(toggle.currentMode);
            Assert.IsTrue(Transport.activeTransport == toggle.transportSettingsLookup[toggle.currentMode]);

            // Assert that mode can change when we chant to a new mode
            toggle.SetMultiplayerMode(MultiplayerMode.FizzySteamworks);
            Assert.IsTrue(toggle.currentMode == MultiplayerMode.FizzySteamworks);
            Assert.IsTrue(Transport.activeTransport == toggle.fizzySteamworksSettings);

            // Set multiplayer mode to KCP from string command
            toggle.SetMultiplayerMode(MultiplayerMode.KcpTransport.ToString());
            Assert.IsTrue(toggle.currentMode == MultiplayerMode.KcpTransport);
            Assert.IsTrue(Transport.activeTransport == toggle.kcpTransportSettings);

            // Test to ensure buttons display or do not via update function with the network client
            // Ensure elements are not displayed when connected
            // Fake connecting to server
            networkServiceMock.Setup(nms => nms.activeNetworkClient).Returns(true);
            // Update then assert that elements are disabled
            toggle.Update();
            Assert.IsTrue(uiElements.activeSelf == false);
            // Fake not connecting to server
            networkServiceMock.Setup(nms => nms.activeNetworkClient).Returns(false);
            // Update then assert that elements are NOT disabled
            toggle.Update();
            Assert.IsTrue(uiElements.activeSelf == true);


            // Cleanup created game objects
            GameObject.DestroyImmediate(uiElements);
            GameObject.DestroyImmediate(go);
        }
    }
}