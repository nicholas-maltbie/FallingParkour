using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace PropHunt.Game.Level
{
    /// <summary>
    /// Library file that contains sets of all game levels
    /// </summary>
    [CreateAssetMenu(fileName = "GameLevelLibrary", menuName = "ScriptableObjects/GameLevelLibraryScriptableObject", order = 1)]
    public class GameLevelLibrary : ScriptableObject
    {
        /// <summary>
        /// Default level to load when entering the game
        /// </summary>
        [SerializeField]
        private int defaultLevelIndex;

        /// <summary>
        /// List of all available levels to select from
        /// </summary>
        [SerializeField]
        private GameLevel[] gameLevels;

        /// <summary>
        /// Has the library been initialized
        /// </summary>
        private bool Initialized => gameIdLookup.Count == gameLevels.Length;

        /// <summary>
        /// Get the default level
        /// </summary>
        public GameLevel DefaultLevel => gameLevels[defaultLevelIndex];

        /// <summary>
        /// Lookup table for level by Id
        /// </summary>
        private Dictionary<string, GameLevel> gameIdLookup =
            new Dictionary<string, GameLevel>();

        /// <summary>
        /// Resets the lookup tables for this library
        /// </summary>
        public void ClearLookups()
        {
            gameIdLookup.Clear();
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
            gameLevels.ToList().ForEach(level => gameIdLookup.Add(level.levelName, level));
        }

        /// <summary>
        /// Does a level exist for a specific name
        /// </summary>
        /// <param name="levelId">Level identifier</param>
        /// <returns>True if a level exists with this id, false otherwise</returns>
        public bool HasLevel(string levelId)
        {
            VerifyLookups();
            return gameIdLookup.ContainsKey(levelId);
        }

        /// <summary>
        /// Gest the level with an associated id
        /// </summary>
        /// <param name="levelId">Level id to lookup for</param>
        /// <returns>The level with the specified id</returns>
        public GameLevel GetLevel(string levelId)
        {
            VerifyLookups();
            return gameIdLookup[levelId];
        }

        /// <summary>
        /// Get all the game levels.
        /// </summary>
        /// <returns>Returns an enumerator of all game levels in an arbitrary order.</returns>
        public IEnumerable<GameLevel> EnumerateLevels()
        {
            VerifyLookups();
            return gameIdLookup.Values.AsEnumerable();
        }

        /// <summary>
        /// Game level lookup information
        /// </summary>
        [Serializable]
        public class GameLevel
        {
            /// <summary>
            /// Name of scene for loading the level
            /// </summary>
            [Scene]
            [SerializeField]
            public string levelName;

            /// <summary>
            /// Display name of scene to end users
            /// </summary>
            [SerializeField]
            public string displayName;
        }
    }
}
