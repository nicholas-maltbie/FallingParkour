
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Component to track an object's displacement and rotation during a fixed update
    /// </summary>
    public class MovementTracking : MonoBehaviour, IMovingGround
    {
        public IUnityService unityService = new UnityService();

        /// <summary>
        /// Should momentum be transferred to players
        /// </summary>
        public bool avoidTransferMomentum;

        /// <summary>
        /// Previously measured position of an object (previous frame)
        /// </summary>
        public Vector3 previousPosition;

        /// <summary>
        /// Previously measured attitude of an object (previous frame)
        /// </summary>
        public Quaternion previousAttitude;

        /// <summary>
        /// Finds the change in attitude (expressed as a quaternion) between
        /// the current and previous fixed update. QFinal * Inv(QInitial)
        /// </summary>
        public Quaternion ChangeAttitude;

        /// <summary>
        /// Displacement between current and previous fixed update
        /// </summary>
        public Vector3 Displacement;

        public void FixedUpdate()
        {
            ChangeAttitude = transform.rotation * Quaternion.Inverse(previousAttitude);
            Displacement = transform.position - previousPosition;

            // Update state
            previousPosition = transform.position;
            previousAttitude = transform.rotation;
        }

        public Vector3 GetVelocityAtPoint(Vector3 point)
        {
            return GetDisplacementAtPoint(point) / unityService.fixedDeltaTime;
        }

        public Vector3 GetDisplacementAtPoint(Vector3 point)
        {
            // Get relative position to previous start
            Vector3 relativePosition = point - previousPosition;
            // Rotate point around center by change in attitude
            Vector3 rotatedFinalPosition = ChangeAttitude * relativePosition;
            // Get the delta due to rotation
            Vector3 deltaRotation = rotatedFinalPosition - relativePosition;
            // Shift point by total displacement
            return deltaRotation + Displacement;
        }
    }
}