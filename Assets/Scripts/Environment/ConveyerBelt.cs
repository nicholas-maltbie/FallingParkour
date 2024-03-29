using UnityEngine;

namespace PropHunt.Environment
{
    public enum RelativeDirection
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Backward
    }

    /// <summary>
    /// Conveyer belt that has a uniform direction and speed
    /// </summary>
    public class ConveyerBelt : MonoBehaviour, IMovingGround
    {
        [SerializeField]
        private bool avoidTransferMomentum;

        [SerializeField]
        private float velocity;

        [SerializeField]
        private RelativeDirection direction = RelativeDirection.Down;

        public Vector3 GetDirection()
        {
            switch (this.direction)
            {
                case RelativeDirection.Up:
                    return transform.up;
                case RelativeDirection.Down:
                    return -transform.up;
                case RelativeDirection.Left:
                    return -transform.right;
                case RelativeDirection.Right:
                    return transform.right;
                case RelativeDirection.Forward:
                    return transform.forward;
                case RelativeDirection.Backward:
                    return -transform.forward;
            }
            return transform.forward;
        }

        /// <inheritdoc/>
        public bool AvoidTransferMomentum() => avoidTransferMomentum;

        /// <inheritdoc/>
        public Vector3 GetDisplacementAtPoint(Vector3 point, float deltaTime) =>
            velocity * GetDirection() * deltaTime;

        /// <inheritdoc/>
        public Vector3 GetVelocityAtPoint(Vector3 point, float deltaTime) =>
            velocity * GetDirection();

        /// <inheritdoc/>
        public bool ShouldAttach() => false;

        public void OnCollisionStay(Collision other)
        {
            if (other.rigidbody != null && !other.rigidbody.isKinematic)
            {
                Vector3 movement = velocity * GetDirection() * Time.deltaTime;
                other.gameObject.GetComponent<Rigidbody>().MovePosition(
                    other.gameObject.transform.position + movement);
            }
        }

        /// <inheritdoc/>
        public float GetMovementWeight(Vector3 point, Vector3 playerVelocity, float deltaTime)
        {
            return 1.0f;
        }

        /// <inheritdoc/>
        public float GetTransferMomentumWeight(Vector3 point, Vector3 playerVelocity, float deltaTime)
        {
            return 1.0f;
        }
    }
}