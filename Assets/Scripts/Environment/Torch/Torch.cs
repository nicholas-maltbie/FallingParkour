using System.Linq;
using nickmaltbie.Noise;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment.Torch
{
    /// <summary>
    /// Object that can illuminate nearby objects. 
    /// </summary>
    public class Torch : MonoBehaviour
    {
        /// <summary>
        /// Number of octaves in the noise for this torch.
        /// </summary>
        public int octaves = 8;

        /// <summary>
        /// How much does the noise decay  over each octave of noise.
        /// </summary>
        public float persistence = 0.8f;

        /// <summary>
        /// How much does the frequency grow over each octave of noise.
        /// </summary>
        public float frequencyGrowth = 1.2f;

        /// <summary>
        /// Seed value for this torch.
        /// </summary>
        public int seed = 42;

        /// <summary>
        /// Minimum range of the torch.
        /// </summary>
        public float minRange = 1.0f;

        /// <summary>
        /// Variation in the lighting of the torch beyond min range due to noise.
        /// </summary>
        public float variation = 1.0f;

        /// <summary>
        /// Falloff of lighting of torch after objects are beyond current range.
        /// </summary>
        public float falloff = 1.0f;

        /// <summary>
        /// Current range of the torch.
        /// </summary>
        private float currentRange = 0.0f;

        /// <summary>
        /// Noise function controlling the torch light.
        /// </summary>
        private OctaveNoise noise;

        public void Start()
        {
            this.noise = new OctaveNoise(new PerlinNoise(0, seed, 256), octaves, persistence, frequencyGrowth);
        }

        public void Update()
        {
            this.currentRange = this.minRange + this.noise.GetNoise(new Vector3(Time.time, 0, 0)) * this.variation;
        }

        /// <summary>
        /// Get the strength at which this torch is illuminating an element at a given position.
        /// </summary>
        /// <param name="pos">Position of element.</param>
        /// <returns>Intensity at which this torch is illuminating an element at a given timestamp.</returns>
        public float GetStrength(Vector3 pos)
        {
            // Get the distance between this torch and the element
            float dist = Vector3.Distance(pos, this.transform.position);

            // Get the current light value, if it's within range, fully light the object
            if (dist <= this.currentRange)
            {
                return 1;
            }
            // If it's within falloff, return the partially lit value.
            else if (dist - this.currentRange < this.falloff)
            {
                return 1 - ((dist - this.currentRange) / this.falloff);
            }
            // Otherwise return 0 as this is not lighting the element
            else
            {
                return 0;
            }
        }
    }
}