using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Moving ground object that a player can move along with
    /// </summary>
    public interface IMovingGround
    {
        /// <summary>
        /// Get the velocity (in units per second) that the object is moving
        /// at a given point on the surface of the object (in world space).
        /// </summary>
        /// <param name="point">Point on the surface of the object (in world space).</param>
        /// <returns>Velocity that the object is moving at the point.</returns>
        Vector3 GetVelocityAtPoint(Vector3 point);

        /// <summary>
        /// Get displacement of the moving object at a given point on the
        /// surface of the object (in world space) for the current fixed update.
        /// </summary>
        /// <param name="point">Point on the surface of the object (in world space).</param>
        /// <returns>Displacement on the surface of this object from that point
        /// for the current fixed update</returns>
        Vector3 GetDisplacementAtPoint(Vector3 point);
    }
}