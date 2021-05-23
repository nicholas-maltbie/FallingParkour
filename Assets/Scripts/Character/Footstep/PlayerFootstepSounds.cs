using Mirror;
using PropHunt.Animation;
using PropHunt.Environment.Sound;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character.Footstep
{
    /// <summary>
    /// Create footstep sounds based on player animation
    /// </summary>
    public class PlayerFootstepSounds : TimedPlayerFootstepSound
    {
        /// <summary>
        /// Foot grounded component for detecting footsteps
        /// </summary>
        public PlayerFootGrounded footGrounded;

        public override void Start()
        {
            base.Start();
            footGrounded.PlayerFootstep += HandleFootstepEvent;
        }

        public void HandleFootstepEvent(object sender, FootstepEvent footstepEvent)
        {
            if (!networkService.isLocalPlayer || footstepEvent.state != FootstepState.Down || (unityService.time - lastFootstep) < minFootstepSoundDelay)
            {
                return;
            }

            MakeFootstepAtPoint(footstepEvent.footstepPosition, footstepEvent.floor);
        }
    }
}