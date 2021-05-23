using PropHunt.Environment.Sound;
using UnityEngine;

namespace PropHunt.Character.Footstep
{
    [RequireComponent(typeof(KinematicCharacterController))]
    public class AttachedFootstepSound : TimedPlayerFootstepSound
    {
        /// <summary>
        /// Audio source where sound effects are coming from
        /// </summary>
        [SerializeField]
        public AudioSource audioSource;

        public override void PlayFootstepSound(SoundEffectEvent sfxEvent)
        {
            audioSource.Stop();
            audioSource.pitch = sfxEvent.pitchValue;
            audioSource.PlayOneShot(SoundEffectManager.Instance.soundEffectLibrary.GetSFXClipById(sfxEvent.sfxId).audioClip, sfxEvent.volume);
            audioSource.Play();
        }
    }
}
