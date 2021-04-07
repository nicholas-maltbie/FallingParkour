
using UnityEngine;

namespace PropHunt.Utils
{
    public static class PhysicsUtils
    {
        /// <summary>
        /// Filters a set of raycast hits for the first hit that is not 
        /// of a collider attached to a given game object
        /// </summary>
        /// <param name="ignore">Game object to ignore when checking for collider hit</param>
        /// <param name="hits">All of the hits to process</param>
        /// <param name="closest">Closest hit that is attached to the ignored object</param>
        /// <returns>True if a hit was detected, false otherwise</returns>
        public static bool FilterForFirstHitIgnore(GameObject ignore, RaycastHit[] hits, out RaycastHit closest)
        {
            bool hitSomething = false;
            closest = new RaycastHit { distance = Mathf.Infinity };
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject != ignore && hit.distance < closest.distance)
                {
                    hitSomething = true;
                    closest = hit;
                }
            }
            return hitSomething;
        }

        /// <summary>
        /// Compute the first object hit while ignoring a given object for a raycast.
        /// Will include overlapping objects if objects overlap.
        /// </summary>
        /// <param name="ignore">Object to ignore from raycast</param>
        /// <param name="source">Source position of raycast</param>
        /// <param name="direction">Direction of raycast</param>
        /// <param name="distance">Distance of raycast</param>
        /// <param name="layerMask">Laymask for raycast</param>
        /// <param name="queryTriggerInteraction">Query trigger interaction for raycast</param>
        /// <param name="closest">The closest raycast hit event</param>
        /// <returns>True if a hit was detected, false otherwise.</returns>
        public static bool RaycastFirstHitIgnore(GameObject ignore, Vector3 source, Vector3 direction, float distance,
            LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction, out RaycastHit closest)
        {
            return FilterForFirstHitIgnore(ignore, Physics.RaycastAll(source, direction, distance, layerMask, queryTriggerInteraction), out closest);
        }

        /// <summary>
        /// Compute the first object hit while ignoring a given object for a spherecast.
        /// Will include overlapping objects if objects overlap.
        /// </summary>
        /// <param name="ignore">Object to ignore from spherecast</param>
        /// <param name="source">Source position of spherecast</param>
        /// <param name="radius">Radius spherecast</param>
        /// <param name="direction">Direction of spherecast</param>
        /// <param name="distance">Distance of spherecast</param>
        /// <param name="layerMask">Laymask for spherecast</param>
        /// <param name="queryTriggerInteraction">Query trigger interaction for spherecast</param>
        /// <param name="closest">The closest raycast hit event</param>
        /// <returns>True if a hit was detected, false otherwise.</returns>
        public static bool SpherecastFirstHitIgnore(GameObject ignore, Vector3 source, float radius, Vector3 direction, float distance,
            LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction, out RaycastHit closest)
        {
            return FilterForFirstHitIgnore(ignore, Physics.SphereCastAll(source, radius, direction, distance, layerMask, queryTriggerInteraction), out closest);
        }
    }
}
