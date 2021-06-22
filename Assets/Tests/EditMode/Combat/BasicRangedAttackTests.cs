using Mirror.Tests.RemoteAttributeTest;
using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Combat;
using PropHunt.Utils;
using System.Collections;
using Tests.EditMode.Game.Flow;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Combat
{
    /// <summary>
    /// Tests to verify behaviour of basic ranged attack
    /// </summary>
    [TestFixture]
    public class BasicRangedAttackTests : CustomNetworkManagerTestBase
    {
        BasicRangedAttack attack;

        Mock<INetworkService> networkServiceMock;

        Mock<IUnityService> unityServiceMock;

        GameObject attackTarget;

        int attackCount = 0;

        public void CountAttack(object sender, PlayerAttackEvent attackEvent) => attackCount++;

        [SetUp]
        public override void SetUp()
        {
            reloadScene = false;
            base.SetUp();
        }

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
#if UNITY_EDITOR
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
                UnityEditor.SceneManagement.NewSceneMode.Single);
#endif
            // Count attacks made by player
            CombatManager.OnPlayerAttack += CountAttack;
            attackCount = 0;

            // Allow player attacks
            PlayerInputManager.playerMovementState = PlayerInputState.Allow;

            // Setup a game object for our player
            GameObject playerObject = new GameObject();
            playerObject.name = "player";
            // Add a FocusDetection Behaviour to our object
            CameraController controller = playerObject.AddComponent<CameraController>();
            this.attack = playerObject.AddComponent<BasicRangedAttack>();
            this.attack.Start();
            // Setup the fields for the basic ranged attack
            // Setup and connect mocked network connection
            this.networkServiceMock = new Mock<INetworkService>();
            this.unityServiceMock = new Mock<IUnityService>();
            this.attack.networkService = this.networkServiceMock.Object;
            this.attack.unityService = this.unityServiceMock.Object;
            // Make player object it's own camera
            controller.cameraTransform = playerObject.transform;

            // Setup thing for player to look at
            attackTarget = new GameObject();
            // Make it a box
            BoxCollider box = attackTarget.AddComponent<BoxCollider>();
            // Add a character team and other info to the target
            attackTarget.AddComponent<CharacterName>();
            attackTarget.AddComponent<PlayerTeam>().playerTeam = Team.Prop;
            // Move it to position (0, 0, 2), which is 2 units in front of the player
            box.transform.position = new Vector3(0, 0, 2);
            attackTarget.name = "Box";

            yield return null;
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            // Cleanup created game object
            GameObject.DestroyImmediate(this.attack.gameObject);
            GameObject.DestroyImmediate(attackTarget);

            CombatManager.OnPlayerAttack -= CountAttack;
        }

        [Test]
        public void TestDoNothingIfNotLocal()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(false);
            this.attack.Update();
        }

        [Test]
        public void TestAttackCooldownTiming()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            this.networkServiceMock.Setup(e => e.isServer).Returns(true);
            this.unityServiceMock.Setup(e => e.time).Returns(0);
            // Assert that we can attack at start
            Assert.IsTrue(this.attack.CanAttack);
            // Attempt to attack, then assert that cooldown has kicked in
            this.attack.Attack(new UnityEngine.InputSystem.InputAction.CallbackContext());
            this.attack.attacking = true;
            this.attack.Update();
            Assert.IsTrue(attackCount == 1);
            Assert.IsFalse(this.attack.CanAttack);
        }
    }

    /// <summary>
    /// Tests to verify behaviour of commands in focus detection object
    /// </summary>
    [TestFixture]
    public class BasicRangedAttackCommandTests : RemoteTestBase
    {
        [UnityTest]
        public IEnumerator TestVerifyAttackNotServer()
        {
            PlayerInputManager.playerMovementState = PlayerInputState.Allow;
            Mock<INetworkService> networkServiceMock = new Mock<INetworkService>();
            Mock<IUnityService> unityServiceMock = new Mock<IUnityService>();

            BasicRangedAttack attack = CreateHostObject<BasicRangedAttack>(true);

            attack.Start();
            attack.GetComponent<CameraController>().cameraTransform = attack.gameObject.transform;

            attack.networkService = networkServiceMock.Object;
            attack.unityService = unityServiceMock.Object;

            networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            networkServiceMock.Setup(e => e.isServer).Returns(false);
            unityServiceMock.Setup(e => e.time).Returns(0);
            yield return null;
            // Assert that we can attack at start
            Assert.IsTrue(attack.CanAttack);
            // Attempt to attack, then assert that cooldown has kicked in
            attack.attacking = true;
            attack.Update();

            yield return null;
        }
    }
}