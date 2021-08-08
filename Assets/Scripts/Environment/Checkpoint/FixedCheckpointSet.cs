using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropHunt.Environment.Checkpoint
{
    /// <summary>
    /// Fixed set of checkpoints for spawning players at set manually by end user.
    /// </summary>
    public class FixedCheckpointSet : MonoBehaviour, ISpawnPointCollection
    {
        /// <summary>
        /// Order of this checkpoint relative to other checkpoints.
        /// </summary>
        [SerializeField]
        [Tooltip("Order of checkpoint relative to others.")]
        private int order;

        /// <summary>
        /// Sets of checkpoints that players can spawn at.
        /// </summary>
        [SerializeField]
        [Tooltip("Sets of checkpoints that players can spawn at.")]
        private Transform[] checkpoints;

        /// <inheritdoc/>
        public int Total() => checkpoints.Length;

        /// <inheritdoc/>
        public (Vector3, Quaternion) GetSpawnPoint(int index) =>
            (checkpoints[index].position, checkpoints[index].rotation);


        /// <inheritdoc/>
        public int Priority()
        {
            return order;
        }
    }
}
