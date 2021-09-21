using PropHunt.Character;
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
        /// How long should a player be knocked prone (maximum time) after they are hit?
        /// </summary>
        [SerializeField]
        [Tooltip("Time to set player prone when hit by this.")]
        private float maxProneTime = 3.0f;

        /// <summary>
        /// How long should a player be knocked prone (minimum time) after they are hit?
        /// </summary>
        [SerializeField]
        [Tooltip("Minimum time player is prone after being hit.")]
        private float minProneTime = 0.1f;

        public void OnCollisionEnter(Collision collision)
        {
            PushPlayer(collision);
        }

        public void PushPlayer(Collision collision, float forceMod = 1.2f)
        {

            if (collision.gameObject == null)
            {
                return;
            }
            KinematicCharacterController kcc = collision.gameObject.GetComponent<KinematicCharacterController>();
            if (kcc == null || !kcc.IsLocalPlayer)
            {
                return;
            }

            // Knock the player prone for prone time seconds
            kcc.KnockPlayerProne(minProneTime, maxProneTime);
            Vector3 point = collision.contacts[0].point;
            collision.rigidbody.AddForceAtPosition(GetComponent<Rigidbody>().GetPointVelocity(point) * forceMod, point, ForceMode.Impulse);
        }
    }
}