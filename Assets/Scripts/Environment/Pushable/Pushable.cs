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

        /// <summary>
        /// Cooldown in seconds between how soon the owner can change.
        /// </summary>
        private float ownerChangeCooldown = 0.05f;

        /// <summary>
        /// Last time the owner was changed.
        /// </summary>
        private float lastOwnerChangeTime = Mathf.NegativeInfinity;

        public void Awake()
        {
            objRigidbody = GetComponent<Rigidbody>();
        }

        /// <inheritdoc/>
        public void PushObjectServerAndLocal(Vector3 force, Vector3 point, ForceMode forceMode, ulong sourceId)
        {
            if (!IsServer)
            {
                PushObjectServerRpc(force, point, forceMode, sourceId);
            }
            else if (Time.time - lastOwnerChangeTime >= ownerChangeCooldown)
            {
                gameObject.GetComponent<NetworkObject>().RemoveOwnership();
                lastOwnerChangeTime = Time.time;
            }
            PushObject(force, point, forceMode);
        }

        /// <inheritdoc/>
        [ServerRpc(RequireOwnership = false)]
        public void PushObjectServerRpc(Vector3 force, Vector3 point, ForceMode forceMode, ulong sourceId)
        {
            if (Time.time - lastOwnerChangeTime >= ownerChangeCooldown)
            {
                gameObject.GetComponent<NetworkObject>().ChangeOwnership(sourceId);
                lastOwnerChangeTime = Time.time;
            }
            PushObject(force, point, forceMode);
        }

        private void PushObject(Vector3 force, Vector3 point, ForceMode forceMode)
        {
            if (!IsOwner)
            {
                var networkRigidbody = GetComponent<NetworkRigidbody>();
            }
            objRigidbody.AddForceAtPosition(force, point, forceMode);
        }
    }
}