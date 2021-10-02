using nickmaltbie.TileMap.Common;
using UnityEngine;

namespace PropHunt.Environment.Torch
{
    public interface IWorldGridContainer
    {
        IWorldGrid<Vector2Int, GameObject> GetWorldGrid();
    }
}