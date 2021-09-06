using MLAPI;
using MLAPI.Messaging;

namespace PropHunt.Environment
{
    /// <summary>
    /// Detect when players stand on this object
    /// </summary>
    public abstract class DetectPlayerStand : NetworkBehaviour
    {
        /// <summary>
        /// When a player steps onto this tile
        /// </summary>
        /// <param name="sender">Who stepped on this object</param>
        [ServerRpc]
        public virtual void StepOnServerRpc() { }

        /// <summary>
        /// When a player steps off of this tile
        /// </summary>
        /// <param name="sender">Who stepped on this object</param>
        [ServerRpc]
        public virtual void StepOffServerRpc() { }
    }
}