using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.EditMode.Character
{
    /// <summary>
    /// Tests to verify behaviour of KinematicCharacterController script
    /// </summary>
    [TestFixture]
    public class KinematicCharacterControllerTests
    {
        private KinematicCharacterController kcc;

        /// <summary>
        /// Mock of network service attached to cameraFollow script
        /// </summary>
        Mock<INetworkService> networkServiceMock;

        /// <summary>
        /// Mock of unity service to mock inputs and delta time
        /// </summary>
        Mock<IUnityService> unityServiceMock;

        /// <summary>
        /// Collider cast mock for mocking collisions
        /// </summary>
        Mock<IColliderCast> colliderCastMock;

        [SetUp]
        public void Setup()
        {
            // Setup character movement player
            GameObject characterGo = new GameObject();
            this.kcc = characterGo.AddComponent<KinematicCharacterController>();
            this.kcc.Start();
            this.unityServiceMock = new Mock<IUnityService>();
            this.networkServiceMock = new Mock<INetworkService>();
            this.colliderCastMock = new Mock<IColliderCast>();
            this.kcc.unityService = this.unityServiceMock.Object;
            this.kcc.networkService = this.networkServiceMock.Object;
            this.kcc.capsuleCollider = characterGo.AddComponent<CapsuleCollider>();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.kcc.gameObject);
        }

        [Test]
        public void TestWithRigidbody()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            this.kcc.gameObject.AddComponent<Rigidbody>();
            this.kcc.inputMovement = new Vector3(0, 0, 1.0f);
            this.kcc.Update();
            Assert.IsTrue(this.kcc.transform.position == Vector3.zero);
        }

        [Test]
        public void TestNotLocal()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(false);
            // this.kcc.Update();
            this.kcc.Update();
        }

        [Test]
        public void TestDenyMovement()
        {
            this.kcc.inputMovement = new Vector3(0, 0, 1.0f);
            PlayerInputManager.playerMovementState = PlayerInputState.Deny;
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            // this.kcc.Update();
            this.kcc.Update();

            Assert.IsTrue(this.kcc.transform.position == Vector3.zero);

            PlayerInputManager.playerMovementState = PlayerInputState.Allow;
            // this.kcc.Update();
            this.kcc.Update();

            Assert.IsTrue(this.kcc.transform.position == Vector3.zero);
        }

        [Test]
        public void TestMapMovementFromInput()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            // Simulate giving input movement
            this.kcc.OnMove(new UnityEngine.InputSystem.InputAction.CallbackContext());
            this.kcc.OnSprint(new UnityEngine.InputSystem.InputAction.CallbackContext());
            this.kcc.inputMovement = new Vector3(0.5f, 0, 0.5f);
            Assert.IsTrue(this.kcc.InputMovement.magnitude <= 1.0f);
            Assert.IsTrue(this.kcc.InputMovement.x == this.kcc.InputMovement.z);
            this.kcc.inputMovement = new Vector3(1.0f, 0, 0.0f);
            Assert.IsTrue(this.kcc.InputMovement.magnitude <= 1.0f);
            Assert.IsTrue(this.kcc.InputMovement.x == 1.0f);
            Assert.IsTrue(this.kcc.InputMovement.z == 0.0f);
        }

        [Test]
        public void TestJumpingVelocityChange()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            // When grounded
            this.unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
            this.unityServiceMock.Setup(e => e.fixedDeltaTime).Returns(1.0f);
            this.colliderCastMock.Setup(e => e.CastSelf(It.IsAny<Vector3>(), It.IsAny<float>())).Returns(
                new ColliderCastHit
                {
                    hit = true,
                    distance = 0.001f,
                    normal = Vector3.up
                }
            );
            this.kcc.Update();
            Assert.IsTrue(this.kcc.StandingOnGround);
            Assert.IsFalse(this.kcc.Falling);

            // Allow player to attempt to jump
            this.kcc.OnJump(new UnityEngine.InputSystem.InputAction.CallbackContext());
            this.kcc.attemptingJump = true;
            this.kcc.Update();

            // Assert that we are jumping
            UnityEngine.Debug.Log(this.kcc.Velocity);
            Assert.IsTrue(Vector3.Project(this.kcc.Velocity, -this.kcc.gravity).magnitude > 0);
        }

        [Test]
        public void TestFallingStateBasedOnCollision()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);

            // When grounded
            this.unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
            this.unityServiceMock.Setup(e => e.fixedDeltaTime).Returns(1.0f);
            this.colliderCastMock.Setup(e => e.CastSelf(Vector3.down, this.kcc.groundCheckDistance)).Returns(
                new ColliderCastHit
                {
                    hit = true,
                    distance = 0.001f,
                    normal = Vector3.up
                }
            );
            this.kcc.Update();
            Assert.IsTrue(this.kcc.StandingOnGround);
            Assert.IsTrue(!this.kcc.Falling);

            // When on slope
            this.unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
            this.unityServiceMock.Setup(e => e.fixedDeltaTime).Returns(1.0f);
            this.colliderCastMock.Setup(e => e.CastSelf(Vector3.down, this.kcc.groundCheckDistance)).Returns(
                new ColliderCastHit
                {
                    hit = true,
                    distance = 0.001f,
                    normal = Vector3.right
                }
            );
            this.kcc.Update();
            Assert.IsTrue(this.kcc.StandingOnGround);
            Assert.IsTrue(this.kcc.Falling);

            // When falling
            this.unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
            this.unityServiceMock.Setup(e => e.fixedDeltaTime).Returns(1.0f);
            this.colliderCastMock.Setup(e => e.CastSelf(Vector3.down, this.kcc.groundCheckDistance)).Returns(
                new ColliderCastHit
                {
                    hit = false
                }
            );
            this.kcc.Update();
            Assert.IsTrue(!this.kcc.StandingOnGround);
            Assert.IsTrue(this.kcc.Falling);
        }

        [Test]
        public void TestSnapDown()
        {
            this.colliderCastMock.Setup(e => e.CastSelf(It.IsAny<Vector3>(), It.IsAny<float>())).Returns(new ColliderCastHit
            {
                hit = true,
                distance = 1.0f
            });

            this.kcc.SnapPlayerDown();
        }

        [Test]
        public void TestPushOverlapping()
        {
            GameObject overlap = new GameObject();
            Collider overlapCollider = overlap.AddComponent<BoxCollider>();

            unityServiceMock.Setup(e => e.deltaTime).Returns(0.001f);
            unityServiceMock.Setup(e => e.fixedDeltaTime).Returns(0.001f);
            // Setup overlapping
            this.colliderCastMock.Setup(e => e.GetOverlappingDirectional()).Returns(new ColliderCastHit[]{
                new ColliderCastHit
                {
                    hit = true,
                    collider = overlapCollider
                }
            });

            // Test push overlapping
            this.kcc.PushOutOverlapping();

            // Assert that there is some push
            Assert.IsTrue(this.kcc.transform.position != Vector3.zero);

            // Test without collider
            GameObject.DestroyImmediate(this.kcc.GetComponent<Collider>());
            this.kcc.PushOutOverlapping();

            // Cleanup
            GameObject.DestroyImmediate(overlap);
        }

        [Test]
        public void TestPushAnotherObject()
        {
            var push = this.kcc.gameObject.AddComponent<CharacterPush>();
            push.networkService = networkServiceMock.Object;

            GameObject otherObject = new GameObject();
            Rigidbody rigidbody = otherObject.AddComponent<Rigidbody>();
            BoxCollider collider = otherObject.AddComponent<BoxCollider>();

            rigidbody.isKinematic = false;

            this.kcc.onGround = true;
            this.kcc.distanceToGround = 0.01f;
            this.kcc.angle = 0.0f;

            // Simulate hitting the object
            this.kcc.maxBounces = 1;
            this.kcc.verticalSnapUp = 1.0f;
            float hitHeight = (this.kcc.transform.position - this.kcc.GetComponent<Collider>().bounds.extents).y + 0.1f;
            this.colliderCastMock.Setup(e => e.CastSelf(It.IsAny<Vector3>(), It.IsAny<float>())).Returns(
                new ColliderCastHit
                {
                    hit = true,
                    distance = 1.0f,
                    collider = collider
                }
            );
            this.kcc.MovePlayer(Vector3.forward);
        }

        [Test]
        public void TestSnappingUpWhileMoving()
        {
            this.kcc.onGround = true;
            this.kcc.distanceToGround = 0.01f;
            this.kcc.angle = 0.0f;
            this.kcc.stepUpDepth = 100.0f;

            // Simulate hitting a step
            this.kcc.maxBounces = 1;
            this.kcc.verticalSnapUp = 1.0f;
            float hitHeight = (this.kcc.transform.position - this.kcc.GetComponent<Collider>().bounds.extents).y + 0.1f;
            this.colliderCastMock.Setup(e => e.CastSelf(It.IsAny<Vector3>(), It.IsAny<float>())).Returns(
                new ColliderCastHit
                {
                    hit = true,
                    distance = 0.01f,
                    pointHit = Vector3.up * hitHeight
                }
            );
            this.kcc.MovePlayer(Vector3.forward);
        }

        [Test]
        public void TestAttemptSnapUpSteps()
        {
            this.kcc.onGround = true;
            this.kcc.distanceToGround = 0.01f;
            this.kcc.angle = 0.0f;

            // Setup the hit when hitting a step and not being able to move forward
            this.colliderCastMock.Setup(e => e.CastSelf(It.IsAny<Vector3>(), It.IsAny<float>())).Returns(
                new ColliderCastHit { hit = true, distance = 1.0f }
            );
            UnityEngine.Debug.Log(!this.kcc.Falling + " ");
            this.kcc.AttemptSnapUp(1.0f, new RaycastHit(), Vector3.forward);

            // Setup the event where they can walk forward
            this.colliderCastMock.Setup(e => e.CastSelf(It.IsAny<Vector3>(), It.IsAny<float>())).Returns(
                new ColliderCastHit { hit = false, distance = 0 }
            );
            this.kcc.AttemptSnapUp(1.0f, new RaycastHit(), Vector3.forward);
        }
    }
}