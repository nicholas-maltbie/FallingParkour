namespace nickmaltbie.TileMap.Common
{
    /// <summary>
    /// Tile map that supports blocking spaces within the map. Blocked locations are still within the map but can be
    /// ignored when calculating neighbors of a tile as they do not show up in the neighbors of a tile.
    /// </summary>
    /// <typeparam name="K">Coordinate system for the map.</typeparam>
    /// <typeparam name="V">Values held in the map.</typeparam>
    public interface IBlockableTileMap<K, V> : ITileMap<K, V>
    {
        /// <summary>
        /// Block a given location within the tile map.
        /// </summary>
        /// <param name="loc">Location within the tile map to block.</param>
        void Block(K loc);

        /// <summary>
        /// Unblocks state of a tile within the map.
        /// </summary>
        /// <param name="loc">Location within the tile map to unblock.</param>
        void Unblock(K loc);

        /// <summary>
        /// Check if a location within the tile map is blocked.
        /// </summary>
        /// <param name="loc">Location within the tile map to check.</param>
        /// <returns>True if the tile is blocked, false otherwise.</returns>
        bool IsBlocked(K loc);
    }
}
