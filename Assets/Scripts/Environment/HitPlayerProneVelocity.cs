using PropHunt.Character;
using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Hit players prone when they are hit by this object and have a high enough
    /// relative velocity to the player.
    /// </summary>
    public class HitPlayerProneVelocity : MonoBehaviour
    {
        /// <summary>
        /// How long should a player be knocked prone (minimum time) after they are hit?
        /// </summary>
        [SerializeField]
        [Tooltip("Time to set player prone when hit by this.")]
        private float proneTime = 3.0f;

        /// <summary>
        /// Threshold relative velocity between the player and object to hit that
        /// give player prone.
        /// </summary>
        [SerializeField]
        [Tooltip("Threshold relative velocity required to knock the player prone.")]
        private float thresholdRelativeVelocity = 1.0f;

        /// <summary>
        /// Percent of threshold velocity this item must travel at to knock the player prone.
        /// </summary>
        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Percent of threshold velocity this object must travel at to knock the player prone.")]
        private float percentThresholdVelocity = 0.5f;

        /// <summary>
        /// Rigidbody associated with this object.
        /// </summary>
        private Rigidbody body;

        /// <summary>
        /// Network Rigidbody associated with this object
        /// </summary>
        private NetworkRigidbody networkRigidbody;

        public Vector3 GetVelocity()
        {
            if (networkRigidbody != null)
            {
                return networkRigidbody.netVelocity.Value;
            }
            if (body != null)
            {
                return body.velocity;
            }
            return Vector3.zero;
        }

        public void Awake()
        {
            this.body = GetComponent<Rigidbody>();
            this.networkRigidbody = GetComponent<NetworkRigidbody>();
        }

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
            Vector3 velocity = GetVelocity();
            if ((kcc.LinearVelocity - velocity).magnitude > thresholdRelativeVelocity &&
                (percentThresholdVelocity == 0 ||
                    velocity.magnitude > thresholdRelativeVelocity * percentThresholdVelocity))
            {
                // Knock the player prone for prone time seconds
                kcc.KnockPlayerProne(proneTime);
                Vector3 point = collision.contacts[0].point;
                collision.rigidbody.AddForceAtPosition(GetComponent<Rigidbody>().GetPointVelocity(point) * forceMod, point);
            }
        }
    }
}