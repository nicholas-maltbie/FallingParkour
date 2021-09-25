using MLAPI;
using MLAPI.NetworkVariable;
using PropHunt.Game.Level;
using PropHunt.UI;
using UnityEngine;
using static PropHunt.Game.Level.GameLevelLibrary;

namespace PropHunt.Game.Flow
{
    /// <summary>
    /// Has the player completed the current level.
    /// </summary>
    public enum CompletedStatus
    {
        Incomplete,
        Completed
    }

    /// <summary>
    /// Completed level status token attached to each player
    /// </summary>
    public class CompletedLevel : NetworkBehaviour
    {
        /// <summary>
        /// Current completed level status of this player.
        /// </summary>
        public NetworkVariable<CompletedStatus> Status = new NetworkVariable<CompletedStatus>(CompletedStatus.Incomplete);
    }
}
