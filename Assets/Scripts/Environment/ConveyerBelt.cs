using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Conveyer belt that has a uniform direction and speed
    /// </summary>
    public class ConveyerBelt : MonoBehaviour, IMovingGround
    {
        [SerializeField]
        private bool avoidTransferMomentum;

        [SerializeField]
        private Vector3 velocity;

        /// <inheritdoc/>
        public bool AvoidTransferMomentum() => avoidTransferMomentum;

        /// <inheritdoc/>
        public Vector3 GetDisplacementAtPoint(Vector3 point, float deltaTime) => velocity * deltaTime;

        /// <inheritdoc/>
        public Vector3 GetVelocityAtPoint(Vector3 point, float deltaTime) => velocity;

        /// <inheritdoc/>
        public bool ShouldAttach() => false;
    }
}