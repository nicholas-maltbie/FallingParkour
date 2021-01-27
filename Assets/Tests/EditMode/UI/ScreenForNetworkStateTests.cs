using Moq;
using NUnit.Framework;
using PropHunt.UI;
using PropHunt.Utils;
using System.Collections.Generic;
using UnityEngine;
using static PropHunt.UI.MenuController;

namespace Tests.EditMode.UI
{
    /// <summary>
    /// Tests for ScreenForNetworkState object to switch screens based on the current network state
    /// </summary>
    [TestFixture]
    public class ScreenForNetworkStateTests
    {
        private ScreenForNetworkState stateController;

        private Mock<INetworkService> networkServiceMock;

        private List<GameObject> screenObjects = new List<GameObject>();

        private string currentScreen;

        [SetUp]
        public void Setup()
        {
            UIManager.RequestScreenChange += (object source, RequestScreenChangeEventArgs args) =>
            {
                this.currentScreen = args.newScreen;
            };

            GameObject go = new GameObject();
            this.stateController = go.AddComponent<ScreenForNetworkState>();
            this.networkServiceMock = new Mock<INetworkService>();
            this.stateController.networkService = networkServiceMock.Object;

            // setup various network screens 
            this.stateController.offlineScreen = new GameObject();
            this.stateController.connectingScreen = new GameObject();
            this.stateController.onlineScreen = new GameObject();
            this.stateController.serverScreen = new GameObject();

            this.stateController.offlineScreen.name = "offline";
            this.stateController.connectingScreen.name = "connecting";
            this.stateController.onlineScreen.name = "online";
            this.stateController.serverScreen.name = "server";

            this.screenObjects.Add(this.stateController.offlineScreen);
            this.screenObjects.Add(this.stateController.connectingScreen);
            this.screenObjects.Add(this.stateController.onlineScreen);
            this.screenObjects.Add(this.stateController.serverScreen);
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(stateController.gameObject);
            foreach (GameObject screen in screenObjects)
            {
                GameObject.DestroyImmediate(screen);
            }
            screenObjects.Clear();
        }

        [Test]
        public void VerifyLoadProperScreens()
        {
            // Start offline
            networkServiceMock.Setup(e => e.activeNetworkClient).Returns(false);
            networkServiceMock.Setup(e => e.isConnectedNetworkClient).Returns(false);
            networkServiceMock.Setup(e => e.activeNetworkServer).Returns(false);
            this.stateController.Update();
            // Assert that offline screen is loaded
            Assert.IsTrue(currentScreen == this.stateController.offlineScreen.name);
            // Change to connecting
            networkServiceMock.Setup(e => e.activeNetworkClient).Returns(true);
            networkServiceMock.Setup(e => e.isConnectedNetworkClient).Returns(false);
            networkServiceMock.Setup(e => e.activeNetworkServer).Returns(false);
            this.stateController.Update();
            // Assert that connecting screen is loaded
            Assert.IsTrue(currentScreen == this.stateController.connectingScreen.name);
            // Change to online
            networkServiceMock.Setup(e => e.activeNetworkClient).Returns(true);
            networkServiceMock.Setup(e => e.isConnectedNetworkClient).Returns(true);
            networkServiceMock.Setup(e => e.activeNetworkServer).Returns(false);
            this.stateController.Update();
            // Assert that online screen is loaded
            Assert.IsTrue(currentScreen == this.stateController.onlineScreen.name);
            // Change to server only
            networkServiceMock.Setup(e => e.activeNetworkClient).Returns(false);
            networkServiceMock.Setup(e => e.isConnectedNetworkClient).Returns(false);
            networkServiceMock.Setup(e => e.activeNetworkServer).Returns(true);
            this.stateController.Update();
            // Assert that server only screen is loaded
            Assert.IsTrue(currentScreen == this.stateController.serverScreen.name);

        }
    }
}