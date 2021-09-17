using UnityEngine;

namespace PropHunt.Environment.Teleport
{
    /// <summary>
    /// Box that will send a <see cref="Teleportable"/> items to their specificed
    /// location upon entering this object's trigger collider.
    /// </summary>
    public class TeleportBox : MonoBehaviour
    {
        /// <summary>
        /// Whenever something collides with this object, teleport it
        /// </summary>
        /// <param name="other">Other object that collided with this</param>
        public void OnTriggerEnter(Collider other)
        {
            ITeleportable teleportable = other.GetComponent<ITeleportable>();
            if (teleportable != null)
            {
                teleportable.Teleport(gameObject);
            }
        }
    }
}