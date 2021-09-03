using Mirror;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Hit players prone when they are hit by this object.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class HitPlayerProne : MonoBehaviour
    {
        /// <summary>
        /// How long should a player be knocked prone (minimum time) after they are hit?
        /// </summary>
        [SerializeField]
        [Tooltip("Time to set player prone when hit by this.")]
        private float proneTime = 3.0f;

        public void OnCollisionEnter(Collision collision)
        {
            PushPlayer(collision);
        }

        public void PushPlayer(Collision collision)
        {
            
            if (collision.gameObject == null)
            {
                return;
            }
            KinematicCharacterController kcc = collision.gameObject.GetComponent<KinematicCharacterController>();
            if (kcc == null || !kcc.isLocalPlayer)
            {
                return;
            }

            // Knock the player prone for prone time seconds
            kcc.KnockPlayerProne(proneTime);
            Vector3 point = collision.contacts[0].point;
            collision.rigidbody.AddForceAtPosition(GetComponent<Rigidbody>().GetPointVelocity(point), point);
        }
    }
}