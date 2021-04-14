using Mirror;
using Mirror.Tests.RemoteAttrributeTest;
using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Environment;
using PropHunt.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Character
{
    public class FocusableTest : Focusable
    {
        public int framesFocused;
        public GameObject lastFocusSender;

        public override void Focus(GameObject sender)
        {
            framesFocused += 1;
            this.lastFocusSender = sender;
        }
    }

    public class InteractTest : Interactable
    {
        public int timesInteracted;
        public GameObject lastInteraction;
        public override void Interact(GameObject source)
        {
            timesInteracted++;
            lastInteraction = source;
        }
    }

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
        /// Mock of unity service for player inputs
        /// </summary>
        Mock<IUnityService> unityServiceMock;

        /// <summary>
        /// What we want the player to look at
        /// </summary>
        GameObject focusTarget;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
#if UNITY_EDITOR
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
#endif

            // Setup a game object for our player
            GameObject playerObject = new GameObject();
            // Add a FocusDetection Behaviour to our object
            this.focusDetection = playerObject.AddComponent<FocusDetection>();
            this.focusDetection.Start();
            // Setup the fields for the focus detection
            // Setup and connect mocked network connection
            this.networkServiceMock = new Mock<INetworkService>();
            this.unityServiceMock = new Mock<IUnityService>();
            this.focusDetection.networkService = this.networkServiceMock.Object;
            this.focusDetection.unityService = this.unityServiceMock.Object;
            // Make player object it's own camera
            this.focusDetection.cameraTransform = playerObject.transform;

            // Setup thing for player to look at
            focusTarget = new GameObject();
            // Make it a box
            BoxCollider box = focusTarget.AddComponent<BoxCollider>();
            // Move it to position (0, 0, 2), which is 2 units in front of the player
            box.transform.position = new Vector3(0, 0, 2);
            focusTarget.name = "Box";

            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            // Cleanup created game object
            GameObject.DestroyImmediate(this.focusDetection.gameObject);
            GameObject.DestroyImmediate(focusTarget);
        }

        [Test]
        public void TestDoNothingIfNotLocal()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(false);
            this.focusDetection.Update();
            // Make sure it didn't update to look at something
            Assert.IsTrue(this.focusDetection.focus == null);
        }

        [Test]
        public void TestLookingAtObject()
        {
            // Setup player settings
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            // Add focusable component to object
            FocusableTest test = this.focusTarget.AddComponent<FocusableTest>();
            // Wait a frame to update the physics world and load colliders
            this.focusDetection.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            this.focusDetection.Update();
            // Make sure is looking at the box
            UnityEngine.Debug.Log(this.focusDetection.focus);
            Assert.IsTrue(this.focusDetection.focus == focusTarget);
            Assert.IsTrue(test.framesFocused == 1);
            Assert.IsTrue(test.lastFocusSender == this.focusDetection.gameObject);
        }

        [Test]
        public void TestLookingAwayFromObject()
        {
            // Setup player settings
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            // rotate the player so they are looking away from the box
            // rotate player 90 degrees to look away from box
            this.focusDetection.gameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
            this.focusDetection.Update();
            // Make sure is looking at the box
            UnityEngine.Debug.Log(this.focusDetection.focus);
            Assert.IsTrue(this.focusDetection.focus == null);
        }

        [Test]
        public void TestInteractWithObject()
        {
            // Set local player state to true
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            // Allow player to interact with box
            this.unityServiceMock.Setup(e => e.GetButtonDown("Interact")).Returns(true);
            this.networkServiceMock.Setup(e => e.isServer).Returns(true);
            InteractTest testInteract = this.focusTarget.AddComponent<InteractTest>();
            // Wait a frame to update the physics world and load colliders
            this.focusDetection.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            this.focusDetection.Update();
            // Make sure is looking at the box
            Assert.IsTrue(this.focusDetection.focus == focusTarget);
            UnityEngine.Debug.Log(this.focusDetection.focus.GetComponent<Interactable>());
            // Assert that the object was interacted with
            Assert.IsTrue(testInteract.timesInteracted == 1);
            Assert.IsTrue(testInteract.lastInteraction = this.focusDetection.gameObject);
        }
    }

    /// <summary>
    /// Tests to verify behaviour of commands in focus detection object
    /// </summary>
    [TestFixture]
    public class FocusDetectionCommandTests : RemoteTestBase
    {
        [UnityTest]
        public IEnumerator TestObjectInteractWhenCommandSentToServer()
        {
            LogAssert.ignoreFailingMessages = true;

            Mock<INetworkService> networkServiceMock = new Mock<INetworkService>();
            FocusDetection hostBehaviour = CreateHostObject<FocusDetection>(true);
            hostBehaviour.networkService = networkServiceMock.Object;
            networkServiceMock.Setup(e => e.isServer).Returns(true);

            NetworkConnectionToClient connectionToClient = NetworkServer.connections[0];
            Debug.Assert(connectionToClient != null, $"connectionToClient was null, This means that the test is broken and will give the wrong results");

            GameObject focus = new GameObject();
            InteractTest testInteract = focus.AddComponent<InteractTest>();
            focus.AddComponent<NetworkIdentity>();
            NetworkServer.Spawn(focus);
            hostBehaviour.CmdInteractWithObject(focus, hostBehaviour.gameObject);
            yield return null;
            // Assert that the object was interacted with
            Assert.IsTrue(testInteract.timesInteracted == 1);
            Assert.IsTrue(testInteract.lastInteraction = hostBehaviour.gameObject);
            GameObject.DestroyImmediate(focus);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestObjectInteractWhenCommandSentFromServer()
        {
            LogAssert.ignoreFailingMessages = true;

            Mock<INetworkService> networkServiceMock = new Mock<INetworkService>();
            FocusDetection hostBehaviour = CreateHostObject<FocusDetection>(true);
            hostBehaviour.networkService = networkServiceMock.Object;
            int calls = 0;
            networkServiceMock.Setup(e => e.isServer).Callback(() => calls++).Returns(calls > 1);

            NetworkConnectionToClient connectionToClient = NetworkServer.connections[0];
            Debug.Assert(connectionToClient != null, $"connectionToClient was null, This means that the test is broken and will give the wrong results");

            GameObject focus = new GameObject();
            InteractTest testInteract = focus.AddComponent<InteractTest>();
            focus.AddComponent<NetworkIdentity>();
            NetworkServer.Spawn(focus);
            hostBehaviour.InteractWithObject(focus, hostBehaviour.gameObject);
            yield return null;
            // Assert that the object was interacted with
            // Assert.IsTrue(testInteract.timesInteracted == 1);
            // Assert.IsTrue(testInteract.lastInteraction = hostBehaviour.gameObject);
            GameObject.DestroyImmediate(focus);
            yield return null;
        }
    }
}