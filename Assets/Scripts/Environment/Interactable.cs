using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Behaviour that can be interacted with by a player
    /// </summary>
    public abstract class Interactable : MonoBehaviour
    {
        /// <summary>
        /// Have a player interact with this object, this should only be 
        /// invoked on the server.
        /// </summary>
        /// <param name="sender">Player sending the interaction command</param>
        public abstract void Interact(GameObject sender);
    }
}