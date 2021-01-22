using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Character
{
    /// <summary>
    /// Tests to verify behaviour of focus detection script
    /// </summary>
    [TestFixture]
    public class FocusDetectionTests
    {
        /// <summary>
        /// Focus detection object being tested
        /// </summary>
        FocusDetection focusDetection;

        /// <summary>
        /// Mock of network service attached to cameraFollow script
        /// </summary>
        Mock<INetworkService> networkServiceMock;

        /// <summary>
        /// What we want the player to look at
        /// </summary>
        GameObject focusTarget;

        [SetUp]
        public void SetUp()
        {
            // Setup a game object for our player
            GameObject playerObject = new GameObject();
            // Add a FocusDetection Behaviour to our object
            this.focusDetection = playerObject.AddComponent<FocusDetection>();
            this.focusDetection.Start();
            // Setup the fields for the focus detection
            // Setup and connect mocked network connection
            this.networkServiceMock = new Mock<INetworkService>();
            this.focusDetection.networkService = this.networkServiceMock.Object;
            // Make player object it's own camera
            this.focusDetection.cameraTransform = playerObject.transform;

            // Setup thing for player to look at
            focusTarget = new GameObject();
            // Make it a box
            BoxCollider box = focusTarget.AddComponent<BoxCollider>();
            // Move it to position (0, 0, 2), which is 2 units in front of the player
            box.transform.position = new Vector3(0, 0, 2);
        }

        [TearDown]
        public void TearDown()
        {
            // Cleanup created game object
            GameObject.DestroyImmediate(this.focusDetection.gameObject);
            GameObject.DestroyImmediate(focusTarget);
        }

        [UnityTest]
        public IEnumerator TestDoNothingIfNotLocal()
        {
            // Wait a frame to update the physics world and load colliders
            yield return null;
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(false);
            this.focusDetection.Update();
            // Make sure it didn't update to look at something
            Assert.IsTrue(this.focusDetection.focus == null);
        }

        [UnityTest]
        public IEnumerator TestLookingAtObject()
        {
            // Wait a frame to update the physics world and load colliders
            yield return null;

            RaycastHit hit;
            bool isSomethingThere = Physics.SphereCast(new Vector3(0, 0, 0), 0.1f, new Vector3(0, 0, 1), out hit, 5.0f);
            Debug.Log(isSomethingThere + " " + hit);

            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            this.focusDetection.Update();
            // Make sure is looking at the box
            Assert.IsTrue(this.focusDetection.focus == focusTarget);
        }

        [UnityTest]
        public IEnumerator TestLookingAwayFromObject()
        {
            // Wait a frame to update the physics world and load colliders
            yield return null;

            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            // rotate the player so they are looking away from the box
            // rotate player 90 degrees to look away from box
            this.focusDetection.transform.rotation = Quaternion.Euler(0, 90, 0);
            this.focusDetection.Update();
            // Make sure is looking at the box
            Assert.IsTrue(this.focusDetection.focus == null);
        }
    }
}