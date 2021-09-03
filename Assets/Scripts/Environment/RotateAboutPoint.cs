using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Set parameters for a kinematic rigidbody
    /// </summary>
    public class RotateAboutPoint : NetworkBehaviour
    {
        /// <summary>
        /// Angular velocity of object in degrees per second for each euclidian axis
        /// </summary>
        [SerializeField]
        [SyncVar]
        [Tooltip("Angular velocity of object in degrees per second for each euclidian axis")]
        protected Vector3 angularVelocity;

        /// <summary>
        /// Accelerating the rigidbody in degrees per second squared
        /// </summary>
        [SerializeField]
        [SyncVar]
        [Tooltip("Acceleration of rigidbody c squared")]
        protected Vector3 angularAcceleration;

        /// <summary>
        /// maximum angular velocity in each axis in degrees per second
        /// </summary>
        [SerializeField]
        [SyncVar]
        [Tooltip("Maximum angular velocity in each axis in degrees per second")]
        protected Vector3 maxAngularVelocity;

        /// <summary>
        /// Position of the object.
        /// </summary>
        protected Vector3 pos;

        /// <summary>
        /// Rigidbody attached to the object
        /// </summary>
        protected Rigidbody objRigidbody;

        public void Start()
        {
            pos = transform.position;
            objRigidbody = GetComponent<Rigidbody>();
        }

        public void Update()
        {
            angularVelocity += angularAcceleration * Time.deltaTime;
            angularVelocity = new Vector3(
                Mathf.Min(angularVelocity.x, maxAngularVelocity.x),
                Mathf.Min(angularVelocity.y, maxAngularVelocity.y),
                Mathf.Min(angularVelocity.z, maxAngularVelocity.z));
            transform.position = pos;
            objRigidbody.centerOfMass = Vector3.zero;
            objRigidbody.velocity = Vector3.zero;
            objRigidbody.angularVelocity = Mathf.Deg2Rad * angularVelocity;
        }
    }
}