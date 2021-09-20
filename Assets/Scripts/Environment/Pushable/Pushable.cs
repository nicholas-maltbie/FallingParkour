using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

namespace PropHunt.Environment.Pushable
{
    /// <summary>
    /// Pushable object that can be shoved with a given force
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Pushable : NetworkBehaviour, IPushable
    {
        /// <summary>
        /// Object rigidbody for pushing by players.
        /// </summary>
        private Rigidbody objRigidbody;

        public void Awake()
        {
            objRigidbody = GetComponent<Rigidbody>();
        }

        /// <inheritdoc/>
        public void PushObjectServerAndLocal(Vector3 force, Vector3 point, ForceMode forceMode)
        {
            PushObject(force, point, forceMode);
            if (!IsServer)
            {
                PushObjectServerRpc(force, point, forceMode);
            }
        }

        /// <inheritdoc/>
        [ServerRpc(RequireOwnership = false)]
        public void PushObjectServerRpc(Vector3 force, Vector3 point, ForceMode forceMode)
        {
            PushObject(force, point, forceMode);
        }

        private void PushObject(Vector3 force, Vector3 point, ForceMode forceMode)
        {
            objRigidbody.AddForceAtPosition(force, point, forceMode);
            if (!IsServer)
            {
                var networkRigidbody = GetComponent<NetworkRigidbody>();
            }
        }
    }
}