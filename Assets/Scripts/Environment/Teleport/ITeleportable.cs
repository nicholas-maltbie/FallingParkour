using UnityEngine;

namespace PropHunt.Environment.Teleport
{
    /// <summary>
    /// Object that can be teleported.
    /// </summary>
    public interface ITeleportable
    {
        /// <summary>
        /// Trigger teleport event for this object from a given source.
        /// </summary>
        void Teleport(GameObject source);
    }
}