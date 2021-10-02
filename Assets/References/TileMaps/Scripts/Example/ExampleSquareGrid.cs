
using System.Linq;
using nickmaltbie.TileMap.Common;
using nickmaltbie.TileMap.Square;
using UnityEngine;

namespace nickmaltbie.TileMap.Example
{
    /// <summary>
    /// Example square grid of spawned prefabs for testing purposes.
    /// </summary>
    public class ExampleSquareGrid : MonoBehaviour
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
        /// Prefab to spawn within each square in the grid.
        /// </summary>
        [Tooltip("Prefab to spawn within each square in the grid.")]
        [SerializeField]
        private GameObject tilePrefab;

        /// <summary>
        /// World grid containing spawned prefabs.
        /// </summary>
        private IWorldGrid<Vector2Int, GameObject> worldGrid;

        public void Start()
        {
            ITileMap<Vector2Int, GameObject> tileMap = new SquareTileMap<GameObject>(
                this.width, this.height, Adjacency.Orthogonal);
            this.worldGrid = new SquareWorldGrid<GameObject>(
                tileMap, this.tileSize, base.transform);

            Enumerable.Range(0, this.width).ToList().ForEach(
                x => Enumerable.Range(0, this.height).ToList().ForEach(
                    y =>
                    {
                        GameObject spawned = GameObject.Instantiate(this.tilePrefab);
                        spawned.transform.SetParent(base.transform);

                        var pos = new Vector2Int(x, y);

                        spawned.name = $"({x}, {y})";
                        spawned.transform.position = this.worldGrid.GetWorldPosition(pos);
                        spawned.transform.rotation = Quaternion.Euler(
                                this.tilePrefab.transform.rotation.eulerAngles +
                                this.worldGrid.GetWorldRotation(pos).eulerAngles);
                    }
                )
            );
        }

    }
}
