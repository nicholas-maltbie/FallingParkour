
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Component to track an object's displacement and rotation during an update
    /// </summary>
    public class MovementTracking : MonoBehaviour, IMovingGround
    {
        /// <summary>
        /// Unity service for manaing updates
        /// </summary>
        public IUnityService unityService = new UnityService();

        /// <summary>
        /// Should momentum be transferred to players when they
        /// leave this object.
        /// </summary>
        public bool avoidTransferMomentum;

        /// <inheritdoc/>
        public bool AvoidTransferMomentum() => avoidTransferMomentum;

        /// <inheritdoc/>
        public bool ShouldAttach() => true;

        /// <summary>
        /// Previously measured position of an object (previous frame)
        /// </summary>
        public Vector3 PreviousPosition { get; private set; }

        /// <summary>
        /// Previously measured attitude of an object (previous frame)
        /// </summary>
        public Quaternion PreviousAttitude { get; private set; }

        /// <summary>
        /// Finds the change in attitude (expressed as a quaternion) between
        /// the current and previous update. QFinal * Inv(QInitial)
        /// </summary>
        public Quaternion ChangeAttitude { get; private set; }

        /// <summary>
        /// Displacement between current and previous update
        /// </summary>
        public Vector3 Displacement { get; private set; }

        public void Update()
        {
            ChangeAttitude = transform.rotation * Quaternion.Inverse(PreviousAttitude);
            Displacement = transform.position - PreviousPosition;

            // Update state
            PreviousPosition = transform.position;
            PreviousAttitude = transform.rotation;
        }

        public Vector3 GetVelocityAtPoint(Vector3 point)
        {
            return GetDisplacementAtPoint(point) / unityService.deltaTime;
        }

        public Vector3 GetDisplacementAtPoint(Vector3 point)
        {
            // Get relative position to previous start
            Vector3 relativePosition = point - PreviousPosition;
            // Rotate point around center by change in attitude
            Vector3 rotatedFinalPosition = ChangeAttitude * relativePosition;
            // Get the delta due to rotation
            Vector3 deltaRotation = rotatedFinalPosition - relativePosition;
            // Shift point by total displacement
            return deltaRotation + Displacement;
        }
    }
}