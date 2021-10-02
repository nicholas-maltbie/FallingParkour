
using System;
using System.Collections.Generic;
using System.Linq;
using nickmaltbie.TileMap.Common;
using nickmaltbie.TileMap.Hexagon;
using nickmaltbie.TileMap.Pathfinding;
using UnityEngine;

namespace nickmaltbie.TileMap.Example
{
    /// <summary>
    /// Coordinate of a hex referenced back to the grid it belongs to.
    /// </summary>
    public class Coord : MonoBehaviour
    {
        public Vector2Int coord;
    }

    /// <summary>
    /// Path modes that can be used when pathfinding in this grid.
    /// </summary>
    public enum PathMode
    {
        DepthFirstSearch,
        BreadthFirstSearch,
        AStar,
        HillClimbing
    }

    /// <summary>
    /// Example hexagon grid of spawned prefabs for testing purposes.
    /// </summary>
    public class ExampleHexGrid : MonoBehaviour
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

        /// <summary>
        /// Type of mode for searching for the 
        /// </summary>
        [Tooltip("Path finding mode to use when searching for path.")]
        [SerializeField]
        private PathMode searchMode = PathMode.AStar;

        /// <summary>
        /// World grid containing spawned prefabs.
        /// </summary>
        private IWorldGrid<Vector2Int, GameObject> worldGrid;

        public void Start()
        {
            ITileMap<Vector2Int, GameObject> tileMap = new HexTileMap<GameObject>(
                this.width, this.height);
            this.worldGrid = new HexWorldGrid<GameObject>(
                tileMap, this.hexRadius, base.transform);

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
                        spawned.AddComponent<Coord>().coord = pos;
                        tileMap[pos] = spawned;
                    }
                )
            );
        }

        private Vector2Int selected1;
        private Vector2Int selected2;
        private int toggle = 0;
        private List<Vector2Int> path = new List<Vector2Int>();

        private GameObject GetTile(Vector2Int loc) => worldGrid.GetTileMap()[loc];

        private void ColorTile(Vector2Int loc, Color color)
        {
            GetTile(loc).GetComponent<MeshRenderer>().material.SetColor("_Color", color);
        }

        public void Update()
        {

            if (Input.GetButtonDown("Fire1"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.collider == null)
                    {
                        return;
                    }

                    Coord coord = hit.collider.gameObject.GetComponent<Coord>();
                    if (coord == null)
                    {
                        return;
                    }

                    Vector2Int selected = coord.coord;

                    if (toggle == 0)
                    {
                        path.ForEach(loc => ColorTile(loc, Color.white));
                        selected1 = selected;
                        ColorTile(selected, Color.yellow);
                    }
                    else if (toggle == 1)
                    {
                        selected2 = selected;
                        ColorTile(selected, Color.yellow);

                        switch (searchMode)
                        {
                            case PathMode.DepthFirstSearch:
                                worldGrid.GetTileMap().FindPathDFS(selected1, selected2, out path);
                                break;
                            case PathMode.BreadthFirstSearch:
                                worldGrid.GetTileMap().FindPathBFS(selected1, selected2, out path);
                                break;
                            case PathMode.HillClimbing:
                                Func<Path<Vector2Int>, float> pathWeightHillClimbing = (Path<Vector2Int> path) =>
                                    Vector2Int.Distance(path.Node, selected2);
                                worldGrid.GetTileMap().FindPathAStar(selected1, selected2, pathWeightHillClimbing, out path);
                                break;
                            case PathMode.AStar:
                                // Func<Path<Vector2Int>, Tuple<int, float>> pathWeightAStar = (Path<Vector2Int> path) =>
                                //     new Tuple<int, float>(path.Length(), Vector2Int.Distance(path.Node, selected2));
                                Func<Path<Vector2Int>, float> pathWeightAStar = (Path<Vector2Int> path) =>
                                    path.Length() + Vector2Int.Distance(path.Node, selected2);
                                worldGrid.GetTileMap().FindPathAStar(selected1, selected2, pathWeightAStar, out path);
                                break;
                        }

                        path.Where(loc => loc != selected1 && loc != selected2)
                            .ToList()
                            .ForEach(loc => ColorTile(loc, Color.red));
                    }

                    toggle = (toggle + 1) % 2;
                }
            }
        }

    }
}
