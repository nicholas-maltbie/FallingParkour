using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PropHunt.Environment.Checkpoint
{
    /// <summary>
    /// An ordered collection of spawn locations
    /// </summary>
    public interface ISpawnPointCollection
    {
        /// <summary>
        /// total number of spawn locations in the group
        /// </summary>
        /// <returns></returns>
        int Total();

        /// <summary>
        /// Gets the transform of a spawn point for a given index
        /// </summary>
        /// <param name="index">Index of the spawn point out of the collection</param>
        /// <returns>The location and rotation of the given spawn position</returns>
        (Vector3, Quaternion) GetSpawnPoint(int index);
    }
}
