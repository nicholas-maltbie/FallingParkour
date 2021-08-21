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
        private CharacterAvatar defaultAvatar;

        /// <summary>
        /// Sets of character avatars that can be loaded from this library
        /// </summary>
        [SerializeField]
        private CharacterAvatar[] playerModels;

        /// <summary>
        /// Has the library been initialized
        /// </summary>
        private bool Initialized => avatarIDLookup.Count == playerModels.Length;

        /// <summary>
        /// Get the default avatar
        /// </summary>
        public CharacterAvatar DefaultAvater => defaultAvatar;

        /// <summary>
        /// Lookup table for character avatar by ID
        /// </summary>
        private Dictionary<string, CharacterAvatar> avatarIDLookup =
            new Dictionary<string, CharacterAvatar>();

        /// <summary>
        /// Resets the lookup tables for this library
        /// </summary>
        public void ClearLookups()
        {
            avatarIDLookup.Clear();
        }

        /// <summary>
        /// Verifies that lookup tables exist. If they do not, they will be created
        /// </summary>
        public void VerifyLookups()
        {
            if (!Initialized)
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
            foreach (CharacterAvatar model in playerModels)
            {
                avatarIDLookup.Add(model.Name, model);
            }
        }

        public bool HasCharacterAvatar(string avatarId)
        {
            VerifyLookups();
            return avatarIDLookup.ContainsKey(avatarId);
        }

        public CharacterAvatar GetCharacterAvatar(string avatarId)
        {
            VerifyLookups();
            return avatarIDLookup[avatarId];
        }

        [Serializable]
        public class CharacterAvatar
        {
            [SerializeField]
            public GameObject avatar;

            [SerializeField]
            public Sprite characterSprite;

            public string Name => avatar.name;
        }
    }
}
