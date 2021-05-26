using System;
using System.Collections.Generic;
using UnityEngine;

namespace PropHunt.Environment.Sound
{
    /// <summary>
    /// Library file that contains sets of sound effects
    /// </summary>
    [CreateAssetMenu(fileName = "SFXLibrary", menuName = "ScriptableObjects/SpawnSFXLibraryScriptableObject", order = 1)]
    public class SoundEffectLibrary : ScriptableObject
    {
        /// <summary>
        /// Sets of labeled sounds that can be played from this library
        /// </summary>
        [SerializeField]
        public LabeledSFX[] sounds;

        /// <summary>
        /// Has the library been initialized
        /// </summary>
        private bool initialized = false;

        /// <summary>
        /// Lookup table for sounds effects by sound material
        /// </summary>
        private Dictionary<SoundMaterial, List<LabeledSFX>> soundMaterialLookup =
            new Dictionary<SoundMaterial, List<LabeledSFX>>();

        /// <summary>
        /// Lookup table for sound effects by sound type
        /// </summary>
        private Dictionary<SoundType, List<LabeledSFX>> soundTypeLookup =
            new Dictionary<SoundType, List<LabeledSFX>>();

        /// <summary>
        /// Lookup table for sounds by material and type
        /// </summary>
        /// <returns></returns>
        private Dictionary<Tuple<SoundMaterial, SoundType>, List<LabeledSFX>> soundMaterialTypeLookup =
            new Dictionary<Tuple<SoundMaterial, SoundType>, List<LabeledSFX>>();

        /// <summary>
        /// Lookup table for sound effect by ID
        /// </summary>
        private Dictionary<string, LabeledSFX> soundIdLookup =
            new Dictionary<string, LabeledSFX>();

        /// <summary>
        /// Resets the lookup tables for this sound effect library
        /// </summary>
        public void ClearLookups()
        {
            initialized = false;
            soundMaterialLookup.Clear();
            soundTypeLookup.Clear();
            soundMaterialTypeLookup.Clear();
            soundIdLookup.Clear();
        }

        /// <summary>
        /// Verifies that lookup tables exist. If they do not, they will be created
        /// </summary>
        public void VerifyLookups()
        {
            if (!initialized)
            {
                ClearLookups();
                SetupLookups();
            }
        }

        /// <summary>
        /// Creates lookup tables based on set of saved sound effects
        /// </summary>
        public void SetupLookups()
        {
            foreach (LabeledSFX labeled in sounds)
            {
                labeled.audioClip.LoadAudioData();
                Tuple<SoundMaterial, SoundType> tupleKey = new Tuple<SoundMaterial, SoundType>(labeled.soundMaterial, labeled.soundType);
                if (!soundMaterialLookup.ContainsKey(labeled.soundMaterial))
                {
                    soundMaterialLookup[labeled.soundMaterial] = new List<LabeledSFX>();
                }
                if (!soundTypeLookup.ContainsKey(labeled.soundType))
                {
                    soundTypeLookup[labeled.soundType] = new List<LabeledSFX>();
                }
                if (!soundMaterialTypeLookup.ContainsKey(tupleKey))
                {
                    soundMaterialTypeLookup[tupleKey] = new List<LabeledSFX>();
                }
                soundMaterialLookup[labeled.soundMaterial].Add(labeled);
                soundTypeLookup[labeled.soundType].Add(labeled);
                soundMaterialTypeLookup[tupleKey].Add(labeled);
                soundIdLookup[labeled.soundId] = labeled;
            }
            initialized = true;
        }

        /// <summary>
        /// Get a random sound effect by material
        /// </summary>
        /// <param name="soundMaterial">Sound material to filter by</param>
        /// <returns>A labeled sound effect that has the given sound material</returns>
        public LabeledSFX GetSFXClipBySoundMaterial(SoundMaterial soundMaterial)
        {
            List<LabeledSFX> sounds = soundMaterialLookup[soundMaterial];
            return sounds[(int)UnityEngine.Random.Range(0, sounds.Count)];
        }

        /// <summary>
        /// Gets a random sound effect clip by type of sound
        /// </summary>
        /// <param name="soundType">Lookup based on this sound type</param>
        /// <returns>A labeled sound effect that has the given sound type</returns>
        public LabeledSFX GetSFXClipBySoundType(SoundType soundType)
        {
            List<LabeledSFX> sounds = soundTypeLookup[soundType];
            return sounds[(int)UnityEngine.Random.Range(0, sounds.Count)];
        }

        /// <summary>
        /// Gets a sound effect clip by material and type
        /// </summary>
        /// <param name="soundMaterial">Sound material to filter search</param>
        /// <param name="soundType">Sound type to filter search</param>
        /// <returns>A labeled sound effect that has the given sound and material types</returns>
        public LabeledSFX GetSFXClipBySoundMaterialAndType(SoundMaterial soundMaterial, SoundType soundType)
        {
            List<LabeledSFX> sounds = soundMaterialTypeLookup[new Tuple<SoundMaterial, SoundType>(soundMaterial, soundType)];
            return sounds[(int)UnityEngine.Random.Range(0, sounds.Count)];
        }

        /// <summary>
        /// Gests a sound effect clip by a given id
        /// </summary>
        /// <param name="soundId">identifier name to lookup the sound by</param>
        /// <returns>The labeled sound effect with a given id</returns>
        public LabeledSFX GetSFXClipById(string soundId)
        {
            return soundIdLookup[soundId];
        }

        /// <summary>
        /// Does this library contain any sound effects for a given material?
        /// </summary>
        /// <param name="soundMaterial">Sound material to lookup</param>
        /// <returns>True if any sound effects have this material, false otherwise</returns>
        public bool HasSoundEffect(SoundMaterial soundMaterial)
        {
            return soundMaterialLookup.ContainsKey(soundMaterial) && soundMaterialLookup[soundMaterial].Count > 0;
        }

        /// <summary>
        /// Does this library contain any sound effects for a given sound type
        /// </summary>
        /// <param name="soundType">Sound type to lookup</param>
        /// <returns>True if any sound effects have this type, false otherwise</returns>
        public bool HasSoundEffect(SoundType soundType)
        {
            return soundTypeLookup.ContainsKey(soundType) && soundTypeLookup[soundType].Count > 0;
        }


        /// <summary>
        /// Does this library contain any sound effects for a given sound material and type
        /// </summary>
        /// <param name="soundType">Sound type to lookup</param>
        /// <param name="soundMaterial">Sound material to lookup</param>
        /// <returns>True if any sound effects have this material and type, false otherwise</returns>
        public bool HasSoundEffect(SoundMaterial soundMaterial, SoundType soundType)
        {
            return soundMaterialTypeLookup.ContainsKey(new Tuple<SoundMaterial, SoundType>(soundMaterial, soundType)) &&
                soundMaterialTypeLookup[new Tuple<SoundMaterial, SoundType>(soundMaterial, soundType)].Count > 0;
        }

        /// <summary>
        /// Does this library contain a sound effect with a given id
        /// </summary>
        /// <param name="soundId">Sound id to lookup</param>
        /// <returns>True if there is a sound effect with this id, false otherwise</returns>
        public bool HasSoundEffect(string soundId)
        {
            return soundIdLookup.ContainsKey(soundId);
        }
    }

    /// <summary>
    /// Various kinds of sound materials for labeling sound effects
    /// </summary>
    public enum SoundMaterial
    {
        Glass,
        Wood,
        Metal,
        Concrete,
        Water,
        Misc
    }

    /// <summary>
    /// Various types of sound for labeling sound effects
    /// </summary>
    public enum SoundType
    {
        Hit,
        Break,
        Scrape,
        Roll,
        Footstep,
        Misc,
        PropTransformation,
        Attack,
        Death,
    }

    /// <summary>
    /// Labeled sound effect for categorizing and seraching sounds
    /// </summary>
    [System.Serializable]
    public class LabeledSFX
    {
        /// <summary>
        /// Type of material this sound is made by
        /// </summary>
        public SoundMaterial soundMaterial;
        /// <summary>
        /// The type of sound effect made in this clip
        /// </summary>
        public SoundType soundType;
        /// <summary>
        /// Sound effect audio clip related to this sound effect
        /// </summary>
        public AudioClip audioClip;
        /// <summary>
        /// unique identifier for this labeled sound effect
        /// </summary>
        public string soundId;
    }
}
