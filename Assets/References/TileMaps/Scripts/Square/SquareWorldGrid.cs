using nickmaltbie.TileMap.Common;
using UnityEngine;

namespace nickmaltbie.TileMap.Square
{
    /// <summary>
    /// Grid for loading a square grid map into the unity game scene.
    /// </summary>
    public class SquareWorldGrid<V> : IWorldGrid<Vector2Int, V>
    {
        /// <summary>
        /// Size of each tile within the grid.
        /// </summary>
        private float tileSize;

        /// <summary>
        /// Base position for this tile map.
        /// </summary>
        private Transform basePosition;

        /// <summary>
        /// Square tile map for this world grid.
        /// </summary>
        private ITileMap<Vector2Int, V> squareTileMap;

        /// <summary>
        /// Create a square grid with a given tile map.
        /// </summary>
        /// <param name="tileMap">Tile map that this world grid represents.</param>
        /// <param name="tileSize">Size of each tile, the length of each edge in the square.</param>
        /// <param name="basePosition">Base position of the square grid.</param>
        public SquareWorldGrid(ITileMap<Vector2Int, V> tileMap, float tileSize, Transform basePosition)
        {
            this.squareTileMap = tileMap;
            this.basePosition = basePosition;
            this.tileSize = tileSize;
        }

        /// <inheritdoc/>
        public ITileMap<Vector2Int, V> GetTileMap() => squareTileMap;

        /// <inheritdoc/>
        public Vector3 GetWorldPosition(Vector2Int loc) =>
            basePosition.position + basePosition.TransformVector(new Vector3(loc.x, 0, loc.y) * tileSize);

        /// <inheritdoc/>
        public Quaternion GetWorldRotation(Vector2Int loc) => basePosition.rotation;
    }
}