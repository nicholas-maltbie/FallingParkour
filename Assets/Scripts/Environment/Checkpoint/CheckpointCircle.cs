using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PropHunt.Environment.Checkpoint
{
    /// <summary>
    /// Checkpoints in a circle
    /// </summary>
    public class CheckpointCircle : MonoBehaviour, ISpawnPointCollection
    {
        /// <summary>
        /// Order of this checkpoint relative to other checkpoints.
        /// </summary>
        [SerializeField]
        [Tooltip("Order of checkpoint relative to others.")]
        private int order;

        /// <summary>
        /// Distance of spawn points from the center in units.
        /// </summary>
        [SerializeField]
        [Tooltip("Distance of spawn points from the center in units.")]
        private float radius = 5;

        /// <summary>
        /// Number of spawn points to create.
        /// </summary>
        [SerializeField]
        [Tooltip("Number of spawn points to create.")]
        private int size = 10;

        /// <summary>
        /// Radius of checkpoint circle in degrees.
        /// </summary>
        [SerializeField]
        [Tooltip("Number of spawn points to create.")]
        private float degrees = 360;

        /// <summary>
        /// Should a spawn point be created at the final degree location?
        /// This should be avoided for a full circle but if only 180 degrees are used it 
        /// might be nice to have the spawn circle wrap around the full half circle.
        /// </summary>
        [SerializeField]
        [Tooltip("Should a spawn point be created at the final degree location.")]
        private bool spawnAtFinalDegree = false;

        /// <summary>
        /// Degrees offset from zero when starting checkpoint path.
        /// </summary>
        [SerializeField]
        [Tooltip("Degrees offset from zero when starting checkpoint path.")]
        private float degreeOffset = 0;

        /// <inheritdoc/>
        public int Total() => size;

        /// <inheritdoc/>
        public (Vector3, Quaternion) GetSpawnPoint(int index)
        {
            float angle = degreeOffset + ((float)index) / (Total() - (spawnAtFinalDegree ? 1 : 0)) * degrees;
            float offsetX = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float offsetZ = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            return (transform.position + new Vector3(offsetX, 0, offsetZ), Quaternion.Euler(0, (-angle - 90), 0));
        }

        /// <inheritdoc/>
        public int Priority()
        {
            return order;
        }
    }
}
