
using System.Collections;
using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Utils;
using Tests.Common.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayMode.Character
{
    /// <summary>
    /// Tests designed to evaluate the Kinematic Character Controller
    /// </summary>
    public class KinematicCharacterControllerTestBase
    {
        /// <summary>
        /// Network service mock attached to the character
        /// </summary>
        protected Mock<INetworkService> networkServiceMock;

        /// <summary>
        /// Mock for unity service attached to the character
        /// </summary>
        protected Mock<IUnityService> unityServiceMock;

        /// <summary>
        /// Character controller for controlling player movement
        /// </summary>
        protected KinematicCharacterController kcc;

        /// <summary>
        /// Basic floor object, should be centered at 0,0,0 and has a width and length of 10 units
        /// and a height of 1 unit.
        /// </summary>
        protected GameObject floor;

        /// <summary>
        /// Player object with a kinematic character controller
        /// </summary>
        protected GameObject player;

        [UnitySetUp]
        public virtual IEnumerator SetUp()
        {
            SceneManager.LoadScene("EmptyScene", LoadSceneMode.Single);
            yield return new WaitForSceneLoaded("EmptyScene", newTimeout: 10);
            // Start off by creating a basic platform
            this.floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // Center the below zero
            this.floor.transform.position = new Vector3(0, -0.5f, 0);
            // Set the size of the box to be 10x10 width and length and 1 unit tall
            this.floor.transform.localScale = new Vector3(10, 1, 10);
            BoxCollider floorCollider = this.floor.AddComponent<BoxCollider>();

            // Setup player object
            // Use default primitive type of sphere so it is easily visible
            this.player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            this.player.transform.position = Vector3.zero;

            // Add a sphere collider to the player
            SphereCollider playerCollider = this.player.GetComponent<SphereCollider>();
            playerCollider.center = Vector3.zero;
            playerCollider.radius = 0.5f;
            // Position player above floor
            playerCollider.transform.position = new Vector3(0, 0.5f, 0);

            // Add a primitive colider cast and kinematic character controller to object
            // These will be created with default settings
            PrimitiveColliderCast colliderCast = this.player.AddComponent<PrimitiveColliderCast>();
            colliderCast.layerMask = ~0;
            kcc = this.player.AddComponent<KinematicCharacterController>();

            // Wait for a fixed update to register the position of the floor in the physics simulation
            yield return new WaitForFixedUpdate();

            // Wait a frame for scene to render
            yield return null;

            // Setup unity and network service mocks for the kcc
            this.unityServiceMock = new Mock<IUnityService>();
            this.networkServiceMock = new Mock<INetworkService>();
            kcc.unityService = this.unityServiceMock.Object;
            kcc.networkService = this.networkServiceMock.Object;

            // Setup player as local player
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            // Setup time passage (as headless function)
            this.unityServiceMock.Setup(e => e.deltaTime).Returns(() => Time.deltaTime);
            this.unityServiceMock.Setup(e => e.fixedDeltaTime).Returns(() => Time.fixedDeltaTime);
        }

        [UnityTearDown]
        public virtual IEnumerator TearDown()
        {
            // Delete any created platforms
            GameObject.DestroyImmediate(floor);

            // Wait for a fixed update to unregister created floor
            yield return new WaitForFixedUpdate();

            // Clear out the created player
            GameObject.DestroyImmediate(player);

            // Wait a frame for scene to render
            yield return null;
        }
    }

    /// <summary>
    /// Class to verify behaviour of walking for kinematic character controller
    /// </summary>
    [TestFixture]
    public class KinematicCharacterControllerWalkingTest : KinematicCharacterControllerTestBase
    {
        [UnityTest]
        public IEnumerator TestPlayerWalkForward()
        {
            Vector3 start = base.player.transform.position;
            // Simulate walking forward
            kcc.inputMovement = new Vector3(0, 0, 1.0f);
            yield return new WaitForSeconds(3);

            Vector3 end = base.player.transform.position;

            Vector3 delta = end - start;

            // Assert that the player did move
            Assert.IsTrue(delta.magnitude > 0);
        }

        [UnityTest]
        public IEnumerator CoyoteTimeTest()
        {
            // Simulate walking forward
            // Set the coyote time of the player to be 3 seconds for this test
            KinematicCharacterController kcc = player.GetComponent<KinematicCharacterController>();
            kcc.inputMovement = new Vector3(0, 0, 1.0f);

            // Test to verify small gap between when character walks off the edge and when they start falling
            // Put the character on top of a box
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.transform.position = new Vector3(0, 0.5f, 0);
            base.player.transform.position = new Vector3(0, 1.5f, 0);

            // Wait for box position to be registered
            yield return new WaitForFixedUpdate();

            // Setup delta time to be mocked at 1/60th of a second
            unityServiceMock.Setup(e => e.fixedDeltaTime).Returns(1 / 60.0f);
            // Wait until player is "Standing on Ground"
            // (or a maximum of 10 seconds = 60 updates per second * 10 seconds = 600 updates)
            for (int update = 0; update < 600 && !kcc.StandingOnGround; update++)
            {
                yield return new WaitForFixedUpdate();
                yield return null;
            }
            Assert.IsTrue(kcc.StandingOnGround);
            // Wait until player has stepped off the platform or not "Standing on Ground"
            // (or a maximum of 10 seconds = 60 updates per second * 10 seconds = 600 updates)
            for (int update = 0; update < 600 && kcc.StandingOnGround; update++)
            {
                yield return new WaitForFixedUpdate();
                yield return null;
            }
            Assert.IsFalse(kcc.StandingOnGround);
            // Assert that the player is not standing on ground and that the player is not "falling"
            //  until the coyote time of 3 seconds has passed

            // Cleanup the created platform
            GameObject.DestroyImmediate(box);
        }
    }

    /// <summary>
    /// Class to verify behaviour of walking for kinematic character controller up stairs
    /// </summary>
    [TestFixture]
    public class KinematicCharacterControllerStairWalkingTest : KinematicCharacterControllerTestBase
    {
        GameObject[] stairObjects;
        int numSteps = 12;
        float stepHeight = 0.2f;
        float stepDepth = 0.35f;
        float width = 1.0f;
        float length = 10.0f;
        Vector3 stairsStart = new Vector3(0, 0, 3);

        [UnitySetUp]
        public override IEnumerator SetUp()
        {
            yield return base.SetUp();

            KinematicCharacterController kcc = base.player.GetComponent<KinematicCharacterController>();
            kcc.verticalSnapUp = stepHeight * 2;
            kcc.verticalSnapDown = stepHeight * 2;

            // Create stairs in front of the player
            // Stairs are made up of numSteps=12 steps spaced out by stepHeight=0.2 units with stepDepth=0.25 depth
            Vector3 direction = Vector3.forward;
            stairObjects = new GameObject[numSteps];

            for (int step = 0; step < numSteps; step++)
            {
                stairObjects[step] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stairObjects[step].AddComponent<BoxCollider>();
                stairObjects[step].transform.position = stairsStart + direction * (stepDepth * step + length / 2) + Vector3.up * stepHeight * (0.5f + step);
                stairObjects[step].transform.localScale = new Vector3(width, stepHeight, length);

                if (step == 0)
                {
                    stairObjects[step].name = "Stairs";
                }
                else
                {
                    stairObjects[step].transform.parent = stairObjects[0].transform;
                }
            }
            yield return new WaitForFixedUpdate();
        }

        [UnityTearDown]
        public override IEnumerator TearDown()
        {
            for (int i = 0; i < stairObjects.Length; i++)
            {
                GameObject.DestroyImmediate(stairObjects[i]);
            }
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestPlayerWalkUpStairs()
        {
            Vector3 start = base.player.transform.position;
            // Simulate walking forward
            kcc.inputMovement = new Vector3(0, 0, 1.0f);
            // So, steps start at (0, 0, 3), have a total depth of stepDepth = 0.25 and 12 steps
            // Total time to walk up steps should be (1 + 0.25 * 12) / speed
            float timeToScale = (stairsStart.magnitude + stepDepth * numSteps) / player.GetComponent<KinematicCharacterController>().movementSpeed;

            // Wait that long plus some extra time for loading
            yield return new WaitForSeconds(timeToScale + 0.1f);

            Vector3 end = base.player.transform.position;

            Vector3 delta = end - start;

            UnityEngine.Debug.Log($"Moved up {delta.y} units at {end} from {start}");
            // Assert that the player did move
            Assert.IsTrue(delta.magnitude > 0);
            // Assert that the player scaled the vertical steps
            Assert.IsTrue(delta.y >= stepHeight * (numSteps) - 0.01f);
        }
    }
}