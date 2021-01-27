using Moq;
using NUnit.Framework;
using PropHunt.UI;
using PropHunt.Utils;
using UnityEngine;
using static PropHunt.UI.MenuController;

namespace Tests.EditMode.UI
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
        /// Current screen
        /// </summary>
        private string currentScreen;

        [SetUp]
        public void Setup()
        {
            this.menuControllerObject = new GameObject();
            this.menuController = this.menuControllerObject.AddComponent<MenuController>();

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
    }
}