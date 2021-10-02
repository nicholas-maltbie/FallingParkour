using nickmaltbie.TileMap.Common;
using nickmaltbie.TileMap.Hexagon;
using UnityEngine;

namespace nickmaltbie.TileMap.Example
{
    /// <summary>
    /// Example hexagon grid of spawned prefabs for testing purposes.
    /// </summary>
    public class ExampleHexGrid : AbstractExampleGrid
    {
        /// <summary>
        /// Width of the tile map in number of squares.
        /// </summary>
        [Tooltip("Width of the tile map in number of squares.")]
        [SerializeField]
        private int width = 8;

        /// <summary>
        /// Height of the tile map in number of squares.
        /// </summary>
        [Tooltip("Height of the tile map in number of squares.")]
        [SerializeField]
        private int height = 8;

        /// <summary>
        /// Hexagon radius for each tile within the grid
        /// </summary>
        [Tooltip("Hexagon radius")]
        [SerializeField]
        private float hexRadius = 1.0f;

        protected override (IWorldGrid<Vector2Int, GameObject>, IBlockableTileMap<Vector2Int, GameObject>)
            CreateGridMap()
        {
            IBlockableTileMap<Vector2Int, GameObject> tileMap = new HexTileMap<GameObject>(
                this.width, this.height);

            return (
                new HexWorldGrid<GameObject>(tileMap, this.hexRadius, base.transform),
                tileMap
            );
        }
    }
}
