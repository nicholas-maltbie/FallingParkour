using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.EditMode.Character
{
    /// <summary>
    /// Tests to verify behaviour of character movement script behaviour
    /// </summary>
    [TestFixture]
    public class CharacterMovementTests
    {
        /// <summary>
        /// Character movement component for this test
        /// </summary>
        CharacterMovement characterMovement;

        /// <summary>
        /// Character controller for character movement management
        /// </summary>
        CharacterController characterController;

        /// <summary>
        /// Camera transform for camera movement and changes
        /// </summary>
        Transform cameraTransform;

        /// <summary>
        /// Mock of network service attached to cameraFollow script
        /// </summary>
        Mock<INetworkService> networkServiceMock;

        /// <summary>
        /// Mock of unity service to mock inputs and delta time
        /// </summary>
        Mock<IUnityService> unityServiceMock;

        [SetUp]
        public void Setup()
        {
            // Setup character movement player
            GameObject characterGo = new GameObject();
            this.characterController = characterGo.AddComponent<CharacterController>();
            this.characterMovement = characterGo.AddComponent<CharacterMovement>();
            this.characterMovement.Start();
            this.unityServiceMock = new Mock<IUnityService>();
            this.networkServiceMock = new Mock<INetworkService>();
            this.characterMovement.unityService = this.unityServiceMock.Object;
            this.characterMovement.networkService = this.networkServiceMock.Object;

            // Setup camera transform
            GameObject cameraPosition = new GameObject();
            this.cameraTransform = cameraPosition.transform;
            this.characterMovement.cameraTransform = this.cameraTransform;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.characterMovement.gameObject);
            GameObject.DestroyImmediate(this.cameraTransform.gameObject);
        }

        [Test]
        public void CharacterMovementNotLocal()
        {
            this.characterMovement.Update();
        }

        [Test]
        public void CharacterMovementNotGrounded()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);

            // Do a test when the character is turning, moving forward, and jumping
            this.unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
            this.unityServiceMock.Setup(e => e.GetAxis("Horizontal")).Returns(-1.0f);
            this.unityServiceMock.Setup(e => e.GetAxis("Vertical")).Returns(-1.0f);
            this.unityServiceMock.Setup(e => e.GetAxis("Mouse X")).Returns(1.0f);
            this.unityServiceMock.Setup(e => e.GetAxis("Mouse Y")).Returns(-1.0f);
            this.characterMovement.Update();
        }

        [Test]
        public void CharacterMovementGrounded()
        {
            // Add some ground below the player
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            // Create a floor below the player
            GameObject floor = new GameObject();
            BoxCollider floorCollider = floor.AddComponent<BoxCollider>();
            floor.transform.transform.position = Vector3.zero;
            this.characterMovement.gameObject.transform.position = Vector3.zero;

            // Do a test when the character is turning, moving forward, and jumping
            this.unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
            this.unityServiceMock.Setup(e => e.GetAxis("Horizontal")).Returns(0.0f);
            this.unityServiceMock.Setup(e => e.GetAxis("Vertical")).Returns(0.0f);
            this.unityServiceMock.Setup(e => e.GetButton("Jump")).Returns(false);

            // Update character movement script
            this.characterMovement.Update();
            this.unityServiceMock.Setup(e => e.GetButton("Jump")).Returns(true);
            this.characterMovement.Update();
        }
    }
}