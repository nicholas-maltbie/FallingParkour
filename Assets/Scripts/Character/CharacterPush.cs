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

            Vector3 pushForce = force * pushPower;

            // Apply the push
            body.AddForceAtPosition(pushForce, hit.point, ForceMode.Force);
            pushable.PushObjectServerRpc(
                pushForce,
                hit.point,
                (int)ForceMode.Force,
                NetworkManager.Singleton.LocalClientId);
        }
    }
}