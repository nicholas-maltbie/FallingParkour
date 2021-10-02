using nickmaltbie.TileMap.Common;
using PropHunt.Utils;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace PropHunt.Environment.Torch
{
    /// <summary>
    /// Manage illuminating a hex grid of elements from a torch manager.
    /// </summary>
    public class TorchManager : MonoBehaviour
    {
        public GameObject elementGrid;

        private IWorldGrid<Vector2Int, GameObject> worldGrid;

        private List<Torch> torches;

        private List<Vector2Int> coords;

        public void Start()
        {
            this.worldGrid = elementGrid.GetComponent<IWorldGridContainer>().GetWorldGrid();
            this.coords = new List<Vector2Int>();
            foreach (Vector2Int pos in this.worldGrid.GetTileMap())
            {
                this.coords.Add(pos);
            }
            this.torches = new List<Torch>(GetComponentsInChildren<Torch>());
        }

        public void SetIllumination(float value)
        {
            MaterialUtils.RecursiveSetFloatProperty(gameObject, "_Opacity", value);
        }

        public void LateUpdate()
        {
            // Update spawned hexes based on intensity
            this.coords.ForEach(
                pos => 
                {
                    // If the element does not exist, skip this step
                    if (this.worldGrid.GetTileMap()[pos] == null)
                    {
                        return;
                    }

                    // Find the torch with the highest intensity on this point
                    Vector3 objPos = this.worldGrid.GetTileMap()[pos].transform.position;

                    // Go through each torch, compute maximum intensity
                    float maxIntensity = 0;
                    foreach(Torch torch in torches)
                    {
                        float light = torch.GetStrength(objPos);
                        if (light > maxIntensity)
                        {
                            maxIntensity = light;
                        }
                    }

                    // Update opacity of tile as intensity
                    MaterialUtils.RecursiveSetFloatProperty(
                        this.worldGrid.GetTileMap()[pos],
                        "_Opacity",
                        maxIntensity);
                }
            );
        }
    }
}
