using MLAPI;
using PropHunt.Environment.Pushable;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character
{
    /// <summary>
    /// Have a character controller push any dynamic rigidbody it hits
    /// </summary>
    public class CharacterPush : NetworkBehaviour
    {
        /// <summary>
        /// Power of the player push
        /// </summary>
        public float pushPower = 2.0f;

        /// <summary>
        /// Force of pushing down when standing on objects
        /// </summary>    
        public float weight = 10.0f;

        public void PushObject(IControllerColliderHit hit)
        {
            if (!this.IsLocalPlayer)
            {
                // exit from update if this is not the local player
                return;
            }

            // Check if the thing we hit can be pushed
            Rigidbody body = hit.rigidbody;
            IPushable pushable = hit.gameObject.GetComponent<IPushable>();

            // Do nothing if the object does not have a rigidbody or if
            //   the rigidbody is kinematic
            if (body == null || body.isKinematic || pushable == null)
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
                force = Vector3.Scale(Vector3.down, hit.moveDirection) * pushPower;
                // Also add some force from gravity in case we're not moving down now
                force += Vector3.down * weight;
            }
            else
            {
                // If to the side, use the controller velocity
                // Project movement vector onto plane defined by gravity normal (horizontal plane)
                force = Vector3.ProjectOnPlane(hit.moveDirection, Vector3.down) * pushPower;
            }

            // Apply the push
            pushable.PushObjectServerAndLocal(force * Time.deltaTime * pushPower, hit.point, ForceMode.Impulse);
        }
    }
}