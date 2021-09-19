using System.Collections;
using MLAPI;
using UnityEngine;

namespace PropHunt.Environment.Hexagon
{
    /// <summary>
    /// Create a radial grid of hexagons starting from a single point
    /// </summary>
    public class RadialHexagonGrid : NetworkBehaviour
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
        /// Main recolor for hexagons in this layer
        /// </summary>
        public Color recolor1;

        /// <summary>
        /// Secondary recolor for hexagons in this layer
        /// </summary>
        public Color recolor2;

        /// <summary>
        /// 60 degrees in radians
        /// </summary>
        public const float rad60deg = Mathf.Deg2Rad * 60;

        /// <summary>
        /// Base for attaching hexagons to
        /// </summary>
        private GameObject hexBase;

        /// <summary>
        /// Spawn a hex relative to the center of the hexagon grid
        /// </summary>
        /// <param name="arm">Which of the six arms is this hex on [0-6)</param>
        /// <param name="distance">How many hexagons is this away from the center</param>
        /// <param name="step">Which step along the arm is the hexagon (away from the radial line)</param>
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
            DeleteOnStand hexDelete = hex.GetComponent<DeleteOnStand>();
            if (hexDelete != null)
            {
                hexDelete.normalColor1.Value = recolor1;
                hexDelete.normalColor2.Value = recolor2;
                Color.RGBToHSV(recolor1, out float h1, out float s1, out float v1);
                Color.RGBToHSV(recolor2, out float h2, out float s2, out float v2);
                hexDelete.fadeColor1.Value = Color.HSVToRGB(h1, s1 * 0.2f, 1 - ((1 - v1) * 0.2f));
                hexDelete.fadeColor2.Value = Color.HSVToRGB(h2, s2 * 0.2f, 1 - ((1 - v2) * 0.2f));
                hexDelete.UpdateColor();
            }
            Vector2 newPos = distanceOffset + step * side;
            Vector3 hexPos = new Vector3(newPos.x, 0, newPos.y);
            hex.transform.position = transform.position + hexPos;
            hex.transform.parent = hexBase.transform;

            hex.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
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
            if (IsServer)
            {
                hexBase = new GameObject();
                hexBase.name = "Hexagon Base";
                hexBase.transform.parent = transform;
                StartCoroutine(CreateGrid());
            }
        }
    }
}