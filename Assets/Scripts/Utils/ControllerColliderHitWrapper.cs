using UnityEngine;

namespace PropHunt.Utils
{
    /// <summary>
    /// Interface to hold the data for a controller collider hit event.
    /// Wrapping this in an interface makes the testing of a controller collider hit
    /// easier to manage.
    /// </summary>
    public interface IControllerColliderHit
    {
        /// <summary>
        /// The controller that hit the collider.
        /// </summary>
        CharacterController controller { get; }

        /// <summary>
        /// The collider that was hit by the controller.
        /// </summary>
        Collider collider { get; }

        /// <summary>
        /// The rigidbody that was hit by the controller.
        /// </summary>
        Rigidbody rigidbody { get; }

        /// <summary>
        /// The game object that was hit by the controller.
        /// </summary>
        GameObject gameObject { get; }

        /// <summary>
        /// The transform that was hit by the controller.
        /// </summary>
        Transform transform { get; }

        /// <summary>
        /// The impact point in world space.
        /// </summary>
        Vector3 point { get; }

        /// <summary>
        /// The normal of the surface we collided with in world space.
        /// </summary>
        Vector3 normal { get; }

        /// <summary>
        /// The direction the CharacterController was moving in when the collision occurred.
        /// </summary>
        Vector3 moveDirection { get; }

        /// <summary>
        /// How far the character has travelled until it hit the collider.
        /// </summary>
        float moveLength { get; }
    }

    /// <summary>
    /// Wrapper for the data in a ControllerColliderHit
    /// </summary>
    public class ControllerColliderHitWrapper : IControllerColliderHit
    {
        ControllerColliderHit colliderHit;

        public ControllerColliderHitWrapper(ControllerColliderHit colliderHit)
        {
            this.colliderHit = colliderHit;
        }

        /// <inheritdoc/>
        public CharacterController controller => colliderHit.controller;

        /// <inheritdoc/>
        public Collider collider => colliderHit.collider;

        /// <inheritdoc/>
        public Rigidbody rigidbody => colliderHit.rigidbody;

        /// <inheritdoc/>
        public GameObject gameObject => colliderHit.gameObject;

        /// <inheritdoc/>
        public Transform transform => colliderHit.transform;

        /// <inheritdoc/>
        public Vector3 point => colliderHit.point;

        /// <inheritdoc/>
        public Vector3 normal => colliderHit.normal;

        /// <inheritdoc/>
        public Vector3 moveDirection => colliderHit.normal;

        /// <inheritdoc/>
        public float moveLength => colliderHit.moveLength;
    }
}
