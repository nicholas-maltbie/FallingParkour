using System.Text.RegularExpressions;
using Mirror;
using Mirror.Tests.RemoteAttrributeTest;
using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.EditMode.Character
{
    /// <summary>
    /// Tests to verify behaviour of the character push script
    /// </summary>
    [TestFixture]
    public class CharacterPushTests
    {
        /// <summary>
        /// How the character moved
        /// </summary>
        private CharacterMovement characterMovement;

        /// <summary>
        /// Character Push component for this test
        /// </summary>
        private CharacterPush characterPush;

        /// <summary>
        /// Character controller for character movement management
        /// </summary>
        private CharacterController characterController;

        /// <summary>
        /// Mock of network service attached to cameraFollow script
        /// </summary>
        private Mock<INetworkService> networkServiceMock;

        /// <summary>
        /// Mock of unity service to mock inputs and delta time
        /// </summary>
        private Mock<IUnityService> unityServiceMock;

        /// <summary>
        /// Network identity of the player object
        /// </summary>
        private NetworkIdentity networkIdentity;

        [SetUp]
        public void Setup()
        {
            // Setup character movement player
            GameObject characterGo = new GameObject();
            this.networkIdentity = characterGo.AddComponent<NetworkIdentity>();
            this.characterController = characterGo.AddComponent<CharacterController>();
            this.characterMovement = characterGo.AddComponent<CharacterMovement>();
            this.characterPush = characterGo.AddComponent<CharacterPush>();
            this.characterPush.Start();
            this.networkServiceMock = new Mock<INetworkService>();
            this.characterPush.networkService = this.networkServiceMock.Object;
            characterGo.transform.position = Vector3.zero;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.characterMovement.gameObject);
        }

        /// <summary>
        /// Test what happenes when the character hits something it can't push
        /// </summary>
        [Test]
        public void TestCharacterHitNonPushable()
        {
            // mock being the local player to allow for push events
            networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);

            GameObject hitObject = new GameObject();
            Rigidbody hitRigidbody = hitObject.AddComponent<Rigidbody>();
            hitRigidbody.isKinematic = true;

            // mock the collider hit event of an object without a rigidbody
            Mock<IControllerColliderHit> mockHitEvent = new Mock<IControllerColliderHit>();
            this.characterPush.PushObject(mockHitEvent.Object);

            // Mock the controller hit event of a kinematic object
            mockHitEvent.Setup(e => e.rigidbody).Returns(hitRigidbody);
            this.characterPush.PushObject(mockHitEvent.Object);

            // Cleanup created object
            GameObject.DestroyImmediate(hitObject);
        }

        /// <summary>
        /// Test what happens when an object without a rigidbody is attempted to be pushed
        /// </summary>
        [Test]
        public void TestPueshWithoutRigidbody()
        {
            // Push object without rigidbody
            GameObject hitObject = new GameObject();
            this.characterPush.PushWithForce(hitObject, Vector3.up, Vector3.zero);
            // Cleanup created object
            GameObject.DestroyImmediate(hitObject);
        }

        /// <summary>
        /// Test what happenes when the character pushes and is not local player
        /// </summary>
        [Test]
        public void TestCharacterHitPushableNotLocal()
        {
            // mock being the local player to allow for push events
            networkServiceMock.Setup(e => e.isLocalPlayer).Returns(false);

            GameObject hitObject = new GameObject();
            Rigidbody hitRigidbody = hitObject.AddComponent<Rigidbody>();
            hitRigidbody.isKinematic = false;

            // Mock the controller hit event of a kinematic object
            Mock<IControllerColliderHit> mockHitEvent = new Mock<IControllerColliderHit>();
            this.characterPush.PushObject(mockHitEvent.Object);

            // Cleanup created object
            GameObject.DestroyImmediate(hitObject);
        }

        /// <summary>
        /// Test what happenes when the character hits something it can push while on the client
        /// </summary>
        [Test]
        public void TestCharacterHitPushableOnClient()
        {
            // mock being the local player to allow for push events
            networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            networkServiceMock.Setup(e => e.isServer).Returns(false);

            GameObject hitObject = new GameObject();
            Rigidbody hitRigidbody = hitObject.AddComponent<Rigidbody>();
            hitRigidbody.isKinematic = false;

            // Mock the controller hit event of a kinematic object
            Mock<IControllerColliderHit> mockHitEvent = new Mock<IControllerColliderHit>();
            mockHitEvent.Setup(e => e.rigidbody).Returns(hitRigidbody);
            mockHitEvent.Setup(e => e.gameObject).Returns(hitObject);
            // Since we are on the client, we expect a server, this should throw an exception 
            //  since we are not connected to a server
            this.characterPush.PushObject(mockHitEvent.Object);
            UnityEngine.TestTools.LogAssert.Expect(LogType.Error, "Command Function CmdPushWithForce called without an active client.");

            // Cleanup created object
            GameObject.DestroyImmediate(hitObject);
        }

        /// <summary>
        /// Test what happenes when the character hits something it can push while on the server
        /// </summary>
        [Test]
        public void TestCharacterHitPushableOnServer()
        {
            // mock being the local player to allow for push events
            networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            networkServiceMock.Setup(e => e.isServer).Returns(true);

            GameObject hitObject = new GameObject();
            Rigidbody hitRigidbody = hitObject.AddComponent<Rigidbody>();
            hitRigidbody.isKinematic = false;

            // Mock the controller hit event of a kinematic object
            Mock<IControllerColliderHit> mockHitEvent = new Mock<IControllerColliderHit>();
            mockHitEvent.Setup(e => e.rigidbody).Returns(hitRigidbody);
            mockHitEvent.Setup(e => e.gameObject).Returns(hitObject);
            this.characterPush.PushObject(mockHitEvent.Object);

            // mock hitting an object we are standing on top of
            mockHitEvent.Setup(e => e.moveDirection).Returns(new Vector3(0, -1, 0));
            this.characterPush.PushObject(mockHitEvent.Object);

            // Cleanup created object
            GameObject.DestroyImmediate(hitObject);
        }
    }

    /// <summary>
    /// Tests to verify behaviour of commands in character push script
    /// </summary>
    public class CharacterPushCommandTests : RemoteTestBase
    {
        [Test]
        public void TestSenderConnectionIsSetWhenCommandReceived()
        {
            CharacterPush hostBehaviour = CreateHostObject<CharacterPush>(true);

            NetworkConnectionToClient connectionToClient = NetworkServer.connections[0];
            Debug.Assert(connectionToClient != null, $"connectionToClient was null, This means that the test is broken and will give the wrong results");

            GameObject pushed = new GameObject();
            pushed.AddComponent<Rigidbody>();
            pushed.AddComponent<NetworkIdentity>();
            NetworkServer.Spawn(pushed);
            hostBehaviour.CmdPushWithForce(pushed, Vector3.zero, Vector3.zero);
        }
    }
}