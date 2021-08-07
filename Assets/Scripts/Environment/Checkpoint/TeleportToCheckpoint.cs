using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PropHunt.Character;
using UnityEngine;

namespace PropHunt.Environment.Checkpoint
{
    /// <summary>
    /// A Box that, when entered by a player, will send them back to their checkpoint.
    /// </summary>
    public class TeleportToCheckpoint : MonoBehaviour
    {
        public void OnTriggerEnter(Collider other)
        {
            CharacterCheckpoint character = other.gameObject.GetComponent<CharacterCheckpoint>();
            if (character != null)
            {
                character.MoveToCheckpoint();
            }
        }
    }
}
