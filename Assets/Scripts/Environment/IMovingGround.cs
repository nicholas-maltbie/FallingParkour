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
        
        /// <summary>
        /// Should momentum be transferred to players when they
        /// leave this object.
        /// </summary>
        bool AvoidTransferMomentum();

        /// <summary>
        /// When following this object, should the player attach themselves
        /// to the object to follow it properly? This is important
        /// for rapidly moving objects. Additionally, if the object does
        /// not move but wants to push the player (such as a conveyer belt),
        /// then players should definitely not attach to the object.
        /// </summary>
        bool ShouldAttach();
    }
}