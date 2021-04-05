using System;
using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Prop;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.EditMode.Prop
{
    /// <summary>
    /// Tests to verify behaviour of PropMovement script
    /// </summary>
    [TestFixture]
    public class PropMovementTests
    {
        private PropMovement propMovement;

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
            characterGo.AddComponent<SphereCollider>();
            this.propMovement = characterGo.AddComponent<PropMovement>();
            this.propMovement.Start();
            this.unityServiceMock = new Mock<IUnityService>();
            this.networkServiceMock = new Mock<INetworkService>();
            this.colliderCastMock = new Mock<IColliderCast>();
            this.propMovement.unityService = this.unityServiceMock.Object;
            this.propMovement.networkService = this.networkServiceMock.Object;
            this.propMovement.colliderCast = this.colliderCastMock.Object;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.propMovement.gameObject);
        }

        [Test]
        public void TestWithRigidbody()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            this.propMovement.gameObject.AddComponent<Rigidbody>();
            this.propMovement.inputMovement = Vector3.one;
            this.propMovement.FixedUpdate();
            Assert.IsTrue(this.propMovement.transform.position == Vector3.zero);
        }

        [Test]
        public void TestNotLocal()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(false);
            this.propMovement.Update();
            this.propMovement.FixedUpdate();
        }

        [Test]
        public void TestDenyMovement()
        {
            PlayerInputManager.playerMovementState = PlayerInputState.Deny;
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            this.propMovement.inputMovement = Vector3.one;
            this.propMovement.FixedUpdate();
            Assert.IsTrue(this.propMovement.transform.position == Vector3.zero);
            PlayerInputManager.playerMovementState = PlayerInputState.Allow;
        }

        [Test]
        public void TestMapMovementFromInput()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            this.unityServiceMock.Setup(e => e.GetAxis("Horizontal")).Returns(1.0f);
            this.unityServiceMock.Setup(e => e.GetAxis("Vertical")).Returns(1.0f);
            this.propMovement.Update();
            Assert.IsTrue(this.propMovement.inputMovement.magnitude <= 1.0f);
            Assert.IsTrue(this.propMovement.inputMovement.x == this.propMovement.inputMovement.z);
            this.unityServiceMock.Setup(e => e.GetAxis("Horizontal")).Returns(1.0f);
            this.unityServiceMock.Setup(e => e.GetAxis("Vertical")).Returns(0.0f);
            this.propMovement.Update();
            Assert.IsTrue(this.propMovement.inputMovement.magnitude <= 1.0f);
            Assert.IsTrue(this.propMovement.inputMovement.x == 1.0f);
            Assert.IsTrue(this.propMovement.inputMovement.z == 0.0f);
        }

        [Test]
        public void TestJumpingVelocityChange()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            // When grounded
            this.unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
            this.colliderCastMock.Setup(e => e.CastSelf(It.IsAny<Vector3>(), It.IsAny<float>())).Returns(
                new ColliderCastHit
                {
                    hit = true,
                    distance = 0.001f,
                    normal = Vector3.up
                }
            );
            this.propMovement.FixedUpdate();
            Assert.IsTrue(this.propMovement.StandingOnGround);
            Assert.IsTrue(!this.propMovement.Falling);

            // Allow player to attempt to jump
            this.propMovement.attemptingJump = true;
            this.propMovement.FixedUpdate();

            // Assert that we are jumping
            Assert.IsTrue(Vector3.Project(this.propMovement.velocity, -this.propMovement.gravity).magnitude > 0);
        }

        [Test]
        public void TestFallingStateBasedOnCollision()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);

            // When grounded
            this.unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
            this.colliderCastMock.Setup(e => e.CastSelf(Vector3.down, this.propMovement.groundCheckDistance)).Returns(
                new ColliderCastHit
                {
                    hit = true,
                    distance = 0.001f,
                    normal = Vector3.up
                }
            );
            this.propMovement.FixedUpdate();
            Assert.IsTrue(this.propMovement.StandingOnGround);
            Assert.IsTrue(!this.propMovement.Falling);

            // When on slope
            this.unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
            this.colliderCastMock.Setup(e => e.CastSelf(Vector3.down, this.propMovement.groundCheckDistance)).Returns(
                new ColliderCastHit
                {
                    hit = true,
                    distance = 0.001f,
                    normal = Vector3.right
                }
            );
            this.propMovement.FixedUpdate();
            Assert.IsTrue(this.propMovement.StandingOnGround);
            Assert.IsTrue(this.propMovement.Falling);

            // When falling
            this.unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
            this.colliderCastMock.Setup(e => e.CastSelf(Vector3.down, this.propMovement.groundCheckDistance)).Returns(
                new ColliderCastHit
                {
                    hit = false
                }
            );
            this.propMovement.FixedUpdate();
            Assert.IsTrue(!this.propMovement.StandingOnGround);
            Assert.IsTrue(this.propMovement.Falling);
        }

        [Test]
        public void TestSnapDown()
        {
            this.colliderCastMock.Setup(e => e.CastSelf(It.IsAny<Vector3>(), It.IsAny<float>())).Returns(new ColliderCastHit
            {
                hit = true,
                distance = 1.0f
            });

            this.propMovement.SnapPlayerDown();
        }

        [Test]
        public void TestPushOverlapping()
        {
            GameObject overlap = new GameObject();
            Collider overlapCollider = overlap.AddComponent<BoxCollider>();

            unityServiceMock.Setup(e => e.deltaTime).Returns(0.001f);
            // Setup overlapping
            this.colliderCastMock.Setup(e => e.GetOverlappingDirectional()).Returns(new ColliderCastHit[]{
                new ColliderCastHit
                {
                    hit = true,
                    collider = overlapCollider
                }
            });

            // Test push overlapping
            this.propMovement.PushOutOverlapping();

            // Assert that there is some push
            Assert.IsTrue(this.propMovement.transform.position != Vector3.zero);

            // Test without collider
            GameObject.DestroyImmediate(this.propMovement.GetComponent<Collider>());
            this.propMovement.PushOutOverlapping();

            // Cleanup
            GameObject.DestroyImmediate(overlap);
        }

        [Test]
        public void TestPushAnotherObject()
        {
            var push = this.propMovement.gameObject.AddComponent<CharacterPush>();
            push.networkService = networkServiceMock.Object;

            GameObject otherObject = new GameObject();
            Rigidbody rigidbody = otherObject.AddComponent<Rigidbody>();
            BoxCollider collider = otherObject.AddComponent<BoxCollider>();

            rigidbody.isKinematic = false;

            this.propMovement.onGround = true;
            this.propMovement.distanceToGround = 0.01f;
            this.propMovement.angle = 0.0f;

            // Simulate hitting the object
            this.propMovement.maxBounces = 1;
            this.propMovement.verticalSnapUp = 1.0f;
            float hitHeight = (this.propMovement.transform.position - this.propMovement.GetComponent<Collider>().bounds.extents).y + 0.1f;
            this.colliderCastMock.Setup(e => e.CastSelf(It.IsAny<Vector3>(), It.IsAny<float>())).Returns(
                new ColliderCastHit
                {
                    hit = true,
                    distance = 1.0f,
                    collider = collider
                }
            );
            this.propMovement.MovePlayer(Vector3.forward);
        }

        [Test]
        public void TestSnappingUpWhileMoving()
        {
            this.propMovement.onGround = true;
            this.propMovement.distanceToGround = 0.01f;
            this.propMovement.angle = 0.0f;
            this.propMovement.stepUpDepth = 100.0f;

            // Simulate hitting a step
            this.propMovement.maxBounces = 1;
            this.propMovement.verticalSnapUp = 1.0f;
            float hitHeight = (this.propMovement.transform.position - this.propMovement.GetComponent<Collider>().bounds.extents).y + 0.1f;
            this.colliderCastMock.Setup(e => e.CastSelf(It.IsAny<Vector3>(), It.IsAny<float>())).Returns(
                new ColliderCastHit
                {
                    hit = true,
                    distance = 0.01f,
                    pointHit = Vector3.up * hitHeight
                }
            );
            this.propMovement.MovePlayer(Vector3.forward);
        }

        [Test]
        public void TestAttemptSnapUpSteps()
        {
            this.propMovement.onGround = true;
            this.propMovement.distanceToGround = 0.01f;
            this.propMovement.angle = 0.0f;

            // Setup the hit when hitting a step and not being able to move forward
            this.colliderCastMock.Setup(e => e.CastSelf(It.IsAny<Vector3>(), It.IsAny<float>())).Returns(
                new ColliderCastHit { hit = true, distance = 1.0f }
            );
            UnityEngine.Debug.Log(!this.propMovement.Falling + " ");
            this.propMovement.AttemptSnapUp(1.0f, new ColliderCastHit(), Vector3.forward);

            // Setup the event where they can walk forward
            this.colliderCastMock.Setup(e => e.CastSelf(It.IsAny<Vector3>(), It.IsAny<float>())).Returns(
                new ColliderCastHit { hit = false, distance = 0 }
            );
            this.propMovement.AttemptSnapUp(1.0f, new ColliderCastHit(), Vector3.forward);
        }
    }
}