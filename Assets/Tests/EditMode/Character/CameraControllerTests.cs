using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.EditMode.Character
{
    /// <summary>
    /// Tests to verify behaviour of camera controller script behaviour
    /// </summary>
    [TestFixture]
    public class CameraControllerTests
    {
        CameraController cameraController;
        Mock<INetworkService> networkServiceMock;

        [SetUp]
        public void Setup()
        {
            // Create a game object and setup camera follow component
            GameObject go = new GameObject();
            this.cameraController = go.AddComponent<CameraController>();
            this.cameraController.Start();
            this.networkServiceMock = new Mock<INetworkService>();
            this.cameraController.networkService = this.networkServiceMock.Object;
            this.cameraController.cameraTransform = go.transform;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.cameraController.gameObject);
        }

        [Test]
        public void TestCameraFollowNotLocal()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(false);
            this.cameraController.Update();
        }

        [Test]
        public void TestCameraFollowLocal()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            this.cameraController.Update();
        }
    }
}