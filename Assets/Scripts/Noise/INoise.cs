using UnityEngine;

namespace nickmaltbie.Noise
{
    /// <summary>
    /// Noise function to generate noise values for a three dimensional space.
    /// </summary>
    public interface INoise
    {
        /// <summary>
        /// Will return a noise value between [0.0, 1.0] for a given (X,Y,Z) coordinate.
        /// </summary>
        /// <param name="position">Position in (X,Y,Z) space</param>
        /// <returns>The noise value at that coordinate, a float between [0.0, 1.0]</returns>
        float GetNoise(Vector3 position);
    }
}
