using System.Collections;
using System.Collections.Generic;
using Mirror;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.Audio;

namespace PropHunt.Environment.Sound
{
    /// <summary>
    /// Event describing a sound effect for sharing over the network
    /// </summary>
    public struct SoundEffectEvent : NetworkMessage
    {
        /// <summary>
        /// identifier for sound effect event
        /// </summary>
        public string sfxId;
        /// <summary>
        /// Point where sound effect occurs
        /// </summary>
        public Vector3 point;
        /// <summary>
        /// Pitch value for modulating sound effect
        /// </summary>
        public float pitchValue;
        /// <summary>
        /// Volume modifier for specifing the volume of the sound effect
        /// Should be between 0 and 1
        /// </summary>
        public float volume;
        /// <summary>
        /// Mixer group to play this sound effect in
        /// </summary>
        public string mixerGroup;
    }

    /// <summary>
    /// Manager for sound effects in a given scene
    /// </summary>
    public class SoundEffectManager : MonoBehaviour
    {
        /// <summary>
        /// Global instance of sound effect to play sound effects in the scene
        /// </summary>
        public static SoundEffectManager Instance;

        /// <summary>
        /// Library of sound effect events that can be played
        /// </summary>
        public SoundEffectLibrary soundEffectLibrary;

        /// <summary>
        /// Prefab that contains and audio source and various settings for audio events
        /// </summary>
        public GameObject soundEffectPrefab;

        /// <summary>
        /// Audio mixer for this project
        /// </summary>
        public AudioMixer audioMixer;

        /// <summary>
        /// Default audio mixer group to use when creating sound effects
        /// </summary>
        public const string defaultAudioMixerGroup = "SFX";

        /// <summary>
        /// Lookup for various audio mixer groups
        /// </summary>
        private Dictionary<string, AudioMixerGroup> mixerGroupLookup = new Dictionary<string, AudioMixerGroup>();

        /// <summary>
        /// Get an audio mixer group by name
        /// </summary>
        private AudioMixerGroup GetAudioMixerGroup(string name) => mixerGroupLookup[name.ToUpper()];

        /// <summary>
        /// Does this have a lookup for a given audio mixer group?
        /// </summary>
        private bool HasAudioMixerGroup(string name) => mixerGroupLookup.ContainsKey(name.ToUpper());

        /// <summary>
        /// Maximum number of sound effect sources that are generated
        /// </summary>
        public int maxSFXSources = 16;

        /// <summary>
        /// Returns the number of available sound effect sources in the pool
        /// </summary>
        public int AvailableSources => sfxPool.Count;

        /// <summary>
        /// Returns the number of currently used sound effect sources in the pool
        /// </summary>
        public int UsedSources => maxSFXSources - sfxPool.Count;

        /// <summary>
        /// Pool of available audio sources for making sound effects
        /// </summary>
        public Queue<AudioSource> sfxPool = new Queue<AudioSource>();

        public void ReturnAudioSource(AudioSource source)
        {
            source.gameObject.transform.position = Vector3.zero;
            source.gameObject.SetActive(false);
            sfxPool.Enqueue(source);
        }

        public void OnDestroy()
        {
            Instance = null;
        }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            this.soundEffectLibrary.SetupLookups();
            // Setup audio mixer group lookup
            foreach (AudioMixerGroup group in audioMixer.FindMatchingGroups(string.Empty))
            {
                mixerGroupLookup[group.name.ToUpper()] = group;
            }

            for (int i = 0; i < maxSFXSources; i++)
            {
                GameObject sfxObj = GameObject.Instantiate(soundEffectPrefab);
                AudioSource source = sfxObj.GetComponent<AudioSource>();
                sfxPool.Enqueue(source);
                sfxObj.gameObject.AddComponent<ReturnAudioSourceOnFinish>();
                sfxObj.transform.parent = transform;
                sfxObj.SetActive(false);
            }
        }

        /// <summary>
        /// Create a sound effect event on the server and send this event to all clients
        /// </summary>
        /// <param name="point">Point where the sound effect is playing</param>
        /// <param name="material">Sound material of sound effect to select</param>
        /// <param name="type">Sound type of sound effect to select</param>
        /// <param name="pitch">Pitch value of sound effect</param>
        /// <param name="volume">Volume value of sound effect</param>
        /// <param name="audioMixerGroup">Audio mixer group to play this sound effect in</param>
        [Server]
        public static void CreateNetworkedSoundEffectAtPoint(
            Vector3 point, SoundMaterial material, SoundType type, float pitch = 1.0f, float volume = 1.0f,
            string audioMixerGroup = defaultAudioMixerGroup)
        {
            if (SoundEffectManager.Instance == null || !NetworkServer.active)
            {
                return;
            }

            string sfxId = SoundEffectManager.Instance.soundEffectLibrary.
                GetSFXClipBySoundMaterialAndType(material, type).soundId;

            CreateNetworkedSoundEffectAtPoint(new SoundEffectEvent
            {
                sfxId = sfxId,
                point = point,
                pitchValue = pitch,
                volume = volume,
                mixerGroup = audioMixerGroup
            });
        }

        /// <summary>
        /// Create a sound effect event on the server and send this event to all clients
        /// </summary>
        /// <param name="sfxEvent">Sound effect event to create</param>
        [Server]
        public static void CreateNetworkedSoundEffectAtPoint(SoundEffectEvent sfxEvent)
        {
            if (SoundEffectManager.Instance == null || !NetworkServer.active)
            {
                return;
            }
            NetworkServer.SendToAll<SoundEffectEvent>(sfxEvent);
        }

        /// <summary>
        /// Create a sound effect at a given point based on a sound effect event
        /// </summary>
        /// <param name="sfxEvent">Sound effect event to create</param>
        public static void CreateSoundEffectAtPoint(SoundEffectEvent sfxEvent)
        {
            CreateSoundEffectAtPoint(sfxEvent.point,
                SoundEffectManager.Instance.soundEffectLibrary.GetSFXClipById(sfxEvent.sfxId).audioClip,
                pitchValue: sfxEvent.pitchValue, volume: sfxEvent.volume, audioMixerGroup: sfxEvent.mixerGroup);
        }

        /// <summary>
        /// Creates a sound effect at a given point. Will randomly select a sound effect
        /// with a given material and type.
        /// </summary>
        /// <param name="point">Point that sound effect is created on the map</param>
        /// <param name="soundMaterial">Type of sound material for effect</param>
        /// <param name="soundType">Type of sound effect to select from</param>
        /// <returns>The spanwed game object that will play the sound at a given point</returns>
        public static GameObject CreateSoundEffectAtPoint(Vector3 point, SoundMaterial soundMaterial, SoundType soundType)
        {
            return CreateSoundEffectAtPoint(point,
                SoundEffectManager.Instance.soundEffectLibrary.GetSFXClipBySoundMaterialAndType(soundMaterial, soundType).audioClip);
        }

        public static IEnumerator PlaySFX(AudioSource source, string audioMixerGroup)
        {
            yield return null;
            source.outputAudioMixerGroup = audioMixerGroup != null && SoundEffectManager.Instance.HasAudioMixerGroup(audioMixerGroup) ?
                SoundEffectManager.Instance.GetAudioMixerGroup(audioMixerGroup) :
                SoundEffectManager.Instance.GetAudioMixerGroup(defaultAudioMixerGroup);
            source.gameObject.SetActive(true);
            yield return null;
            source.Play();
            source.gameObject.GetComponent<ReturnAudioSourceOnFinish>().inUse = true;
            yield return null;
        }

        /// <summary>
        /// Creates a sound effect at a given point for a clip and settings.
        /// </summary>
        /// <param name="point">Point on the map to create a sound effect</param>
        /// <param name="clip">Audio clip to play at a given point</param>
        /// <param name="pitchValue">Pitch modulation for the sound effect</param>
        /// <param name="volume">Volume to play sound effect at</param>
        /// <param name="audioMixerGroup">Audio mixer group to play this sound effect in</param>
        /// <returns></returns>
        public static GameObject CreateSoundEffectAtPoint(Vector3 point,
            AudioClip clip, float pitchValue = 1.0f, float volume = 1.0f,
            string audioMixerGroup = defaultAudioMixerGroup)
        {
            if (Instance.sfxPool.Count == 0)
            {
                return null;
            }
            GameObject sfxGo = Instance.sfxPool.Dequeue().gameObject;
            sfxGo.transform.position = point;
            AudioSource source = sfxGo.GetComponent<AudioSource>();
            source.pitch = pitchValue;
            source.clip = clip;
            source.volume = volume;
            SoundEffectManager.Instance.StartCoroutine(PlaySFX(source, audioMixerGroup));
            return sfxGo;
        }
    }
}