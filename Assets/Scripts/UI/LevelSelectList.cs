using PropHunt.Game.Level;
using PropHunt.Game.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static PropHunt.Game.Level.GameLevelLibrary;

namespace PropHunt.UI
{
    /// <summary>
    /// Events that occur when new level is selected
    /// </summary>
    [Serializable]
    public struct LevelSelectEvent
    {
        /// <summary>
        /// New game level selected
        /// </summary>
        public GameLevel level;
    }

    /// <summary>
    /// Level select list for choosing which level the game will start on.
    /// </summary>
    public class LevelSelectList : MonoBehaviour
    {
        /// <summary>
        /// Scene select button
        /// </summary>
        [SerializeField]
        private GameObject sceneButtonPrefab;

        /// <summary>
        /// Location of level select list
        /// </summary>
        [SerializeField]
        private RectTransform levelSelectListLocation;

        /// <summary>
        /// Display text for current scene selected to end users
        /// </summary>
        [SerializeField]
        private Text sceneDisplay;

        /// <summary>
        /// List of all available scenes to select from
        /// </summary>
        [SerializeField]
        private GameLevelLibrary levels;

        /// <summary>
        /// The currently selected scene
        /// </summary>
        private string selectedScene;

        /// <summary>
        /// Event for registering whenever the selected level has changed
        /// </summary>
        public static EventHandler<LevelSelectEvent> LevelSelectChange;

        public static void LevelSelect(LevelSelectEvent levelSelect)
        {
            LevelSelectChange?.Invoke(null, levelSelect);
        }

        public void LevelSelected(string newScene)
        {
            sceneDisplay.text = $"Level: {newScene}";
        }

        public void Start()
        {
            List<GameLevel> scenes = levels.EnumerateLevels().ToList();
            scenes.Sort((level1, level2) => level1.displayName.CompareTo(level2.displayName));
            Enumerable.Range(0, scenes.Count)
                .ToList()
                .ForEach(index =>
                {
                    GameLevel level = scenes[index];
                    GameObject sceneButton = GameObject.Instantiate(sceneButtonPrefab);
                    Button button = sceneButton.GetComponent<Button>();
                    Text buttonText = sceneButton.GetComponentInChildren<Text>();
                    RectTransform rectTransform = sceneButton.GetComponent<RectTransform>();

                    float offsetY = (rectTransform.rect.size.y + 5) * index + 5;
                    float rectHeight = rectTransform.rect.height;

                    buttonText.text = level.displayName;
                    sceneButton.transform.parent = levelSelectListLocation.transform;
                    rectTransform.offsetMin = new Vector2(5, -offsetY - rectHeight);
                    rectTransform.offsetMax = new Vector2(-5, -offsetY);
                    rectTransform.localScale = Vector3.one;

                    button.onClick.AddListener(() =>
                    {
                        this.selectedScene = level.levelName;
                        GameObject.FindObjectOfType<GameSceneManager>().ChangeScene(this.selectedScene);
                        GameObject.FindObjectOfType<CurrentlySelectedLevel>().UpdateLevel(level);
                    });
                });
            LevelSelectChange += (_, args) => LevelSelected(args.level.displayName);
            LevelSelected(levels.DefaultLevel.displayName);
        }
    }
}
