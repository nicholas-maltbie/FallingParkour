
using System.Collections.Generic;
using UnityEngine;

namespace PropHunt.Utils
{
    public struct ColliderCastHit
    {
        /// <summary>
        /// Did this collide with anything
        /// </summary>
        public bool hit;
        /// <summary>
        /// How far did this collider travel before it hit anything (if at all)
        /// </summary>
        public float distance;
        /// <summary>
        /// The point that the object collided with something (if at all)
        /// </summary>
        public Vector3 pointHit;
        /// <summary>
        /// Normal vector of the surface hit
        /// </summary>
        public Vector3 normal;
        /// <summary>
        /// Fraction of total movement achieved if this object hit something
        /// Should be between 0 (for no movement) and 1 (didn't hit anything)
        /// </summary>
        public float fraction;
        /// <summary>
        /// Collider that was hit by this cast
        /// </summary>
        public Collider collider;
    }

    public interface IColliderCast
    {
        /// <summary>
        /// Cast this object and see what it collides with (ignoring overlapping colliders)
        /// </summary>
        /// <param name="direction">Direction to cast the collider</param>
        /// <param name="distance">distance the collider is being cast</param>
        /// <returns>The first object hit and collision information</returns>
        ColliderCastHit CastSelf(Vector3 direction, float distance);

        /// <summary>
        /// Get the colliders of all objects overlapping with this object
        /// </summary>
        /// <returns>Enumerable of colliders with all overlapping objects</returns>
        IEnumerable<Collider> GetOverlapping();

        /// <summary>
        /// Gets the contact points of all objects overlapping with this object
        /// </summary>
        /// <returns>Enumerable of contact points with all overlapping objects</returns>
        IEnumerable<ColliderCastHit> GetOverlappingDirectional();
    }

    public abstract class ColliderCast : MonoBehaviour, IColliderCast
    {
        public abstract ColliderCastHit CastSelf(Vector3 direction, float distance);
        public abstract IEnumerable<Collider> GetOverlapping();
        public abstract IEnumerable<ColliderCastHit> GetOverlappingDirectional();
    }
}
