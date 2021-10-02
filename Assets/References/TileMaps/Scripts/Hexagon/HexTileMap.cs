using nickmaltbie.TileMap.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nickmaltbie.TileMap.Hexagon
{

    /// <summary>
    /// Fixed size hexagon grid tile map that can contain generic values at each position within the map.
    /// </summary>
    /// <typeparam name="V">Type of values contained within each cell in the grid.</typeparam>
    public class HexTileMap<V> : ITileMap<Vector2Int, V>
    {
        /// <summary>
        /// Width of the hexagon grid in tiles.
        /// </summary>
        private int width;

        /// <summary>
        /// Height of the hexagon grid in tiles.
        /// </summary>
        private int height;

        /// <summary>
        /// Values stored at each location in the hexagon grid.
        /// </summary>
        private V[,] values;

        /// <summary>
        /// Create a fixed size hexagon grid with a given width and height.
        /// </summary>
        /// <param name="width">Width of the hexagon grid.</param>
        /// <param name="height">Height of the hexagon grid.</param>
        public HexTileMap(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.values = new V[width, height];
        }

        /// <inheritdoc/>
        public V this[Vector2Int loc]
        {
            get => values[loc.x, loc.y];
            set => values[loc.x, loc.y] = value;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.values = new V[width, height];
        }

        /// <inheritdoc/>
        public int GetNeighborCount(Vector2Int loc)
        {
            return GetNeighbors(loc).Count();
        }

        /// <inheritdoc/>
        public IEnumerable<Vector2Int> GetNeighbors(Vector2Int loc)
        {
            return HexCoord.GetAdjacent(loc).Where(loc => IsInMap(loc));
        }

        /// <inheritdoc/>
        public bool IsInMap(Vector2Int loc)
        {
            return loc.x >= 0 && loc.x < width && loc.y >= 0 && loc.y < height;
        }
    }
}