using nickmaltbie.TileMap.Common;
using nickmaltbie.TileMap.Square;
using UnityEngine;

namespace nickmaltbie.TileMap.Example
{
    /// <summary>
    /// Example square grid of spawned prefabs for testing purposes.
    /// </summary>
    public class ExampleSquareGrid : AbstractExampleGrid
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
        /// Size (distance) between spawned tiles in the world grid.
        /// </summary>
        [Tooltip("Size (distance) between spawned tiles in the world grid.")]
        [SerializeField]
        private float tileSize = 1.0f;

        /// <summary>
        /// Adjacency type used for this square grid.
        /// </summary>
        [Tooltip("Adjacency type for square grid.")]
        [SerializeField]
        private Adjacency adjacency = Adjacency.Orthogonal;

        protected override (IWorldGrid<Vector2Int, GameObject>, IBlockableTileMap<Vector2Int, GameObject>)
            CreateGridMap()
        {
            IBlockableTileMap<Vector2Int, GameObject> tileMap = new SquareTileMap<GameObject>(
                this.width, this.height, this.adjacency);

            return (new SquareWorldGrid<GameObject>(tileMap, this.tileSize, base.transform),
                tileMap
            );
        }

    }
}
