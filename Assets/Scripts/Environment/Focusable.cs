using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Behaviour that should do something when 'focused' (looked at) by a player.
    /// </summary>
    public abstract class Focusable : MonoBehaviour
    {
        /// <summary>
        /// Have a player focus on the given object, generally this should be invoked
        /// on the local client of the player.
        /// This should be invoked every frame that the player is looking at the object.
        /// An object will know it is 'unfocused' when it is not called for a given frame.
        /// </summary>
        /// <param name="sender">Player looking at this object</param>
        public abstract void Focus(GameObject sender);
    }
}