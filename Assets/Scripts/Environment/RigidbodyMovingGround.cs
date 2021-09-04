using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Moving ground defined by a rigidbody
    /// </summary>
    public class RigidbodyMovingGround : MonoBehaviour, IMovingGround
    {
        public IUnityService unityService = new UnityService();
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
        public Vector3 GetVelocityAtPoint(Vector3 point)
        {
            return attachedRigidbody.GetPointVelocity(point);
        }

        /// <inheritdoc/>
        public Vector3 GetDisplacementAtPoint(Vector3 point)
        {
            return attachedRigidbody.GetPointVelocity(point) * unityService.deltaTime;
        }
    }
}