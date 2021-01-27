using System.Collections.Generic;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.UI
{
    /// <summary>
    /// Simple class to abstract commands to change UI for a menu screen
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        /// <summary>
        /// Serialize container to change screen whenever a player gives an input
        /// </summary>
        [System.Serializable]
        public class InputScreenChange
        {
            /// <summary>
            /// Input to listen to from the player, listen to button down event
            /// </summary>
            public string input;
            /// <summary>
            /// Screen to load when the player presses this input as a button down
            /// </summary>
            public GameObject menu;
        }

        /// <summary>
        /// Input screen change events for this menu
        /// </summary>
        public List<InputScreenChange> screenChangeInputs = new List<InputScreenChange>();

        /// <summary>
        /// Unity service for parsing player inputs
        /// </summary>
        public IUnityService unityService = new UnityService();

        public void Update()
        {
            foreach (InputScreenChange change in this.screenChangeInputs)
            {
                if (unityService.GetButtonDown(change.input))
                {
                    SetScreen(change.menu);
                }
            }
        }

        /// <summary>
        /// Request a new screen using a prefab name
        /// </summary>
        /// <param name="screenPrefab">Screen prefab to switch to</param>
        public void SetScreen(GameObject screenPrefab)
        {
            this.SetScreen(screenPrefab.name);
        }

        /// <summary>
        /// Request a new screen directly through a name
        /// </summary>
        /// <param name="name">Name of new screen to display</param>
        public void SetScreen(string name)
        {
            UIManager.RequestNewScreen(this, name);
        }
    }
}