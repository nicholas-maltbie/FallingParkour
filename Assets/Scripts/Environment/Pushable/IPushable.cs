using UnityEngine;
using MLAPI.Messaging;

namespace PropHunt.Environment.Pushable
{
    /// <summary>
    /// Pushable object that can be shoved with a given force
    /// </summary>
    public interface IPushable
    {
        /// <summary>
        /// Send a command to push an object at a specific position.
        /// </summary>
        /// <param name="force">Force vector applied to the object.</param>
        /// <param name="point">Point to apply force on the object.</param>
        /// <param name="forceMode">Mode of the force being applied.</param>
        [ServerRpc(RequireOwnership = false)]
        void PushObjectServerRpc(Vector3 force, Vector3 point, ForceMode forceMode);

        /// <summary>
        /// Send a command to push an object at a specific position on the server and locally.
        /// </summary>
        /// <param name="force">Force vector applied to the object.</param>
        /// <param name="point">Point to apply force on the object.</param>
        /// <param name="forceMode">Mode of the force being applied.</param>
        [ServerRpc(RequireOwnership = false)]
        void PushObjectServerAndLocal(Vector3 force, Vector3 point, ForceMode forceMode);
    }
}