using Mirror;
using UnityEngine;

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
        [Command(requiresAuthority = false)]
        public virtual void CmdStepOn(NetworkConnectionToClient sender = null) { }

        /// <summary>
        /// When a player steps off of this tile
        /// </summary>
        /// <param name="sender">Who stepped on this object</param>
        [Command(requiresAuthority = false)]
        public virtual void CmdStepOff(NetworkConnectionToClient sender = null) { }
    }
}