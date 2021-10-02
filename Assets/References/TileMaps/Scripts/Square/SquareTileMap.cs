using nickmaltbie.TileMap.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nickmaltbie.TileMap.Square
{
    /// <summary>
    /// Fixed size square grid tile map that can contain generic values at each position within the map.
    /// </summary>
    /// <typeparam name="V">Type of values contained within each cell in the grid.</typeparam>
    public class SquareTileMap<V> : IBlockableTileMap<Vector2Int, V>
    {
        /// <summary>
        /// Width of the tile map in squares.
        /// </summary>
        private readonly int width;

        /// <summary>
        /// Height of the tile map in squares.
        /// </summary>
        private readonly int height;

        /// <summary>
        /// Type of adjacency for square tiles in this grid.
        /// </summary>
        private readonly Adjacency adjacencyType;

        /// <summary>
        /// Values stored within each square of the tile map.
        /// </summary>
        private V[,] values;

        /// <summary>
        /// Tiles within the map that are blocked.
        /// </summary>
        private readonly HashSet<Vector2Int> blocked;

        /// <summary>
        /// Initialize a tile map with a given width and height.
        /// </summary>
        /// <param name="width">Width of the tile map in number of squares.</param>
        /// <param name="height">Height of the tile map in number of squares.</param>
        /// <param name="adjacencyType">Type of adjacency within the tile map.</param>
        public SquareTileMap(int width, int height, Adjacency adjacencyType)
        {
            this.width = width;
            this.height = height;
            this.values = new V[width, height];
            this.adjacencyType = adjacencyType;
            this.blocked = new HashSet<Vector2Int>();
        }

        /// <inheritdoc/>
        public V this[Vector2Int loc]
        {
            get => values[loc.x, loc.y];
            set => values[loc.x, loc.y] = value;
        }

        /// <inheritdoc/>
        public int GetNeighborCount(Vector2Int loc)
        {
            return GetNeighbors(loc).Count();
        }

        /// <inheritdoc/>
        public IEnumerable<Vector2Int> GetNeighbors(Vector2Int loc)
        {
            switch (adjacencyType)
            {
                case Adjacency.Full:
                    return SquareCoord.fullAdj.Select(adj => loc + adj).Where(loc => IsInMap(loc));
                case Adjacency.Orthogonal:
                default:
                    return SquareCoord.orthongoalAdj.Select(adj => loc + adj).Where(loc => IsInMap(loc));
            }
        }

        /// <inheritdoc/>
        public bool IsInMap(Vector2Int loc)
        {
            return loc.x >= 0 && loc.x < width && loc.y >= 0 && loc.y < height;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.values = new V[width, height];
        }

        /// <inheritdoc/>
        public IEnumerator<Vector2Int> GetEnumerator()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }

        /// <inheritdoc/>
        public void Block(Vector2Int loc)
        {
            this.blocked.Add(loc);
        }

        /// <inheritdoc/>
        public void Unblock(Vector2Int loc)
        {
            this.blocked.Remove(loc);
        }

        /// <inheritdoc/>
        public bool IsBlocked(Vector2Int loc)
        {
            return this.blocked.Contains(loc);
        }
    }
}
