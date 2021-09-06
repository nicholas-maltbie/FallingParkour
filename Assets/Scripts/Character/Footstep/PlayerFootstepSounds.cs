using PropHunt.Animation;
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
            if (footGrounded != null)
            {
                footGrounded.PlayerFootstep += HandleFootstepEvent;
            }
        }

        public void HandleFootstepEvent(object sender, FootstepEvent footstepEvent)
        {
            if (!base.IsLocalPlayer ||
                footstepEvent.state != FootstepState.Down ||
                (Time.time - lastFootstep) < minFootstepSoundDelay)
            {
                return;
            }

            MakeFootstepAtPoint(footstepEvent.footstepPosition, footstepEvent.floor);
        }
    }
}