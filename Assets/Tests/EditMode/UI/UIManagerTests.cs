using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using PropHunt.UI;
using PropHunt.UI.Events;
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
            this.uiManager.screenPrefabs = new List<GameScreen>();
            this.uiManager.initialScreen = -1;

            this.uiManager.Start();

            Assert.IsTrue(this.uiManager.initialScreen == -1);
        }

        [Test]
        public void UIManagerDestoryDuplicate()
        {
            LogAssert.ignoreFailingMessages = true;
            this.uiManager.screenPrefabs = new List<GameScreen>();
            this.uiManager.initialScreen = -1;

            this.uiManager.Start();
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
            this.uiManager.screenPrefabs = new List<GameScreen>();
            GameObject screen = new GameObject();
            this.uiManager.screenPrefabs.Add(screen.AddComponent<GameScreen>());
            this.uiManager.initialScreen = -10;

            Assert.Throws<System.InvalidOperationException>(() => this.uiManager.Start());

            foreach (GameScreen canvasObject in this.uiManager.screenPrefabs)
            {
                GameObject.DestroyImmediate(canvasObject.gameObject);
            }

            Assert.IsTrue(this.uiManager.initialScreen == 0);

            GameObject.DestroyImmediate(screen);
        }

        [Test]
        public void UIManagerSetupSelectedOutOfRangeInvalidScreens()
        {
            this.uiManager.screenPrefabs = new List<GameScreen>();
            GameObject screen = new GameObject();
            this.uiManager.screenPrefabs.Add(screen.AddComponent<GameScreen>());
            this.uiManager.initialScreen = 10;

            Assert.Throws<System.InvalidOperationException>(() => this.uiManager.Start());

            foreach (GameScreen canvasObject in this.uiManager.screenPrefabs)
            {
                GameObject.DestroyImmediate(canvasObject.gameObject);
            }

            Assert.IsTrue(this.uiManager.initialScreen == 0);

            GameObject.DestroyImmediate(screen);
        }

        [Test]
        public void UIManagerLoadScreenOnEnable()
        {
            this.uiManager.screenPrefabs = new List<GameScreen>();
            GameObject screen1 = new GameObject();
            screen1.name = "Screen 1";
            GameObject screen2 = new GameObject();
            screen2.name = "Screen 2";
            this.uiManager.screenPrefabs.Add(screen1.AddComponent<GameScreen>());
            this.uiManager.screenPrefabs.Add(screen2.AddComponent<GameScreen>());

            this.uiManager.initialScreen = 0;

            Assert.Throws<System.InvalidOperationException>(() => this.uiManager.Start());

            LoadScreenOnEnable loadOnEnable = new GameObject().AddComponent<LoadScreenOnEnable>();

            Assert.IsTrue(this.uiManager.CurrentScreen == "Screen 1");
            loadOnEnable.selectedScreen = screen2;

            // Simulate enabling the object
            loadOnEnable.OnEnable();

            // Ensure loaded screen is correct
            Assert.IsTrue(this.uiManager.CurrentScreen == "Screen 2");

            GameObject.DestroyImmediate(screen1);
            GameObject.DestroyImmediate(screen2);
            GameObject.DestroyImmediate(loadOnEnable);
        }

        [Test]
        public void UIManagerSetupCorrectlyAndSetScreens()
        {
            this.uiManager.screenPrefabs = new List<GameScreen>();
            int totalScreens = 10;
            GameObject[] screenPrefabs = new GameObject[totalScreens];
            string[] screenNames = new string[totalScreens];
            for (int screenIdx = 0; screenIdx < totalScreens; screenIdx++)
            {
                screenPrefabs[screenIdx] = new GameObject();
                screenPrefabs[screenIdx].name = "Screen" + screenIdx.ToString();
                screenNames[screenIdx] = screenPrefabs[screenIdx].name;
                this.uiManager.screenPrefabs.Add(screenPrefabs[screenIdx].AddComponent<GameScreen>());
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

            // Add screenPrefabs until the history overflows
            for (int i = 0; i < uiManager.maxScreenHistory; i++)
            {
                UIManager.RequestNewScreen(this, screenNames[i % 2]);
            }

            // Attempt to revert to previous screen when history is empty
            uiManager.ClearHistory();
            string beforePrevious = this.currentScreen;
            UIManager.PreviousScreen(this);
            // assert that current screen has not changed
            Assert.IsTrue(this.currentScreen == beforePrevious);

            // Cleanup objects created by test
            foreach (GameScreen canvasObject in this.uiManager.screenPrefabs)
            {
                GameObject.DestroyImmediate(canvasObject.gameObject);
            }

            foreach (GameObject screen in screenPrefabs)
            {
                GameObject.DestroyImmediate(screen);
            }
        }
    }
}