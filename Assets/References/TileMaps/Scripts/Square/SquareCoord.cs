using UnityEngine;

namespace nickmaltbie.TileMap.Square
{
    /// <summary>
    /// Types of adjacency for square coordinate grid.
    /// </summary>
    public enum Adjacency
    {
        Orthogonal,
        Full
    }

    /// <summary>
    /// Utility functions for square coordinate grid. 
    /// </summary>
    public static class SquareCoord
    {
        /// <summary>
        /// Up coordinate (0, 1)
        /// </summary>
        public static Vector2Int Up = Vector2Int.up;

        /// <summary>
        /// Right coordinate (0, -1)
        /// </summary>
        public static Vector2Int Down = Vector2Int.down;

        /// <summary>
        /// Left coordinate (-1, 0)
        /// </summary>
        public static Vector2Int Left = Vector2Int.left;

        /// <summary>
        /// Right Coordinate (1, 0)
        /// </summary>
        public static Vector2Int Right = Vector2Int.right;

        /// <summary>
        /// Offset of all orthogonally adjacent tiles enumerated in counter
        /// clockwise order starting with 0 radians at (1, 0).
        /// </summary>
        public static readonly Vector2Int[] orthongoalAdj = new Vector2Int[]
        {
            Right,
            Up,
            Left,
            Down
        };

        /// <summary>
        /// Offset of all full adjacent tiles (orthogonal + diagonals) enumerated in counter
        /// clockwise order starting with 0 radians at (1, 0).
        /// </summary>
        public static readonly Vector2Int[] fullAdj = new Vector2Int[]
        {
            Right,
            Right + Up,
            Up,
            Left + Up,
            Left,
            Left + Down,
            Down,
            Right + Down,
        };
    }
}