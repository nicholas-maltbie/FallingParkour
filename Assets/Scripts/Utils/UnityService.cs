using UnityEngine;

namespace PropHunt.Utils
{
    /// <summary>
    /// Interface for managing static information from unity classes.
    /// </summary>
    public interface IUnityService
    {
        /// <summary>
        /// Get the current delta time in seconds between this and last frame
        /// </summary>
        /// <returns>The current delta time between this and the previous frame</returns>
        float deltaTime { get; }

        /// <summary>
        /// Gets the current time in seconds since start of the game
        /// </summary>
        /// <value>Time in seconds since the start of the game</value>
        float time { get; }

        /// <summary>
        /// Get the raw axis movement for a given axis (by name)
        /// </summary>
        /// <param name="axisName">Name of selected axis</param>
        /// <returns>Movement along axis as a float</returns>
        float GetAxis(string axisName);

        /// <summary>
        /// Get when a button goes down during this frame
        /// </summary>
        /// <param name="buttonName">Name of the button (identifier)</param>
        /// <returns>True if pressed this frame, false otherwise</returns>
        bool GetButtonDown(string buttonName);

        /// <summary>
        /// Gets the current state of a button.
        /// </summary>
        /// <param name="buttonName">Name of the button (identifier)</param>
        /// <returns>True if the button is down, false if the button is up</returns>
        bool GetButton(string buttonName);
    }

    /// <summary>
    /// Default implementation of unity service
    /// </summary>
    public class UnityService : IUnityService
    {
        /// <inheritdoc/>
        public float deltaTime => Time.deltaTime;

        /// <inheritdoc/>
        public float time => Time.time;

        /// <inheritdoc/>
        public float GetAxis(string axisName)
        {
            return Input.GetAxis(axisName);
        }

        /// <inheritdoc/>
        public bool GetButtonDown(string buttonName)
        {
            return Input.GetButtonDown(buttonName);
        }

        /// <inheritdoc/>
        public bool GetButton(string buttonName)
        {
            return Input.GetButton(buttonName);
        }
    }
}