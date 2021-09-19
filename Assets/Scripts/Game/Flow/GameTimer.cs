using System;
using MLAPI;
using MLAPI.NetworkVariable;
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
        /// Amount of time in seconds that has passed since the timer started
        /// </summary>
        [SerializeField]
        private NetworkVariable<float> elapsed = new NetworkVariable<float>();

        /// <summary>
        /// Amount of time that the timer will run for (in seconds)
        /// </summary>
        [SerializeField]
        private NetworkVariable<float> length = new NetworkVariable<float>();

        /// <summary>
        /// Is this timer currently running
        /// </summary>
        public bool Running { get; private set; }

        /// <summary>
        /// Has this given timer completed
        /// </summary>
        public bool Finished => elapsed.Value >= length.Value;

        /// <summary>
        /// What is the time remaining in this timer
        /// </summary>
        public TimeSpan Remaining => Finished ? TimeSpan.Zero : TimeSpan.FromSeconds(length.Value - elapsed.Value);

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
            if (IsServer)
            {
                this.length.Value = length;
                StartTimer();
            }
        }

        /// <summary>
        /// Start this given timer on the server and resets the elapsed time to zero
        /// </summary>
        public void StartTimer()
        {
            if (IsServer)
            {
                this.elapsed.Value = 0.0f;
                this.Running = true;
            }
        }

        /// <summary>
        /// Pauses but saves the current elapsed time of a timer
        /// </summary>
        public void PauseTimer()
        {
            this.Running = false;
        }

        /// <summary>
        /// Resumes a timer's operation
        /// </summary>
        public void ResumeTimer()
        {
            this.Running = true;
        }

        /// <summary>
        /// Stop the given timer from running
        /// </summary>
        public void StopTimer()
        {
            if (IsServer)
            {
                this.elapsed.Value = 0.0f;
                this.Running = false;
            }
        }

        public void Update()
        {
            if (this.Running && !this.Finished && IsServer)
            {
                this.elapsed.Value += Time.deltaTime;
                if (this.Finished)
                {
                    // Notify listeners of completed timer
                    OnFinish?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
