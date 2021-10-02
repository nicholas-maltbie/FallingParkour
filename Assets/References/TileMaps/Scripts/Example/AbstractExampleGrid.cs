using System;
using System.Collections.Generic;
using System.Linq;
using nickmaltbie.TileMap.Common;
using nickmaltbie.TileMap.Pathfinding;
using UnityEngine;

namespace nickmaltbie.TileMap.Example
{
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
    /// Example grid of spawned prefabs.
    /// </summary>
    public abstract class AbstractExampleGrid : MonoBehaviour
    {
        /// <summary>
        /// Type of mode for searching for the 
        /// </summary>
        [Tooltip("Path finding mode to use when searching for path.")]
        [SerializeField]
        public PathMode searchMode = PathMode.AStar;

        /// <summary>
        /// World grid containing spawned prefabs.
        /// </summary>
        private IWorldGrid<Vector2Int, GameObject> worldGrid;

        /// <summary>
        /// Tile for this grid.
        /// </summary>
        private IBlockableTileMap<Vector2Int, GameObject> tileMap;

        /// <summary>
        /// Prefab to spawn within each square in the grid.
        /// </summary>
        [Tooltip("Prefab to spawn within each square in the grid.")]
        [SerializeField]
        private GameObject tilePrefab;

        /// <summary>
        /// First selected element in path.
        /// </summary>
        protected Nullable<Vector2Int> selected1;

        /// <summary>
        /// Second selected element in path.
        /// </summary>
        protected Nullable<Vector2Int> selected2;

        /// <summary>
        /// Toggle of current state in pathfinding.
        /// </summary>
        protected int toggle = 0;

        /// <summary>
        /// Currently found path.
        /// </summary>
        protected List<Vector2Int> path = new List<Vector2Int>();

        public IWorldGrid<Vector2Int, GameObject> WorldGrid => this.worldGrid;

        public GameObject TilePrefab => this.tilePrefab;

        public void OnEnable()
        {
            (worldGrid, tileMap) = CreateGridMap();
            foreach (Vector2Int pos in this.worldGrid.GetTileMap())
            {
                GameObject spawned = GameObject.Instantiate(this.TilePrefab) as GameObject;
                spawned.transform.SetParent(this.transform);

                spawned.name = $"({pos.x}, {pos.y})";
                spawned.transform.position = this.worldGrid.GetWorldPosition(pos);
                spawned.transform.rotation = Quaternion.Euler(
                        this.tilePrefab.transform.rotation.eulerAngles +
                        this.worldGrid.GetWorldRotation(pos).eulerAngles);
                spawned.AddComponent<Coord>().coord = pos;
                this.worldGrid.GetTileMap()[pos] = spawned;
            }
        }

        public void OnDisable()
        {
            foreach (Vector2Int pos in this.worldGrid.GetTileMap())
            {
                GameObject.Destroy(this.worldGrid.GetTileMap()[pos]);
            }
        }

        protected abstract (IWorldGrid<Vector2Int, GameObject>, IBlockableTileMap<Vector2Int, GameObject>)
            CreateGridMap();

        private void ColorTile(Vector2Int loc, Color color)
        {
            GetTile(loc).GetComponent<MeshRenderer>().material.SetColor("_Color", color);
        }

        public void Update()
        {
            if (!(Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")))
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                return;
            }
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

            if (Input.GetButtonDown("Fire2"))
            {
                // toggle blocked state
                if (tileMap.IsBlocked(selected))
                {
                    tileMap.Unblock(selected);
                }
                else
                {
                    tileMap.Block(selected);
                }

                // Update color
                UpdateTileColor(selected);
            }
            if (Input.GetButtonDown("Fire1"))
            {
                if (tileMap.IsBlocked(selected))
                {
                    return;
                }

                if (toggle == 0)
                {
                    // Clear out previous  path
                    selected1 = null;
                    selected2 = null;
                    List<Vector2Int> savedPath = path;
                    this.path = null;
                    savedPath.ForEach(loc => UpdateTileColor(loc));

                    // Start new path
                    selected1 = selected;
                    UpdateTileColor(selected);
                }
                else if (toggle == 1)
                {
                    selected2 = selected;
                    UpdateTileColor(selected);

                    switch (searchMode)
                    {
                        case PathMode.DepthFirstSearch:
                            WorldGrid.GetTileMap().FindPathDFS(selected1.Value, selected2.Value, out path);
                            break;
                        case PathMode.BreadthFirstSearch:
                            WorldGrid.GetTileMap().FindPathBFS(selected1.Value, selected2.Value, out path);
                            break;
                        case PathMode.HillClimbing:
                            Func<Path<Vector2Int>, float> pathWeightHillClimbing = (Path<Vector2Int> path) =>
                                Vector2Int.Distance(path.Node, selected2.Value);
                            WorldGrid.GetTileMap().FindPathAStar(
                                selected1.Value, selected2.Value, pathWeightHillClimbing, out path);
                            break;
                        case PathMode.AStar:
                            // Func<Path<Vector2Int>, Tuple<int, float>> pathWeightAStar = (Path<Vector2Int> path) =>
                            //     new Tuple<int, float>(path.Length(), Vector2Int.Distance(path.Node, selected2));
                            Func<Path<Vector2Int>, float> pathWeightAStar = (Path<Vector2Int> path) =>
                                path.Length() + Vector2Int.Distance(path.Node, selected2.Value);
                            WorldGrid.GetTileMap().FindPathAStar(
                                selected1.Value, selected2.Value, pathWeightAStar, out path);
                            break;
                    }

                    path.ForEach(loc => UpdateTileColor(loc));
                }

                toggle = (toggle + 1) % 2;
            }
        }

        public void UpdateTileColor(Vector2Int loc)
        {
            ColorTile(loc, GetTileColor(loc));
        }

        public Color GetTileColor(Vector2Int loc)
        {
            if (tileMap.IsBlocked(loc))
            {
                return Color.blue;
            }
            if (loc == selected1 || loc == selected2)
            {
                return Color.red;
            }
            if (path != null && path.Contains(loc))
            {
                return Color.yellow;
            }
            return Color.white;
        }

        private GameObject GetTile(Vector2Int loc) => this.worldGrid.GetTileMap()[loc];
    }
}