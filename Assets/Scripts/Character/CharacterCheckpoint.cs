using System.Collections.Generic;
using Mirror;
using PropHunt.Character.Avatar;
using PropHunt.Environment.Checkpoint;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character
{
    /// <summary>
    /// Checkpoint associated with a character
    /// </summary>
    public class CharacterCheckpoint : NetworkBehaviour
    {
        /// <summary>
        /// Previous checkpoints for character
        /// </summary>
        private HashSet<ISpawnPointCollection> previousCheckpoints = new HashSet<ISpawnPointCollection>();

        /// <summary>
        /// Current checkpoint player will be sent to if they are sent back to checkpoint
        /// </summary>
        private ISpawnPointCollection currentCheckpoint;

        /// <summary>
        /// Highest priority checkpoint the player has passed
        /// </summary>
        private int maxCheckpointPriority;

        /// <summary>
        /// Update the checpoint to be a new value
        /// </summary>
        /// <param name="newCheckpoint">new checkpoint to assing this player to</param>
        public void UpdateCheckpoint(ISpawnPointCollection newCheckpoint)
        {
            if (currentCheckpoint != null)
            {
                previousCheckpoints.Add(newCheckpoint);
            }
            maxCheckpointPriority = Mathf.Max(newCheckpoint.Priority(), currentCheckpoint.Priority());
            currentCheckpoint = newCheckpoint;
        }

        /// <summary>
        /// has the character passed this given checkpoint
        /// </summary>
        /// <param name="checkpoint">Checkpoint to verify</param>
        /// <returns>True if the player has crossed a given checkpoint, false otherwise.</returns>
        public bool IsPreviousCheckpoint(ISpawnPointCollection checkpoint)
        {
            return previousCheckpoints.Contains(checkpoint) || currentCheckpoint == checkpoint || checkpoint.Priority() <= maxCheckpointPriority;
        }

        public void Start()
        {
            currentCheckpoint = SpawnPointUtilities.GetDefaultCheckpoint();
        }

        /// <summary>
        /// Move player back to their checkpoint
        /// </summary>
        public void MoveToCheckpoint()
        {
            (var pos, var rot) = currentCheckpoint.GetRandomSpawn();
            transform.position = pos;
            transform.rotation = rot;
        }
    }

}
