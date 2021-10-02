using UnityEngine;

namespace nickmaltbie.TileMap.Common
{
    /// <summary>
    /// World grid to control and load a TileMap into world space.
    /// </summary>
    /// <typeparam name="K">Coordinate system for the map.</typeparam>
    /// <typeparam name="V">Values held in the map.</typeparam>
    public interface IWorldGrid<K, V>
    {
        /// <summary>
        /// Gets the tile map associated with this world grid.
        /// </summary>
        /// <returns></returns>
        ITileMap<K, V> GetTileMap();

        /// <summary>
        /// Gets the world position of a given coordinate within the tile map.
        /// </summary>
        /// <param name="loc">Location within the tile map to lookup.</param>
        /// <returns>Position in world space of this location in the tile map.</returns>
        Vector3 GetWorldPosition(K loc);

        /// <summary>
        /// Gets the world rotation of a given coordinate within the tile map.
        /// </summary>
        /// <param name="loc">Rotation within the tile map to lookup.</param>
        /// <returns>Rotation in world space of this location in the tile map.</returns>
        Quaternion GetWorldRotation(K loc);
    }
}
