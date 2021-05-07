using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment.Sound
{
    /// <summary>
    /// Have an object generate sound effects when it hits something
    /// </summary>
    public class SoundEffectOnHit : NetworkBehaviour
    {
        /// <summary>
        /// Minimum velocity of a collision to create a sound effect (in units per second)
        /// </summary>
        public float minCollisionVelocity = 1.0f;

        /// <summary>
        /// Material of the given object
        /// </summary>
        public SoundMaterial soundMaterial;

        /// <summary>
        /// Minimum delay (in seconds) between making sound effects
        /// </summary>
        public float minimumDelay = 0.15f;

        /// <summary>
        /// Time that the last sound was played
        /// </summary>
        private float lastSound = Mathf.NegativeInfinity;

        /// <summary>
        /// Unity service for managing time
        /// </summary>
        public IUnityService unityService = new UnityService();

        /// <summary>
        /// Minimum pitch varaition
        /// </summary>
        [Range(-3, 3)]
        public float minPitch = 0.8f;

        /// <summary>
        /// Maximum pitch variation
        /// </summary>
        [Range(-3, 3)]
        public float maxPitch = 1.2f;

        /// <summary>
        /// Random variation in volume
        /// </summary>
        [Range(0, 1)]
        public float volumeVariation = 0.01f;

        /// <summary>
        /// Speed at which sound effect will be played at full volume
        /// </summary>
        public float maximumSpeed = 10.0f;

        public INetworkService networkService;

        public void Awake()
        {
            networkService = new NetworkService(this);
        }

        /// <summary>
        /// Process a collision event and make a sound effect if permissible
        /// </summary>
        /// <param name="collision">Collision event that ocurred</param>
        public void CollisionEvent(ICollision collision)
        {
            Vector3 point = collision.GetContact(0).point;
            float velocity = collision.relativeVelocity.magnitude;
            if (velocity < minCollisionVelocity)
            {
                return;
            }

            if ((unityService.time - lastSound) >= minimumDelay)
            {
                lastSound = unityService.time;
            }
            else
            {
                return;
            }

            float speedVolume = Mathf.Clamp(velocity / maximumSpeed, 0, 1.0f);
            float sampledVariation = Random.Range(-volumeVariation, volumeVariation);
            float volume = Mathf.Clamp(speedVolume + sampledVariation, 0, 1);

            SoundEffectManager.CreateNetworkedSoundEffectAtPoint(point,
                soundMaterial, SoundType.Hit,
                pitch: Random.Range(minPitch, maxPitch), volume: volume);
        }

        public void OnCollisionEnter(Collision other)
        {
            if (!networkService.isServer)
            {
                return;
            }
            CollisionEvent(new CollisionWrapper(other));
        }
    }
}