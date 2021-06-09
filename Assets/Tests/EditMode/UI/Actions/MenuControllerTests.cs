using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PropHunt.UI;
using PropHunt.Utils;
using UnityEngine;
using static PropHunt.UI.MenuController;

namespace Tests.EditMode.UI.Actions
{
    /// <summary>
    /// Tests for Menu Controller Tests
    /// </summary>
    [TestFixture]
    public class MenuControllerTests
    {
        /// <summary>
        /// Object to hold menu controller
        /// </summary>
        private GameObject menuControllerObject;

        /// <summary>
        /// Menu controller object
        /// </summary>
        private MenuController menuController;

        /// <summary>
        /// UIManager for managing previous screens
        /// </summary>
        private UIManager uiManager;

        /// <summary>
        /// Current screen
        /// </summary>
        private string currentScreen;

        [SetUp]
        public void Setup()
        {
            this.menuControllerObject = new GameObject();
            this.uiManager = this.menuControllerObject.AddComponent<UIManager>();
            this.menuController = this.menuControllerObject.AddComponent<MenuController>();

            this.uiManager.screenPrefabs = new List<Canvas>(new Canvas[] { menuControllerObject.AddComponent<Canvas>() });
            Assert.Throws<System.InvalidOperationException>(() => this.uiManager.Start());

            // Listen to requested screen change events
            UIManager.RequestScreenChange += (object source, RequestScreenChangeEventArgs args) =>
            {
                this.currentScreen = args.newScreen;
            };
        }

        [TearDown]
        public void TearDown()
        {
            // Cleanup game object
            this.uiManager.OnDestroy();
            GameObject.DestroyImmediate(this.menuControllerObject);
        }

        [Test]
        public void SetScreenTests()
        {
            GameObject holderObject = new GameObject();
            holderObject.name = "Helloworld";
            this.menuController.SetScreen(holderObject);
            Assert.IsTrue(this.currentScreen == "Helloworld");

            this.menuController.SetScreen("NewScreen");
            Assert.IsTrue(this.currentScreen == "NewScreen");

            GameObject.DestroyImmediate(holderObject);
        }

        [Test]
        public void TestSetScreenOnInput()
        {
            GameObject holderObject = new GameObject();
            holderObject.name = "default_screen";
            this.menuController.SetScreen(holderObject);
            holderObject.name = "input_change_screen";

            InputScreenChange change = new InputScreenChange();
            change.input = "Cancel";
            change.menu = holderObject;
            this.menuController.screenChangeInputs.Add(change);

            Mock<IUnityService> unityServiceMock = new Mock<IUnityService>();
            menuController.unityService = unityServiceMock.Object;
            unityServiceMock.Setup(e => e.GetButtonDown("Cancel")).Returns(true);
            this.menuController.Update();
            Assert.IsTrue(this.currentScreen == "input_change_screen");

            GameObject.DestroyImmediate(holderObject);
        }

        [Test]
        public void TestBlockedChange()
        {
            GameObject holderObject = new GameObject();
            holderObject.name = "default_screen";
            this.menuController.SetScreen(holderObject);
            holderObject.name = "input_change_screen";
            // Assert cannot change screen when disallowed
            this.menuController.allowInputChanges = false;

            InputScreenChange change = new InputScreenChange();
            change.input = "Cancel";
            change.menu = holderObject;
            this.menuController.screenChangeInputs.Add(change);

            Mock<IUnityService> unityServiceMock = new Mock<IUnityService>();
            menuController.unityService = unityServiceMock.Object;
            unityServiceMock.Setup(e => e.GetButtonDown("Cancel")).Returns(true);
            this.menuController.Update();
            Assert.IsFalse(this.currentScreen == "input_change_screen");

            this.menuController.PreviousScreen();
            this.menuController.SetScreen("input_change_screen");

            GameObject.DestroyImmediate(holderObject);
        }

        [Test]
        public void TestOperationOnInput()
        {
            // Setup a list of supported screens

            InputScreenOperation operation = new InputScreenOperation();
            operation.input = "Cancel";
            operation.operation = MenuOperation.Previous;
            this.menuController.screenChangeOperations.Add(operation);

            // Make a history of two screen operations
            GameObject previousScreen = new GameObject();
            previousScreen.name = "previous_screen";
            this.menuController.SetScreen(previousScreen);
            GameObject nextScreen = new GameObject();
            nextScreen.name = "next_screen";

            // Reset UI manager's screen list
            this.uiManager.OnDestroy();
            this.uiManager.screenPrefabs = new List<Canvas>(new Canvas[] { previousScreen.AddComponent<Canvas>(), nextScreen.AddComponent<Canvas>() });
            Assert.Throws<System.InvalidOperationException>(() => this.uiManager.Start());

            this.menuController.SetScreen(nextScreen);

            Mock<IUnityService> unityServiceMock = new Mock<IUnityService>();
            menuController.unityService = unityServiceMock.Object;
            unityServiceMock.Setup(e => e.GetButtonDown("Cancel")).Returns(true);
            this.menuController.Update();
            Assert.IsTrue(this.currentScreen == "previous_screen");

            GameObject.DestroyImmediate(previousScreen);
            GameObject.DestroyImmediate(nextScreen);
        }
    }
}