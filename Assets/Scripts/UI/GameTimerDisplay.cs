using System;
using System.Collections.Generic;
using System.Linq;
using PropHunt.Game.Flow;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI
{
    /// <summary>
    /// Game timer to display the game phase timer
    /// </summary>
    public class GameTimerDisplay : MonoBehaviour, IScreenComponent
    {
        /// <summary>
        /// Define a threshold of time in which the color of the timer's text will change 
        /// </summary>
        [Serializable]
        public struct TimerColorChangeThreshold
        {
            /// <summary>
            /// Time remaining at which to start displaying the color in seconds
            /// </summary>
            public float time;
            /// <summary>
            /// Color in which to switch text to once the time threshold has been reached
            /// </summary>
            public Color textColor;
        }

        /// <summary>
        /// Thresholds at which to change timer color
        /// </summary>
        public List<TimerColorChangeThreshold> thresholds;

        /// <summary>
        /// Default color to display if no threshold has been passed
        /// </summary>
        public Color defaultColor;

        /// <summary>
        /// Game phase timer tag for finding game phase timers (if they exist)
        /// </summary>
        public const string gamePhaseTimerTag = "GamePhaseTimer";

        /// <summary>
        /// Text object to display remaining timer
        /// </summary>
        public Text timerText;

        /// <summary>
        /// Is the timer being currently displayed
        /// </summary>
        private bool isDisplayed = false;

        public void Start()
        {
            // Sort thresholds by time remaining (in increasing order)
            thresholds = thresholds.OrderBy(threshold => threshold.time).ToList();
        }

        public void OnScreenLoaded()
        {
            isDisplayed = true;
        }

        public void OnScreenUnloaded()
        {
            isDisplayed = false;
        }

        /// <summary>
        /// Get the game timer associated with the game phase timer tag
        /// </summary>
        private static GameTimer GetTimer()
        {
            GameObject go = GameObject.FindGameObjectWithTag(gamePhaseTimerTag);
            if (go == null)
            {
                return null;
            }
            return go.GetComponent<GameTimer>();
        }

        private Color GetThresholdColor(TimeSpan remainingTime)
        {
            // Go through each threshold and check if the time remaining has crossed the threshold
            foreach (TimerColorChangeThreshold threshold in thresholds)
            {
                if (remainingTime.TotalSeconds <= threshold.time)
                {
                    return threshold.textColor;
                }
            }
            // If no thresholds have been passed, return the default color
            return defaultColor;
        }

        public void Update()
        {
            if (isDisplayed)
            {
                GameTimer timer = GetTimer();
                if (timer != null)
                {
                    timerText.text = timer.GetTime();
                    timerText.color = GetThresholdColor(timer.Remaining);
                }
                else
                {
                    timerText.text = "";
                    timerText.color = defaultColor;
                }
            }
        }
    }
}