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
    public class SquareTileMap<V> : ITileMap<Vector2Int, V>
    {
        /// <summary>
        /// Width of the tile map in squares.
        /// </summary>
        private int width;

        /// <summary>
        /// Height of the tile map in squares.
        /// </summary>
        private int height;

        /// <summary>
        /// Type of adjacency for square tiles in this grid.
        /// </summary>
        private Adjacency adjacencyType;

        /// <summary>
        /// Values stored within each square of the tile map.
        /// </summary>
        private V[,] values;

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
    }
}
