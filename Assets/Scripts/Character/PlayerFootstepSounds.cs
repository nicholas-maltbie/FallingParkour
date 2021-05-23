using Mirror;
using PropHunt.Animation;
using PropHunt.Environment.Sound;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character
{
    /// <summary>
    /// Create footstep sounds based on player animation
    /// </summary>
    [RequireComponent(typeof(KinematicCharacterController))]
    public class PlayerFootstepSounds : NetworkBehaviour
    {
        /// <summary>
        /// Foot grounded component for detecting footsteps
        /// </summary>
        public PlayerFootGrounded footGrounded;

        /// <summary>
        /// Kinematic character conttroller for player movement
        /// </summary>
        private KinematicCharacterController kcc;

        /// <summary>
        /// Volume of footsteps when sprinting
        /// </summary>
        public float sprintVolume = 0.95f;

        /// <summary>
        /// Volume of footsteps when walking
        /// </summary>
        public float walkVolume = 0.45f;

        /// <summary>
        /// Minimum pitch modulation for footsteps
        /// </summary>
        public float minPitchRange = 0.95f;

        /// <summary>
        /// Maximum pitch modulation for footsteps
        /// </summary>
        public float maxPitchRange = 1.05f;

        /// <summary>
        /// Maximum time while walking between footstep sounds
        /// </summary>
        public float maxFootstepSoundDelay = 0.75f;

        /// <summary>
        /// Minimum delay between footstep sounds
        /// </summary>
        public float minFootstepSoundDelay = 0.25f;

        /// <summary>
        /// How long has the player been walking but not made a footsep sound
        /// </summary>
        private float elapsedWalkingSilent = 0.0f;

        private float lastFootstep = Mathf.NegativeInfinity;

        public INetworkService networkService;
        public IUnityService unityService = new UnityService();

        public void Awake()
        {
            this.networkService = new NetworkService(this);
        }

        public void Start()
        {
            this.kcc = GetComponent<KinematicCharacterController>();
            footGrounded.PlayerFootstep += HandleFootstepEvent;
        }

        private SoundMaterial GetSoundMaterial(GameObject gameObject)
        {
            return SoundMaterial.Concrete;
        }

        public void HandleFootstepEvent(object sender, FootstepEvent footstepEvent)
        {
            if (!networkService.isLocalPlayer || footstepEvent.state != FootstepState.Down || (unityService.time - lastFootstep) < minFootstepSoundDelay)
            {
                return;
            }

            MakeFootstepAtPoint(footstepEvent.footstepPosition, footstepEvent.floor);
        }

        public void Update()
        {
            if (!this.networkService.isLocalPlayer)
            {
                return;
            }

            // If the player is on the ground and not moving, update the elapsed walking time
            if (!kcc.Falling && kcc.InputMovement.magnitude > 0)
            {
                elapsedWalkingSilent += unityService.deltaTime;
            }
            else
            {
                elapsedWalkingSilent = 0;
            }

            if (elapsedWalkingSilent >= maxFootstepSoundDelay)
            {
                MakeFootstepAtPoint(transform.position, kcc.floor);
            }
        }

        private void MakeFootstepAtPoint(Vector3 point, GameObject ground)
        {
            lastFootstep = unityService.time;
            elapsedWalkingSilent = 0.0f;
            SoundEffectEvent sfxEvent = new SoundEffectEvent
            {
                sfxId = SoundEffectManager.Instance.soundEffectLibrary.GetSFXClipBySoundMaterialAndType(
                    GetSoundMaterial(ground),
                    SoundType.Footstep).soundId,
                point = point,
                pitchValue = Random.Range(minPitchRange, maxPitchRange),
                volume = kcc.Sprinting ? sprintVolume : walkVolume,
                mixerGroup = "Footsteps"
            };
            SoundEffectManager.CreateSoundEffectAtPoint(sfxEvent);
            if (this.networkService.isServer)
            {
                RpcCreateFootstepSound(sfxEvent);
            }
            else
            {
                CmdCreateFootstepSound(sfxEvent);
            }
        }

        [Command]
        public void CmdCreateFootstepSound(SoundEffectEvent sfxEvent)
        {
            RpcCreateFootstepSound(sfxEvent);
        }

        [ClientRpc]
        public void RpcCreateFootstepSound(SoundEffectEvent sfxEvent)
        {
            if (this.networkService.isLocalPlayer)
            {
                return;
            }
            SoundEffectManager.CreateSoundEffectAtPoint(sfxEvent);
        }
    }
}