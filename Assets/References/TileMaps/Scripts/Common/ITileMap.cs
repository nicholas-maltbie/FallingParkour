using System.Collections.Generic;

namespace nickmaltbie.TileMap.Common
{
    /// <summary>
    /// A tile map for a given coordinate system that contains specific values.
    /// </summary>
    /// <typeparam name="K">Coordinate system for the map.</typeparam>
    /// <typeparam name="V">Values held in the map.</typeparam>
    public interface ITileMap<K, V>
    {
        /// <summary>
        /// Get the neighbors of a given location on the map
        /// </summary>
        /// <param name="loc">Location on the map to find neighbors of.</param>
        /// <returns>The locations of neighbors on the map</returns>
        IEnumerable<K> GetNeighbors(K loc);

        /// <summary>
        /// Get the number of neighbors at a given location in the map.
        /// </summary>
        /// <param name="loc">Location on the map to compute neighbors for.</param>
        /// <returns>The number of neighbors that a given tile has.</returns>
        int GetNeighborCount(K loc);

        /// <summary>
        /// Check if a given location is within the bounds of the map.
        /// </summary>
        /// <param name="loc">Location to check if it is within bounds.</param>
        /// <returns>True if the location is in the map, false otherwise.</returns>
        bool IsInMap(K loc);

        /// <summary>
        /// Modify the value saved at a specific location within the map.
        /// </summary>
        V this[K loc]
        {
            get;
            set;
        }

        /// <summary>
        /// Clear all values from the tile grid.
        /// </summary>
        void Clear();
    }
}
