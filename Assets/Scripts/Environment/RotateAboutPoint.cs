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

        protected Vector3 pos;

        protected Rigidbody objRigidbody;

        public void Start()
        {
            pos = transform.position;
            objRigidbody = GetComponent<Rigidbody>();
        }

        public void Update()
        {
            transform.position = pos;
            objRigidbody.centerOfMass = Vector3.zero;
            objRigidbody.velocity = Vector3.zero;
            objRigidbody.angularVelocity = Mathf.Deg2Rad * angularVelocity;
        }
    }
}