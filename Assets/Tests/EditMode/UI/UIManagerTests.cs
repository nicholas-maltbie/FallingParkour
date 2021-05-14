using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using PropHunt.UI;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.UI
{
    /// <summary>
    /// Tests for UIManager Tests
    /// </summary>
    [TestFixture]
    public class UIManagerTests
    {
        /// <summary>
        /// Object to hold UIManager
        /// </summary>
        private GameObject uiManagerObject;

        /// <summary>
        /// uiManager object
        /// </summary>
        private UIManager uiManager;

        /// <summary>
        /// Current screen
        /// </summary>
        private string currentScreen;

        /// <summary>
        /// Number of times the screne has changed
        /// </summary>
        private int screenChangeEvents = 0;

        private void HandleTestScreenChange(object source, ScreenChangeEventArgs args)
        {
            this.currentScreen = args.newScreen;
            this.screenChangeEvents++;
        }

        [SetUp]
        public void Setup()
        {
            UIManager.Instance = null;

            this.uiManagerObject = new GameObject();
            this.uiManager = this.uiManagerObject.AddComponent<UIManager>();

            this.currentScreen = "";
            this.screenChangeEvents = 0;

            // Listen to requested screen change events
            UIManager.ScreenChangeOccur += HandleTestScreenChange;
        }

        [TearDown]
        public void TearDown()
        {
            // Disable and cleanup UIManager
            this.uiManager.OnDestroy();
            GameObject.DestroyImmediate(this.uiManagerObject);
            UIManager.Instance = null;
            UIManager.ScreenChangeOccur -= HandleTestScreenChange;
        }

        [Test]
        public void UIManagerSetupWithNoScreens()
        {
            this.uiManager.screenPrefabs = new List<Canvas>();
            this.uiManager.initialScreen = -1;

            this.uiManager.Start();

            Assert.IsTrue(this.uiManager.initialScreen == -1);
        }

        [Test]
        public void UIManagerDestoryDuplicate()
        {
            this.uiManager.screenPrefabs = new List<Canvas>();
            this.uiManager.initialScreen = -1;

            this.uiManager.Start();
            this.uiManager.OnDestroy();
            this.uiManager.Start();
            this.uiManager.OnDestroy();

            // Assert that the destory works as expected
            LogAssert.ignoreFailingMessages = true;
            IEnumerator enumerator = this.uiManager.DestorySelf();
            while (enumerator.MoveNext()) { }

            Assert.IsTrue(this.uiManager.initialScreen == -1);
        }

        [Test]
        public void UIManagerSetupSelectedNegativeInvalidScreens()
        {
            this.uiManager.screenPrefabs = new List<Canvas>();
            GameObject screen = new GameObject();
            this.uiManager.screenPrefabs.Add(screen.AddComponent<Canvas>());
            this.uiManager.initialScreen = -10;

            Assert.Throws<System.InvalidOperationException>(() => this.uiManager.Start());

            foreach (Canvas canvasObject in this.uiManager.screenPrefabs)
            {
                GameObject.DestroyImmediate(canvasObject.gameObject);
            }

            Assert.IsTrue(this.uiManager.initialScreen == 0);

            GameObject.DestroyImmediate(screen);
        }

        [Test]
        public void UIManagerSetupSelectedOutOfRangeInvalidScreens()
        {
            this.uiManager.screenPrefabs = new List<Canvas>();
            GameObject screen = new GameObject();
            this.uiManager.screenPrefabs.Add(screen.AddComponent<Canvas>());
            this.uiManager.initialScreen = 10;

            Assert.Throws<System.InvalidOperationException>(() => this.uiManager.Start());

            foreach (Canvas canvasObject in this.uiManager.screenPrefabs)
            {
                GameObject.DestroyImmediate(canvasObject.gameObject);
            }

            Assert.IsTrue(this.uiManager.initialScreen == 0);

            GameObject.DestroyImmediate(screen);
        }

        [Test]
        public void UIManagerSetupCorrectlyAndSetScreens()
        {
            this.uiManager.screenPrefabs = new List<Canvas>();
            int totalScreens = 10;
            GameObject[] screens = new GameObject[totalScreens];
            string[] screenNames = new string[totalScreens];
            for (int screenIdx = 0; screenIdx < totalScreens; screenIdx++)
            {
                screens[screenIdx] = new GameObject();
                screens[screenIdx].name = "Screen" + screenIdx.ToString();
                screenNames[screenIdx] = screens[screenIdx].name;
                this.uiManager.screenPrefabs.Add(screens[screenIdx].AddComponent<Canvas>());
            }
            this.uiManager.initialScreen = 2;

            Assert.Throws<System.InvalidOperationException>(() => this.uiManager.Start());

            // Assert that the correct screen is still loaded
            Assert.IsTrue(this.uiManager.initialScreen == 2);
            this.screenChangeEvents = 0;

            // Request that screen 0 be loaded
            UIManager.RequestNewScreen(this, screenNames[0]);

            UnityEngine.Debug.Log(this.screenChangeEvents);
            Assert.IsTrue(this.screenChangeEvents == 1);
            Assert.IsTrue(this.currentScreen == screenNames[0]);

            // Request screen zero again
            UIManager.RequestNewScreen(this, screenNames[0]);
            // Should have no new screne load requests
            Assert.IsTrue(this.screenChangeEvents == 1);
            Assert.IsTrue(this.currentScreen == screenNames[0]);

            // Request an invalid screen
            UIManager.RequestNewScreen(this, "HelloWorld");
            // Should have no new screne load requests
            Assert.IsTrue(this.screenChangeEvents == 1);
            Assert.IsTrue(this.currentScreen == screenNames[0]);

            // Request an valid screen
            UIManager.RequestNewScreen(this, screenNames[5]);
            // Should have no new screne load requests
            Assert.IsTrue(this.screenChangeEvents == 2);
            Assert.IsTrue(this.currentScreen == screenNames[5]);

            // Request an valid screen
            UIManager.RequestNewScreen(this, screenNames[2]);
            // Should have no new screne load requests
            Assert.IsTrue(this.screenChangeEvents == 3);
            Assert.IsTrue(this.currentScreen == screenNames[2]);

            // Cleanup objects created by test
            foreach (Canvas canvasObject in this.uiManager.screenPrefabs)
            {
                GameObject.DestroyImmediate(canvasObject.gameObject);
            }

            foreach (GameObject screen in screens)
            {
                GameObject.DestroyImmediate(screen);
            }
        }
    }
}