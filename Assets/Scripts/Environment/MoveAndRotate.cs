using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Set parameters for a kinematic rigidbody
    /// </summary>
    public class MoveAndRotate : NetworkBehaviour
    {
        /// <summary>
        /// Angular velocity of object in degrees per second for each euclidian axis
        /// </summary>
        [SerializeField]
        [Tooltip("Angular velocity of object in degrees per second for each euclidian axis")]
        protected NetworkVariable<Vector3> angularVelocity = new NetworkVariable<Vector3>();

        /// <summary>
        /// Does this rotation work in local or world space. If true, will rotate in local space.
        /// If false will rotate in world space.
        /// </summary>
        [SerializeField]
        [Tooltip("Does this rotation work in local or world space")]
        protected NetworkVariable<bool> localRotation = new NetworkVariable<bool>();

        /// <summary>
        /// Linear velocity of object in units per second for each axis
        /// </summary>
        [SerializeField]
        [Tooltip("Linear velocity of object in units per second for each axis")]
        protected NetworkVariable<Vector3> linearVelocity = new NetworkVariable<Vector3>();

        public void Start()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }
        }

        public void Update()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }
            float deltaTime = Time.deltaTime;

            // move object by velocity
            transform.position += deltaTime * linearVelocity.Value;
            // rotate object by rotation
            if (localRotation.Value)
            {
                transform.localRotation = transform.localRotation * Quaternion.Euler(deltaTime * angularVelocity.Value);
            }
            else
            {
                transform.rotation = transform.rotation * Quaternion.Euler(deltaTime * angularVelocity.Value);
            }
        }
    }
}