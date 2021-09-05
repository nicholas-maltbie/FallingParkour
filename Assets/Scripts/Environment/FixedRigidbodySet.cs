using MLAPI;
using MLAPI.NetworkVariable;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Set parameters for a kinematic rigidbody
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class FixedRigidbodySet : NetworkBehaviour
    {

        /// <summary>
        /// Angular velocity of object in degrees per second for each euclidian axis
        /// </summary>
        [SerializeField]
        [Tooltip("Angular velocity of object in degrees per second for each euclidian axis")]
        protected NetworkVariableVector3 angularVelocity = new NetworkVariableVector3();

        /// <summary>
        /// Does this rotation work in local or world space. If true, will rotate in local space.
        /// If false will rotate in world space.
        /// </summary>
        [SerializeField]
        [Tooltip("Does this rotation work in local or world space")]
        protected NetworkVariableBool localRotation = new NetworkVariableBool();

        /// <summary>
        /// Linear velocity of object in units per second for each axis
        /// </summary>
        [SerializeField]
        [Tooltip("Linear velocity of object in units per second for each axis")]
        protected NetworkVariableVector3 linearVelocity = new NetworkVariableVector3();

        /// <summary>
        /// Does this velocity work in local or world space. If true, will translate in local space.
        /// If false will translate in world space.
        /// </summary>
        [SerializeField]
        [Tooltip("Does this translation work in local or world space.")]
        protected NetworkVariableBool localTranslation = new NetworkVariableBool();

        /// <summary>
        /// Rigidbody for this object
        /// </summary>
        protected new Rigidbody rigidbody;

        public void Start()
        {
            this.rigidbody = GetComponent<Rigidbody>();
            this.rigidbody.isKinematic = true;
        }

        public void FixedUpdate()
        {
            // move object by velocity
            Vector3 deltaPos = Time.fixedDeltaTime * linearVelocity.Value;
            if (localTranslation.Value && transform.parent != null)
            {
                this.rigidbody.MovePosition(transform.parent.position + transform.localPosition + deltaPos);
            }
            else
            {
                this.rigidbody.MovePosition(transform.position + deltaPos);
            }

            // rotate object by rotation
            Quaternion deltaRotation = Quaternion.Euler(Time.fixedDeltaTime * angularVelocity.Value);
            if (localRotation.Value && transform.parent != null)
            {
                this.rigidbody.MoveRotation(transform.parent.rotation * transform.localRotation * deltaRotation);
            }
            else
            {
                this.rigidbody.MoveRotation(transform.rotation * deltaRotation);
            }
        }
    }
}