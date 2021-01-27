using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character
{
    /// <summary>
    /// Current player input state, is the player allowed to give input or not
    /// </summary>
    public enum PlayerInputState
    {
        Allow,
        Deny
    }

    /// <summary>
    /// Manager for player input information as a static class
    /// </summary>
    public static class PlayerInputManager
    {
        /// <summary>
        /// Current state of the player movement action
        /// </summary>
        public static PlayerInputState playerMovementState = PlayerInputState.Allow;
    }
}
