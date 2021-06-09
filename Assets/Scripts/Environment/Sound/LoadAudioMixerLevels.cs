using UnityEngine;
using UnityEngine.Audio;

namespace PropHunt.Environment.Sound
{
    /// <summary>
    /// Load audio mixer levels from player preferences on startup
    /// </summary>
    public class LoadAudioMixerLevels : MonoBehaviour
    {
        /// <summary>
        /// Key to store volume under
        /// </summary>
        public const string soundVolumePrefixPlayerPrefKey = "SoundVolume_";

        /// <summary>
        /// Mixer groups to load volume for (if one is saved)
        /// </summary>
        public AudioMixerGroup[] mixerGroups;

        public void Start()
        {
            // Setup sliders
            foreach (AudioMixerGroup mixerGroup in mixerGroups)
            {
                string soundKey = soundVolumePrefixPlayerPrefKey + mixerGroup.name;
                string parameter = $"{mixerGroup.name} Volume";
                mixerGroup.audioMixer.GetFloat(parameter, out float defaultVolume);
                mixerGroup.audioMixer.SetFloat(parameter, PlayerPrefs.GetFloat(soundKey, defaultVolume));
            }
        }
    }
}
