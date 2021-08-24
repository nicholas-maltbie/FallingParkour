using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Moving ground defined by a rigidbody
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyMovingGround : MonoBehaviour, IMovingGround
    {
        public IUnityService unityService = new UnityService();
        private Rigidbody attachedRigidbody;

        /// <summary>
        /// Should momentum be transferred to players when they
        /// leave this object.
        /// </summary>
        public bool avoidTransferMomentum;

        /// <inheritdoc/>
        public bool AvoidTransferMomentum() => avoidTransferMomentum;

        /// <inheritdoc/>
        public bool ShouldAttach() => false;

        public void Start()
        {
            attachedRigidbody = GetComponent<Rigidbody>();
        }

        /// <inheritdoc/>
        public Vector3 GetVelocityAtPoint(Vector3 point)
        {
            return attachedRigidbody.GetPointVelocity(point);
        }

        /// <inheritdoc/>
        public Vector3 GetDisplacementAtPoint(Vector3 point)
        {
            return attachedRigidbody.GetPointVelocity(point) * unityService.fixedDeltaTime;
        }
    }
}