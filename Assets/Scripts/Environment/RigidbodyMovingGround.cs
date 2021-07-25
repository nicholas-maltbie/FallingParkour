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