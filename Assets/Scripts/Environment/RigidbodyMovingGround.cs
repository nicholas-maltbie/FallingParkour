using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Moving ground defined by a rigidbody
    /// </summary>
    public class RigidbodyMovingGround : MonoBehaviour, IMovingGround
    {
        [SerializeField]
        private Rigidbody attachedRigidbody;

        /// <summary>
        /// Should momentum be transferred to players when they
        /// leave this object.
        /// </summary>
        public bool avoidTransferMomentum;

        /// <inheritdoc/>
        public bool AvoidTransferMomentum() => avoidTransferMomentum;

        /// <inheritdoc/>
        public bool ShouldAttach() => true;

        public void Start()
        {
            if (attachedRigidbody == null)
            {
                attachedRigidbody = GetComponent<Rigidbody>();
            }
        }

        /// <inheritdoc/>
        public Vector3 GetVelocityAtPoint(Vector3 point, float deltaTime)
        {
            NetworkRigidbody nrb = attachedRigidbody.gameObject.GetComponent<NetworkRigidbody>();
            if (nrb != null)
            {
                Vector3 vel = attachedRigidbody.GetPointVelocity(point);
                attachedRigidbody.velocity = nrb.netVelocity.Value;
                attachedRigidbody.angularVelocity = nrb.netAngularVelocity.Value;
            }
            return attachedRigidbody.GetPointVelocity(point);
        }

        /// <inheritdoc/>
        public Vector3 GetDisplacementAtPoint(Vector3 point, float deltaTime)
        {
            return attachedRigidbody.GetPointVelocity(point) * deltaTime;
        }
    }
}