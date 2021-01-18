using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character
{
    /// <summary>
    /// Have a character controller push any dynamic rigidbody it hits
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(CharacterMovement))]
    public class CharacterPush : NetworkBehaviour
    {
        /// <summary>
        /// Network service for checking client server status of object
        /// </summary>
        public INetworkService networkService;

        /// <summary>
        /// Power of the player push
        /// </summary>
        public float pushPower = 2.0f;

        /// <summary>
        /// Force of pushing down when standing on objects
        /// </summary>    
        public float weight = 10.0f;

        /// <summary>
        /// Movement for this player, things like gravity and velocity
        /// </summary>
        private CharacterMovement characterMovement;

        public void Start()
        {
            this.characterMovement = this.GetComponent<CharacterMovement>();
            this.networkService = new NetworkService(this);
        }

        /// <summary>
        /// Apply a push to an object at a point with a given force
        /// </summary>
        /// <param name="hit">Object being pushed</param>
        /// <param name="force">Force applied to the object</param>
        /// <param name="point">Point at which point is applied</param>
        public void PushWithForce(GameObject hit, Vector3 force, Vector3 point)
        {
            hit.GetComponent<Rigidbody>().AddForceAtPosition(force, point);
        }

        /// <summary>
        /// Send a command tot he server to push a given object.
        /// </summary>
        /// <param name="hit">Object hit</param>
        /// <param name="force">Force of push</param>
        /// <param name="point">Point of hit on the game object</param>
        [Command]
        public void CmdPushWithForce(GameObject hit, Vector3 force, Vector3 point)
        {
            PushWithForce(hit, force, point);
        }

        public void PushObject(IControllerColliderHit hit)
        {
            if (!this.networkService.isLocalPlayer)
            {
                // exit from update if this is not the local player
                return;
            }

            // Check if the thing we hit can be pushed
            Rigidbody body = hit.rigidbody;

            // Do nothing if the object does not have a rigidbody or if
            //   the rigidbody is kinematic
            if (body == null || body.isKinematic)
            {
                return;
            }

            Vector3 force = Vector3.zero;
            // We use gravity and weight to push things down, we use
            // our velocity and push power to push things other directions
            if (hit.moveDirection.y < -0.3)
            {
                // If below us, push down
                // Only take the movement component associated with gravity
                force = Vector3.Scale(characterMovement.gravity.normalized, characterMovement.moveDirection) * pushPower;
                // Also add some force from gravity in case we're not moving down now
                force += characterMovement.gravity.normalized * weight;
            }
            else
            {
                // If to the side, use the controller velocity
                // Project movement vector onto plane defined by gravity normal (horizontal plane)
                force = Vector3.ProjectOnPlane(characterMovement.moveDirection, characterMovement.gravity) * pushPower;
            }

            // Apply the push
            if (this.networkService.isServer)
            {
                // On the server, just push it
                PushWithForce(hit.gameObject, force, hit.point);
            }
            else
            {
                // On client, send message to server to push the object
                CmdPushWithForce(hit.gameObject, force, hit.point);
            }
        }

        public void OnControllerColliderHit(ControllerColliderHit hit)
        {
            this.PushObject(new ControllerColliderHitWrapper(hit));
        }
    }
}