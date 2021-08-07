using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PropHunt.Environment.Checkpoint
{
    /// <summary>
    /// Matrix of checkpoint elements with a constant given offset
    /// in row and column.
    /// </summary>
    public class CheckpointMatrix : MonoBehaviour, ISpawnPointCollection
    {
        /// <summary>
        /// Offset along the X and Z axis.
        /// </summary>
        /// <returns></returns>
        public Vector2 offset = new Vector2(1, 1);

        /// <summary>
        /// Number of rows of spawn locations.
        /// </summary>
        public int rows = 1;

        /// <summary>
        /// Number of columns of spawn locations.
        /// </summary>
        public int cols = 1;

        /// <summary>
        /// The initial starting point of the checkpoint array.
        /// </summary>
        private Vector3 Start => transform.position;

        /// <summary>
        /// Rotation of this checkpoint array.
        /// </summary>
        private Quaternion Rotation => transform.rotation;

        /// <summary>
        /// Offset of a checkpoint for a given row and column.
        /// </summary>
        /// <param name="row">Row of the element index.</param>
        /// <param name="col">Column of the element index.</param>
        /// <returns>The offset in world space for a given row and column from the start location.</returns>
        private Vector3 Offset(int row, int col) => Rotation * new Vector3(offset.x * col, 0, offset.y * row);

        /// <summary>
        /// Get the row and column of an element from a index.
        /// </summary>
        /// <param name="index">Index of a element in the array.</param>
        /// <returns>The row and column of the element at this index.</returns>
        private (int, int) GetRowCol(int index) => (index / cols, index % cols);

        /// <summary>
        /// Gets the index of an element form a given row and column.
        /// </summary>
        /// <param name="row">Row of the element index.</param>
        /// <param name="col">Column of the element index.</param>
        /// <returns>The index of an element with a given row and column.</returns>
        private int GetIndex(int row, int col) => row * cols + col;

        /// <inheritdoc/>
        public int Total() => rows * cols;

        /// <inheritdoc/>
        public (Vector3, Quaternion) GetSpawnPoint(int index)
        {
            (int row, int col) = GetRowCol(index);
            return (Start + Offset(row, col), Rotation);
        }
    }
}
