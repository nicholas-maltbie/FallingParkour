using Moq;
using NUnit.Framework;
using PropHunt.Animation;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Animation
{
    [TestFixture]
    public class PlayerLookTargetTests
    {
        [Test]
        public void Setup()
        {
            GameObject go = new GameObject();
            NetworkIKControl networkIKControl = go.AddComponent<NetworkIKControl>();
            CameraController cameraController = go.AddComponent<CameraController>();
            PlayerLookTarget playerLookTarget = go.AddComponent<PlayerLookTarget>();
            Mock<INetworkService> networkServiceMock = new Mock<INetworkService>();

            networkIKControl.Awake();
            playerLookTarget.Awake();
            playerLookTarget.Start();

            // Assert that camera hasn't been set yet when starting as client
            Assert.IsFalse(networkIKControl.lookState);
            Assert.IsTrue(networkIKControl.lookWeight == 0.0f);

            // Assert that cameras has been set when starting as server
            playerLookTarget.networkService = networkServiceMock.Object;
            networkServiceMock.Setup(e => e.isServer).Returns(true);
            playerLookTarget.Start();

            Assert.IsTrue(networkIKControl.lookState);
            Assert.IsTrue(networkIKControl.lookWeight == 1.0f);

            // Setup some networkIKControl info
            cameraController.cameraTransform = new GameObject().transform;

            // Test look target when is or is not local player
            networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            playerLookTarget.LateUpdate();
            networkServiceMock.Setup(e => e.isLocalPlayer).Returns(false);
            playerLookTarget.LateUpdate();

            // Cleanup objects
            GameObject.DestroyImmediate(go);
            GameObject.DestroyImmediate(cameraController.cameraTransform.gameObject);
        }
    }
}