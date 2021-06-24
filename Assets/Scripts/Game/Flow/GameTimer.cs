using System;
using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Game.Flow
{
    /// <summary>
    /// GameTimer that can be synchronized between clients and server
    /// </summary>
    public class GameTimer : NetworkBehaviour
    {
        /// <summary>
        /// Event handler invoked when the fim
        /// </summary>
        public event EventHandler OnFinish;

        /// <summary>
        /// Unity service for managing unity static operations
        /// </summary>
        public IUnityService unityService = new UnityService();

        /// <summary>
        /// Amount of time in seconds that has passed since the timer started
        /// </summary>
        [SyncVar]
        [SerializeField]
        private float elapsed;

        /// <summary>
        /// Amount of time that the timer will run for (in seconds)
        /// </summary>
        [SyncVar]
        [SerializeField]
        private float length;

        /// <summary>
        /// Is this timer currently running
        /// </summary>
        public bool Running { get; private set; }

        /// <summary>
        /// Has this given timer completed
        /// </summary>
        public bool Finished => elapsed >= length;

        /// <summary>
        /// What is the time remaining in this timer
        /// </summary>
        public TimeSpan Remaining => Finished ? TimeSpan.Zero : TimeSpan.FromSeconds(length - elapsed);

        /// <summary>
        /// Get a human readable version of the time remaining time in the timer
        /// </summary>
        public string GetTime()
        {
            TimeSpan remaining = Remaining;
            return remaining.Minutes > 0 ? remaining.ToString(@"mm\:ss") : remaining.ToString(@"ss\.f");
        }

        /// <summary>
        /// Start this given timer on the server and resets the elapsed time to zero
        /// </summary>
        /// <param name="length">Length of timer to start</param>
        public void StartTimer(float length)
        {
            this.length = length;
            StartTimer();
        }

        /// <summary>
        /// Start this given timer on the server and resets the elapsed time to zero
        /// </summary>
        [Server]
        public void StartTimer()
        {
            this.elapsed = 0.0f;
            Running = true;
        }

        /// <summary>
        /// Pauses but saves the current elapsed time of a timer
        /// </summary>
        [Server]
        public void PauseTimer()
        {
            Running = false;
        }

        /// <summary>
        /// Resumes a timer's operation
        /// </summary>
        public void ResumeTimer()
        {
            Running = true;
        }

        /// <summary>
        /// Stop the given timer from running
        /// </summary>
        [Server]
        public void StopTimer()
        {
            this.elapsed = 0.0f;
            Running = false;
        }

        public void Update()
        {
            if (Running && !Finished)
            {
                this.elapsed += unityService.deltaTime;
                if (Finished)
                {
                    // Notify listeners of completed timer
                    OnFinish?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
