using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Set parameters for a kinematic rigidbody
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class FixedRigidbodySet : MonoBehaviour
    {
        public IUnityService unityService = new UnityService();
        private Rigidbody attachedRigidbody;

        /// <summary>
        /// Angular velocity of object in degrees per second for each euclidian axis
        /// </summary>
        [SerializeField]
        [Tooltip("Angular velocity of object in degrees per second for each euclidian axis")]
        protected Vector3 angularVelocity;

        /// <summary>
        /// Linear velocity of object in units per second for each axis
        /// </summary>
        [SerializeField]
        [Tooltip("Linear velocity of object in units per second for each axis")]
        protected Vector3 linearVelocity;

        public void Start()
        {
            attachedRigidbody = GetComponent<Rigidbody>();
            attachedRigidbody.isKinematic = true;
        }

        public void FixedUpdate()
        {
            float deltaTime = unityService.fixedDeltaTime;

            attachedRigidbody.angularVelocity = Vector3.zero;
            attachedRigidbody.velocity = Vector3.zero;

            // move object by velocity
            transform.position += deltaTime * linearVelocity;
            // rotate object by rotation
            transform.eulerAngles += deltaTime * angularVelocity;
        }
    }
}