using System.Collections;
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
        /// <summary>
        /// Hexagon prefab to spawn in areas
        /// </summary>
        public GameObject hexagonPrefab;

        /// <summary>
        /// Radius of each hexagon from center to vertex
        /// </summary>
        public float hexagonRadius = 0.5f;

        /// <summary>
        /// Compute the distance between two hexagon centers
        /// </summary>
        /// <returns></returns>
        public float HexagonSep => 2 * hexagonRadius * Mathf.Cos(Mathf.Deg2Rad * 30);

        /// <summary>
        /// Radius to build out from center when making the hex grid
        /// </summary>
        public int tileRadius = 2;

        /// <summary>
        /// Delay between animation of creating tiles for the grid
        /// </summary>
        public float timeAnimationDelay = 0.25f;

        /// <summary>
        /// 60 degrees in radians
        /// </summary>
        public const float rad60deg = Mathf.Deg2Rad * 60;

        public void SpawnHex(int arm, int distance, int step)
        {
            // Get direction of this arm
            Vector2 dir = new Vector2(
                Mathf.Cos(rad60deg * arm),
                Mathf.Sin(rad60deg * arm)) * (HexagonSep);
            Vector2 side = new Vector2(
                Mathf.Cos(rad60deg * (arm - 2)),
                Mathf.Sin(rad60deg * (arm - 2))) * (HexagonSep);

            // Compute the starting offset for this distance
            Vector2 distanceOffset = dir * distance;
            // Each tile will be spawned at the base offset + an arm rotated 60 more degrees
            GameObject hex = GameObject.Instantiate(hexagonPrefab);
            Vector2 newPos = distanceOffset + step * side;
            Vector3 hexPos = new Vector3(newPos.x, 0, newPos.y);
            hex.transform.position = transform.position + hexPos;
            hex.transform.parent = transform;
        }

        /// <summary>
        /// Create the hexagon grid
        /// </summary>
        public IEnumerator CreateGrid()
        {
            // Spawn central hex
            SpawnHex(0, 0, 0);

            if (timeAnimationDelay > 0)
            {
                yield return new WaitForSeconds(timeAnimationDelay);
            }

            // Construct hexagons out from starting position
            for (int distance = 1; distance < tileRadius; distance++)
            {
                // Construct each of the six arms of the hexagon
                for (int arm = 5; arm >= 0; arm--)
                {
                    // Each step out has step number of hexagons in the ring for this arm
                    for (int step = 0; step < distance; step++)
                    {
                        if (arm == 5 && step == 0)
                        {
                            continue;
                        }

                        SpawnHex(arm, distance, step);
                        if (timeAnimationDelay > 0)
                        {
                            yield return new WaitForSeconds(timeAnimationDelay);
                        }
                    }
                }
                // Spawn the hex we skipped earlier
                SpawnHex(5, distance, 0);
                if (timeAnimationDelay > 0)
                {
                    yield return new WaitForSeconds(timeAnimationDelay);
                }
            }
        }

        public void Start()
        {
            StartCoroutine(CreateGrid());
        }
    }
}