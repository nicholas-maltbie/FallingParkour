using System;
using System.Collections.Generic;
using UnityEngine;

namespace PropHunt.Character.Avatar
{
    /// <summary>
    /// Library file that contains sets of character avatars
    /// </summary>
    [CreateAssetMenu(fileName = "AvatarLibrary", menuName = "ScriptableObjects/CharacterAvatarLibraryScriptableObject", order = 1)]
    public class CharacterAvatarLibrary : ScriptableObject
    {
        /// <summary>
        /// Default avatar to give to players when no other is provided
        /// </summary>
        [SerializeField]
        private GameObject defaultAvatar;

        /// <summary>
        /// Sets of character avatars that can be loaded from this library
        /// </summary>
        [SerializeField]
        private GameObject[] playerModels;

        /// <summary>
        /// Has the library been initialized
        /// </summary>
        private bool initialized = false;

        /// <summary>
        /// Get the default avatar
        /// </summary>
        public GameObject DefaultAvater => defaultAvatar;

        /// <summary>
        /// Lookup table for character avatar by ID
        /// </summary>
        private Dictionary<string, GameObject> avatarIDLookup =
            new Dictionary<string, GameObject>();

        /// <summary>
        /// Resets the lookup tables for this library
        /// </summary>
        public void ClearLookups()
        {
            initialized = false;
            avatarIDLookup.Clear();
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
            foreach (GameObject model in playerModels)
            {
                avatarIDLookup.Add(model.name, model);
            }
            initialized = true;
        }

        public bool HasCharacterAvatar(string avatarId)
        {
            VerifyLookups();
            return avatarIDLookup.ContainsKey(avatarId);
        }

        public GameObject GetCharacterAvatar(string avatarId)
        {
            VerifyLookups();
            return avatarIDLookup[avatarId];
        }
    }
}
