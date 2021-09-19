using MLAPI;
using MLAPI.NetworkVariable;
using PropHunt.Game.Level;
using PropHunt.UI;
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
        [SerializeField]
        private NetworkVariable<GameLevel> currentLevel = new NetworkVariable<GameLevel>();

        /// <summary>
        /// Library of all the levels in the game
        /// </summary>
        [SerializeField]
        private GameLevelLibrary library;

        public void Start()
        {
            var manager = GameObject.FindObjectOfType<GameSceneManager>();
            if (IsServer && manager != null)
            {
                currentLevel.Value = library.GetLevel(manager.gameScene);
            }
        }

        public void OnEnable()
        {
            currentLevel.OnValueChanged += UpdateSelectedLevel;
        }

        public void OnDisable()
        {
            currentLevel.OnValueChanged -= UpdateSelectedLevel;
        }

        /// <summary>
        /// Update the currently selected level
        /// </summary>
        /// <param name="newLevel">New level that is selected</param>
        public void UpdateLevel(GameLevel newLevel)
        {
            if (IsServer)
            {
                currentLevel.Value = newLevel;
            }
        }

        public void UpdateSelectedLevel(GameLevel _, GameLevel selected)
        {
            LevelSelectList.LevelSelect(new LevelSelectEvent { level = selected });
        }
    }
}
