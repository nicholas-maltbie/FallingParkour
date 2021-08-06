using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Set parameters for a kinematic rigidbody
    /// </summary>
    public class FixedRigidbodySet : NetworkBehaviour
    {
        public IUnityService unityService = new UnityService();

        /// <summary>
        /// Angular velocity of object in degrees per second for each euclidian axis
        /// </summary>
        [SerializeField]
        [SyncVar]
        [Tooltip("Angular velocity of object in degrees per second for each euclidian axis")]
        protected Vector3 angularVelocity;

        /// <summary>
        /// Current rotation of the object as a euclidian degrees
        /// </summary>
        [SerializeField]
        [SyncVar]
        [Tooltip("Current rotation of the object as a euclidian degrees")]
        protected Vector3 attitude;

        /// <summary>
        /// Linear velocity of object in units per second for each axis
        /// </summary>
        [SerializeField]
        [SyncVar]
        [Tooltip("Linear velocity of object in units per second for each axis")]
        protected Vector3 linearVelocity;

        public void Start()
        {
            attitude = this.transform.eulerAngles;
        }

        public void FixedUpdate()
        {
            if (!isServer)
            {
                return;
            }

            float deltaTime = unityService.fixedDeltaTime;

            // move object by velocity
            transform.position += deltaTime * linearVelocity;
            // rotate object by rotation
            attitude += deltaTime * angularVelocity;
            // Bound all angles between 0 and 360
            attitude.x %= 360;
            attitude.y %= 360;
            attitude.z %= 360;
            transform.eulerAngles = attitude;
        }
    }
}