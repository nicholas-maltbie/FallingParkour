using System.Linq;
using nickmaltbie.TileMap.Common;
using nickmaltbie.TileMap.Hexagon;
using PropHunt.Environment.Torch;
using PropHunt.Utils;
using UnityEngine;

namespace nickmaltbie.TileMap.Example
{
    /// <summary>
    /// Example hexagon grid of spawned prefabs for testing purposes.
    /// </summary>
    public class SpawnPrefabHexGrid : MonoBehaviour, IWorldGridContainer
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

        /// <summary>
        /// Prefab to spawn within each square in the grid.
        /// </summary>
        [Tooltip("Prefab to spawn within each square in the grid.")]
        [SerializeField]
        private GameObject tilePrefab;

        [SerializeField]
        private Gradient mainColorGradient;

        [SerializeField]
        private Gradient secondColorGradient;

        /// <summary>
        /// World grid containing spawned prefabs.
        /// </summary>
        private IWorldGrid<Vector2Int, GameObject> worldGrid;

        public IWorldGrid<Vector2Int, GameObject> GetWorldGrid() => worldGrid;

        public void Awake()
        {
            ITileMap<Vector2Int, GameObject> tileMap = new HexTileMap<GameObject>(
                this.width, this.height);
            this.worldGrid = new HexWorldGrid<GameObject>(
                tileMap, this.hexRadius, base.transform);

            Vector2Int min = new Vector2Int(0, 0);
            Vector2Int max = new Vector2Int(this.width, this.height);

            foreach (Vector2Int pos in tileMap)
            {
                GameObject spawned = GameObject.Instantiate(this.tilePrefab);
                spawned.transform.SetParent(base.transform);

                spawned.name = $"({pos.x}, {pos.y})";
                spawned.transform.position = this.worldGrid.GetWorldPosition(pos) - transform.position;
                spawned.transform.rotation = Quaternion.Euler(
                        this.tilePrefab.transform.rotation.eulerAngles +
                        this.worldGrid.GetWorldRotation(pos).eulerAngles);
                spawned.AddComponent<Coord>().coord = pos;
                tileMap[pos] = spawned;

                // Sample colors as ratio between min and max, distance to max : distance to min.
                float distMin = Vector2Int.Distance(min, pos);
                float distMax = Vector2Int.Distance(max, pos);
                float samplePos = distMin / (distMax + distMin);

                MaterialUtils.RecursiveSetColorProperty(
                    spawned, "_ColorWhite", mainColorGradient.Evaluate(samplePos));
                MaterialUtils.RecursiveSetColorProperty(
                    spawned, "_ColorBlack", secondColorGradient.Evaluate(samplePos));
            }
        }
    }
}
