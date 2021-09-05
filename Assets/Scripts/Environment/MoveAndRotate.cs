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
        protected NetworkVariableVector3 angularVelocity = new NetworkVariableVector3();

        /// <summary>
        /// Does this rotation work in local or world space. If true, will rotate in local space.
        /// If false will rotate in world space.
        /// </summary>
        [SerializeField]
        [Tooltip("Does this rotation work in local or world space")]
        protected NetworkVariableBool localRotation = new NetworkVariableBool();

        /// <summary>
        /// Current rotation of the object as a euclidian degrees
        /// </summary>
        [SerializeField]
        [Tooltip("Current rotation of the object as a euclidian degrees")]
        protected NetworkVariableVector3 attitude = new NetworkVariableVector3();

        /// <summary>
        /// Linear velocity of object in units per second for each axis
        /// </summary>
        [SerializeField]
        [Tooltip("Linear velocity of object in units per second for each axis")]
        protected NetworkVariableVector3 linearVelocity = new NetworkVariableVector3();

        public void Start()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }
            if (localRotation.Value)
            {
                attitude.Value = transform.localEulerAngles;
            }
            else
            {
                attitude.Value = transform.eulerAngles;
            }
        }

        public void Update()
        {
            float deltaTime = Time.deltaTime;

            // move object by velocity
            transform.position += deltaTime * linearVelocity.Value;
            // rotate object by rotation
            attitude.Value += deltaTime * angularVelocity.Value;
            // Bound all angles between 0 and 360
            attitude.Value = new Vector3(attitude.Value.x % 360, attitude.Value.y % 360, attitude.Value.z % 360);
            if (localRotation.Value)
            {
                transform.localEulerAngles = attitude.Value;
            }
            else
            {
                transform.eulerAngles = attitude.Value;
            }
        }
    }
}