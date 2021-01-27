using PropHunt.Character;
using UnityEngine;

namespace PropHunt.UI
{
    /// <summary>
    /// Simple class to set player movement state when menu loads
    /// </summary>
    public class PlayerMovementStateOnMenuLoad : MonoBehaviour
    {
        /// <summary>
        /// Player input state to set when this menu is loaded
        /// </summary>
        public PlayerInputState playerInputState = PlayerInputState.Allow;

        public void OnEnable()
        {
            PlayerInputManager.playerMovementState = this.playerInputState;
        }
    }
}