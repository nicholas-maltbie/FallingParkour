using System;
using Mirror;
using PropHunt.Game.Level;
using PropHunt.UI;
using PropHunt.Utils;
using UnityEngine;
using static PropHunt.Game.Level.GameLevelLibrary;

namespace PropHunt.Game.Flow
{
    /// <summary>
    /// Currently selected game level synchronization
    /// </summary>
    public class CurrentlySelectedLevel : NetworkBehaviour
    {
        /// <summary>
        /// Currently selected level for the game
        /// </summary>
        [SyncVar(hook = nameof(UpdateSelectedLevel))]
        [SerializeField]
        private GameLevel currentLevel;

        /// <summary>
        /// Library of all the levels in the game
        /// </summary>
        [SerializeField]
        private GameLevelLibrary library;

        public void Start()
        {
            var manager = GameObject.FindObjectOfType<GameSceneManager>();
            if (manager != null)
            {
                currentLevel = library.GetLevel(manager.gameScene);
            }
        }

        /// <summary>
        /// Update the currently selected level
        /// </summary>
        /// <param name="newLevel">New level that is selected</param>
        [Server]
        public void UpdateLevel(GameLevel newLevel)
        {
            currentLevel = newLevel;
        }

        public void UpdateSelectedLevel(GameLevel _, GameLevel selected)
        {
            LevelSelectList.LevelSelect( new LevelSelectEvent{ level = selected } );
        }
    }
}
