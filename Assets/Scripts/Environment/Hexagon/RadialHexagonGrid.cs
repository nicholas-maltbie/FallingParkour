using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment.Hexagon
{
    /// <summary>
    /// Create a radial grid of hexagons starting from a single point
    /// </summary>
    public class RadialHexagonGrid : MonoBehaviour
    {
        public GameObject hexagonPrefab;

        public float hexagonRadius = 0.5f;

        public float HexagonSep => 2 * hexagonRadius * Mathf.Cos(Mathf.Deg2Rad * 30);

        public int tileRadius = 2;

        public void Start()
        {
            // Spawn central hex
            GameObject centralHex = GameObject.Instantiate(hexagonPrefab);
            centralHex.transform.position = transform.position;
            centralHex.transform.parent = transform;
            // Get 60 degrees in radians
            float rad60deg = Mathf.Deg2Rad * 60;

            // Construct each of the six arms of the hexagon
            for (int arm = 0; arm < 6; arm++)
            {
                // Get direction of this arm
                Vector2 dir = new Vector2(
                    Mathf.Cos(rad60deg * arm),
                    Mathf.Sin(rad60deg * arm)) * (HexagonSep);
                Vector2 side = new Vector2(
                    Mathf.Cos(rad60deg * (arm - 2)),
                    Mathf.Sin(rad60deg * (arm - 2))) * (HexagonSep);

                // Construct hexagons out from starting position
                for (int distance = 1; distance < tileRadius; distance++)
                {
                    // Compute the starting offset for this distance
                    Vector2 baseOffset = dir * distance;

                    // Each step out has step number of hexagons in the ring for this arm
                    for (int step = 0; step < distance; step++)
                    {
                        // Each tile will be spawned at the base offset + an arm rotated 60 more degrees
                        GameObject hex = GameObject.Instantiate(hexagonPrefab);
                        Vector2 newPos = baseOffset + step * side;
                        Vector3 hexPos = new Vector3(newPos.x, 0, newPos.y);
                        hex.transform.position = transform.position + hexPos;
                        hex.transform.parent = transform;
                    }
                }
            }
        }
    }
}