using NUnit.Framework;
using PropHunt.Character;
using PropHunt.UI;
using PropHunt.UI.Actions;
using PropHunt.UI.Events;
using UnityEngine;

namespace Tests.EditMode.UI.Actions
{
    /// <summary>
    /// Tests for various UI Actions such as connect, disconnect, quit game actions
    /// </summary>
    [TestFixture]
    public class UIActionTests
    {
        private GameObject uiHolder;

        [SetUp]
        public void Setup()
        {
            this.uiHolder = new GameObject();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.uiHolder);
        }

        [Test]
        public void QuitGameActionTests()
        {
            this.uiHolder.AddComponent<QuitGameAction>();
            QuitGameAction action = this.uiHolder.GetComponent<QuitGameAction>();
            // Just call the method
            action.QuitGame();
        }

        [Test]
        public void PlayerMovementStateOnMenuLoadTests()
        {
            // Setup menu movement state that denies movement upon enabling
            PlayerMovementStateOnMenuLoad menuLoadMovementState = this.uiHolder.AddComponent<PlayerMovementStateOnMenuLoad>();
            menuLoadMovementState.playerInputState = PlayerInputState.Deny;
            // set current state
            PlayerInputManager.playerMovementState = PlayerInputState.Allow;
            // Check to ensure will set to selected state when enabled
            menuLoadMovementState.OnEnable();
            Assert.IsTrue(menuLoadMovementState.playerInputState == PlayerInputState.Deny);
        }

        [Test]
        public void CursorStateOnMenuLoadTests()
        {
            // Setup menu cursor state that reveals and unlock the cursor
            CursorStateOnMenuLoad cursorStateOnMenuLoad = this.uiHolder.AddComponent<CursorStateOnMenuLoad>();
            cursorStateOnMenuLoad.cursorLockMode = CursorLockMode.None;
            cursorStateOnMenuLoad.cursorVisible = true;
            // set current state
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // Check to ensure cursor state has been properly updated after enabling this component
            cursorStateOnMenuLoad.OnEnable();
            Assert.IsTrue(Cursor.lockState == CursorLockMode.None);
            Assert.IsTrue(Cursor.visible == true);
        }

        [Test]
        public void ToggleTransportActionTests()
        {
            // Setup toggle transport action
            ToggleTransportAction toggleTransportAction = this.uiHolder.AddComponent<ToggleTransportAction>();
            // Setup default toggle transport
            ToggleTransport.Instance = this.uiHolder.AddComponent<ToggleTransport>();
            ToggleTransport.Instance.currentMode = MultiplayerMode.None;
            // Verify current transport mode
            Assert.IsTrue(ToggleTransport.Instance.currentMode == MultiplayerMode.None);
            // Just invoke the method, should do nothing
            toggleTransportAction.SetMultiplayerMode("None");
            // Should do nothing
            Assert.IsTrue(ToggleTransport.Instance.currentMode == MultiplayerMode.None);
        }
    }
}