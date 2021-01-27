using System;
using System.Collections.Generic;
using UnityEngine;

namespace PropHunt.UI
{
    /// <summary>
    /// Class to hold arguments for showing a new screen
    /// </summary>
    public class RequestScreenChangeEventArgs : EventArgs
    {
        /// <summary>
        /// String identifier for the new screen to load
        /// </summary>
        public string newScreen;
    }

    /// <summary>
    /// Class to hold screen change in state
    /// </summary>
    public class ScreenChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Name of the previous screen shown
        /// </summary>
        public string oldScreen;

        /// <summary>
        /// Name of the new screen being changed to
        /// </summary>
        public string newScreen;
    }

    /// <summary>
    /// Class to manager various UI Screens
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        /// <summary>
        /// Events for requesting a screen change
        /// </summary>
        public static event EventHandler<RequestScreenChangeEventArgs> RequestScreenChange;

        /// <summary>
        /// Events for when a screen change has ocurred
        /// </summary>
        public static event EventHandler<ScreenChangeEventArgs> ScreenChangeOccur;

        /// <summary>
        /// Various screens to add into the scene
        /// </summary>
        public List<Canvas> screenPrefabs;

        /// <summary>
        /// Index of the first screen to show
        /// </summary>
        public int initialScreen;

        /// <summary>
        /// Name of the current screen being displayed
        /// </summary>
        public string CurrentScreen { get; private set; }

        /// <summary>
        /// Lookup table of name of screen to screen being shown
        /// </summary>
        private Dictionary<string, GameObject> screenLookup;

        public void Start()
        {
            if (this.screenPrefabs.Count == 0)
            {
                UnityEngine.Debug.Log("No valid screens to display for UIManager");
                return;
            }

            if (this.initialScreen < 0 || this.initialScreen > this.screenPrefabs.Count)
            {
                UnityEngine.Debug.Log($"Initial Screen {this.initialScreen} invalid, defaulting to screen {0}");
                this.initialScreen = 0;
            }

            // Setup dictionary of screens
            this.screenLookup = new Dictionary<string, GameObject>();
            for (int idx = 0; idx < this.screenPrefabs.Count; idx++)
            {
                string screenName = screenPrefabs[idx].name;
                // instantiate a copy of each screen and set all to disabled except current screen
                GameObject screen = GameObject.Instantiate(screenPrefabs[idx].gameObject);
                // Set object parent to this for more organized hierarchy
                screen.transform.SetParent(this.transform, worldPositionStays: false);
                this.screenLookup[screenName] = screen;
                if (idx == this.initialScreen)
                {
                    this.CurrentScreen = screenName;
                    this.screenLookup[screenName].SetActive(true);
                }
                else
                {
                    this.screenLookup[screenName].SetActive(false);
                }
            }
        }

        public void OnEnable()
        {
            // Setup listening to event queue
            UIManager.RequestScreenChange += this.HandleScreenRequest;
        }

        public void OnDisable()
        {
            UIManager.RequestScreenChange -= this.HandleScreenRequest;
        }

        /// <summary>
        /// Handle a request to change screens
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="eventArgs">arguments of screen change</param>
        public void HandleScreenRequest(object sender, RequestScreenChangeEventArgs eventArgs)
        {
            this.SetScreen(eventArgs.newScreen);
        }

        /// <summary>
        /// Sets this screen to be displayed
        /// </summary>
        /// <param name="screenName">Name of the screen to display</param>
        public void SetScreen(string screenName)
        {
            if (!this.screenLookup.ContainsKey(screenName))
            {
                UnityEngine.Debug.Log($"Screen name {screenName} not recognized as new screen");
                return;
            }
            if (screenName == this.CurrentScreen)
            {
                // no change, do nothing
                return;
            }

            GameObject currentlyDisplayed = this.screenLookup[this.CurrentScreen];
            GameObject newDisplay = this.screenLookup[screenName];

            currentlyDisplayed.SetActive(false);
            newDisplay.SetActive(true);

            ScreenChangeEventArgs changeEvent = new ScreenChangeEventArgs();
            changeEvent.oldScreen = this.CurrentScreen;
            changeEvent.newScreen = screenName;

            this.CurrentScreen = screenName;

            // invoke screen change event
            UIManager.ScreenChangeOccur?.Invoke(this, changeEvent);
        }

        /// <summary>
        /// Requests a new screen to be shown
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="name">Name of new screen to show</param>
        public static void RequestNewScreen(object sender, string name)
        {
            RequestScreenChangeEventArgs request = new RequestScreenChangeEventArgs();
            request.newScreen = name;
            UIManager.RequestScreenChange?.Invoke(sender, request);
        }
    }
}