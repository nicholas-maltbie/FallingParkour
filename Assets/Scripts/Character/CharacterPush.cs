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
        /// Cooldown between push events for this player.
        /// </summary>
        public float pushCooldown = 0.1f;

        /// <summary>
        /// Previous time that the player pushed the object.
        /// </summary>
        private float previousPushTime = Mathf.NegativeInfinity;

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
            // If to the side, use the controller velocity
            // Project movement vector onto plane defined by gravity normal (horizontal plane)
            force = Vector3.ProjectOnPlane(hit.moveDirection, Vector3.down) * pushPower;

            if (Time.time - previousPushTime <= pushCooldown)
            {
                return;
            }
            previousPushTime = Time.time;
            // Apply the push
            body.AddForce(force * pushPower, ForceMode.Force);
            pushable.PushObjectServerRpc(
                force * pushPower,
                hit.point,
                (int)ForceMode.Force,
                NetworkManager.Singleton.LocalClientId);
        }
    }
}