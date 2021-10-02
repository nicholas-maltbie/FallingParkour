using UnityEngine;

namespace nickmaltbie.Noise {
    /// <summary>
    /// Octave noise works by combining multiple levels of another noise
    /// function. This allows for more complicated and realistic terrain.
    /// 
    /// It combines increasingly smaller amplitudes and higher frequencies
    /// of a noise function to generate complex, layered noise.
    /// 
    /// An example is a mountian range, it not only has the mountains,
    /// it also has smaller ridges, valleys. Also smaller bounders,
    /// rocks, streams. And at an even smaller level, pebbles and small
    /// divots. The idea of multi octave noise is to combine all of these
    /// layers of noise together into one function.
    /// </summary>
    public class OctaveNoise : INoise {

        /// <summary>
        /// Number of octaves to combine
        /// </summary>
        private int octaves;
        /// <summary>
        /// Persistence of amplitude from octave to next, finer octave.
        /// </summary>
        private float persistence;
        /// <summary>
        /// Growth in frequency from octave to the next, finer octave.
        /// </summary>
        private float frequencyGrowth;
        /// <summary>
        /// Function used to generate noise for various (x,y,z) coordinates.
        /// </summary>
        private INoise noiseFunction;
            
        /// <summary>
        /// Computes multiple octave combination of octaves of a noise function
        /// </summary>
        /// <param name="noise">Noise function to use for combining octaves</param>
        /// <param name="octaves">Number of octaves to apply. Can be any number greater than one.</param>
        /// <param name="persistence">Change in amplitude over each octave (decay). Can be any value between [0.0, 1.0]</param>
        /// <param name="frequencyGrowth">Growth in frequency over each octave (growth factor)</param>
        /// <returns>Returns the combination of multiple octaves of Perlin Noise.</returns>
        public OctaveNoise(INoise noise, int octaves, float persistence, float frequencyGrowth) {
            this.noiseFunction = noise;
            this.octaves = octaves;
            this.persistence = persistence;
            this.frequencyGrowth = frequencyGrowth;
        }

        /// <summary>
        /// Gets the multi octave combination of the provided noise function.
        /// </summary>
        /// <param name="position">Position in 3d space to calculate noise for</param>
        /// <returns>Returns the combination of multiple octaves of noise normalized
        /// to the range of [0.0, 1.0] </returns>
        public float GetNoise(Vector3 position) {
            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            // Used for normalizing result to 0.0 - 1.0
            float maxValue = 0;
            for(int i=0;i<this.octaves;i++) {
                total += this.noiseFunction.GetNoise(position * frequency) * amplitude;
                
                maxValue += amplitude;
                
                amplitude *= this.persistence;
                frequency *= this.frequencyGrowth;
            }
            
            return total/maxValue;
        }
    }
}
