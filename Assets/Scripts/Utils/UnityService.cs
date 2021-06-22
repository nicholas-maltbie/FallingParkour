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
        /// Get the current fixed delta time for physics based update
        /// </summary>
        /// <returns>the delta time shown by the fixed delta time</returns>
        float fixedDeltaTime { get; }

        /// <summary>
        /// Gets the current time in seconds since start of the game
        /// </summary>
        /// <value>Time in seconds since the start of the game</value>
        float time { get; }
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
        public float fixedDeltaTime => Time.fixedDeltaTime;
    }
}