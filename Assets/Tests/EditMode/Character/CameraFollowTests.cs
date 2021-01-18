using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.Character
{
    /// <summary>
    /// Tests to verify behaviour of camera follower script behaviour
    /// </summary>
    [TestFixture]
    public class CameraFollowTests
    {
        /// <summary>
        /// Camera follower for this test
        /// </summary>
        CameraFollow cameraFollow;

        /// <summary>
        /// Mock of network service attached to cameraFollow script
        /// </summary>
        Mock<INetworkService> networkServiceMock;

        /// <summary>
        /// Transform to teleport camera to
        /// </summary>
        Transform cameraTransformTarget;

        /// <summary>
        /// Main camera for the scene
        /// </summary>
        Camera mainCamera;

        [SetUp]
        public void Setup()
        {
            // Create a game object and setup camera follow component
            GameObject go = new GameObject();
            this.cameraFollow = go.AddComponent<CameraFollow>();
            this.cameraFollow.Start();
            this.networkServiceMock = new Mock<INetworkService>();
            this.cameraFollow.networkService = this.networkServiceMock.Object;

            // Cleanup main camera if one exists already
            if (Camera.main != null)
            {
                GameObject.DestroyImmediate(Camera.main.gameObject);
            }
            // Setup main camera
            GameObject cameraGo = new GameObject();
            this.mainCamera = cameraGo.AddComponent<Camera>();
            this.mainCamera.tag = "MainCamera";
            cameraGo.transform.position = Vector3.zero;
            cameraGo.transform.rotation = Quaternion.identity;

            // Setup camera transform target
            GameObject cameraTargetGo = new GameObject();
            cameraTargetGo.transform.position = new Vector3(0, 10, 0);
            cameraTargetGo.transform.rotation = Quaternion.Euler(0, 90, 0);
            this.cameraTransformTarget = cameraTargetGo.transform;
            this.cameraFollow.cameraTransform = cameraTransformTarget;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.cameraFollow.gameObject);
            if (this.mainCamera != null)
            {
                GameObject.DestroyImmediate(this.mainCamera.gameObject);
            }
            GameObject.DestroyImmediate(this.cameraTransformTarget.gameObject);
        }

        [Test]
        public void TestCameraFollowNotLocal()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(false);
            this.cameraFollow.Update();
        }

        [Test]
        public void TestNoMainCamera()
        {
            GameObject.DestroyImmediate(this.mainCamera.gameObject);
            this.mainCamera = null;
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            this.cameraFollow.Update();
        }

        [Test]
        public void TestFollowCharacter()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            this.cameraFollow.Update();
            Assert.IsTrue(this.mainCamera.transform.position == cameraTransformTarget.transform.position);
            Assert.IsTrue(this.mainCamera.transform.rotation.eulerAngles == cameraTransformTarget.transform.rotation.eulerAngles);
        }
    }
}