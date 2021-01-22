using NUnit.Framework;
using PropHunt.Environment;
using UnityEngine;

namespace Tests.EditMode.Environment
{
    /// <summary>
    /// File to test behaviour of teleporter
    /// </summary>
    [TestFixture]
    public class TeleporterTests
    {
        /// <summary>
        /// Our teleporter we are using for a test
        /// </summary>
        Teleporter teleporter;

        [SetUp]
        public void Setup()
        {
            GameObject teleporterGameObject = new GameObject();
            // Add a teleporter to our game object
            this.teleporter = teleporterGameObject.AddComponent<Teleporter>();
            // Position we are teleporting objects to
            GameObject teleportPosition = new GameObject();
            teleportPosition.transform.position = new Vector3(0, 10, 0);
            this.teleporter.teleportLocation = teleportPosition.transform;
        }

        [TearDown]
        public void TearDown()
        {
            // Destroy the game objects we created
            GameObject teleporterGameObject = teleporter.gameObject;
            GameObject teleportLocation = teleporter.gameObject;

            GameObject.DestroyImmediate(teleporterGameObject);
            GameObject.DestroyImmediate(teleportLocation);
        }

        [Test]
        public void TestTeleporterMoveObjects()
        {
            // Create an object to teleport and attach a collider
            GameObject objectToTeleport = new GameObject();
            Collider objectCollider = objectToTeleport.AddComponent<BoxCollider>();

            // Assert that the object has not moved
            Assert.IsTrue(objectToTeleport.transform.position == Vector3.zero);

            // Simulate colliding with the object
            teleporter.OnTriggerEnter(objectCollider);

            // Assert that the object has been teleported to the correct position
            Assert.IsTrue(objectToTeleport.transform.position == teleporter.teleportLocation.transform.position);

            // Cleanup the object we created
            GameObject.DestroyImmediate(objectCollider);
        }

        [Test]
        public void TestTeleporterMoveCharacterController()
        {
            // Create an object to teleport and attach a collider
            GameObject playerToTeleport = new GameObject();
            playerToTeleport.AddComponent<CharacterController>();
            Collider objectCollider = playerToTeleport.AddComponent<BoxCollider>();

            // Assert that the object has not moved
            Assert.IsTrue(playerToTeleport.transform.position == Vector3.zero);

            // Simulate colliding with the object
            teleporter.OnTriggerEnter(objectCollider);

            // Assert that the object has been teleported to the correct position
            Assert.IsTrue(playerToTeleport.transform.position == teleporter.teleportLocation.transform.position);

            // Cleanup the object we created
            GameObject.DestroyImmediate(objectCollider);
        }
    }
}