using UnityEngine;

namespace PropHunt.Environment.Teleport
{
    /// <summary>
    /// Object that can be teleported to a static location.
    /// </summary>
    public class StaticTeleportable : MonoBehaviour, ITeleportable
    {
        /// <summary>
        /// Teleport location for this object.
        /// </summary>
        public Transform givenTeleportLoc;

        /// <summary>
        /// has this object been initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// What location is this object being teleported to.
        /// </summary>
        private Vector3 teleportLoc;

        /// <summary>
        /// What attitude will this object have upon teleporting.
        /// </summary>
        private Quaternion teleportRot;

        public void Awake()
        {
            if (!this.initialized)
            {
                if (givenTeleportLoc == null)
                {
                    SetTeleportPosition(this.transform.position, this.transform.rotation);
                }
                else
                {
                    SetTeleportPosition(givenTeleportLoc.position, givenTeleportLoc.rotation);
                }
            }
        }

        /// <summary>
        /// Set the location of teleport for this object.
        /// </summary>
        /// <param name="pos">Position to give object upon teleport.</param>
        /// <param name="rot">Rotation to give object upon teleport.</param>
        public void SetTeleportPosition(Vector3 pos, Quaternion rot)
        {
            this.teleportLoc = pos;
            this.teleportRot = rot;
            this.initialized = true;
        }

        /// <inheritdoc/>
        public void Teleport(GameObject source)
        {
            this.transform.position = this.teleportLoc;
            this.transform.rotation = this.teleportRot;
            var body = GetComponent<Rigidbody>();
            if (body != null)
            {
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }
        }
    }
}