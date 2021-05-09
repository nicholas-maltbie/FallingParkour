using System.Collections;
using UnityEngine;

namespace PropHunt.Environment.Sound
{
    /// <summary>
    /// Return audio source to sound effect manager on finish
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class ReturnAudioSourceOnFinish : MonoBehaviour
    {
        /// <summary>
        /// Source related to this audio clip
        /// </summary>
        protected AudioSource source;

        /// <summary>
        /// Is this audio clip in use
        /// </summary>
        public bool inUse = false;

        public void Start()
        {
            source = GetComponent<AudioSource>();
        }

        public void Update()
        {
            if (!source.isPlaying && inUse)
            {
                inUse = false;
                SoundEffectManager.Instance.ReturnAudioSource(source);
            }
        }
    }
}