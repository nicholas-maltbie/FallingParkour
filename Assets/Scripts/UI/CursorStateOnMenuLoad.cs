using UnityEngine;

namespace PropHunt.UI
{
    /// <summary>
    /// Simple class to set cursor state when the menu loads
    /// </summary>
    public class CursorStateOnMenuLoad : MonoBehaviour
    {
        /// <summary>
        /// Lock state of cursor when loading this screen
        /// </summary>
        public CursorLockMode cursorLockMode = CursorLockMode.None;

        /// <summary>
        /// Visible state of the cursor when loading this screen
        /// </summary>
        public bool cursorVisible = true;

        public void OnEnable()
        {
            Cursor.lockState = cursorLockMode;
            Cursor.visible = cursorVisible;
        }
    }
}