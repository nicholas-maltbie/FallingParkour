using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PropHunt.Character;
using PropHunt.Environment.Sound;
using UnityEngine;

namespace PropHunt.Environment.Checkpoint
{
    /// <summary>
    /// Checkpoint flag that, when entered, updates the checkpoint of players
    /// </summary>
    public class CheckpointFlag : MonoBehaviour
    {
        public GameObject checkpoint;

        private ISpawnPointCollection GetCheckpoint() => checkpoint.GetComponent<ISpawnPointCollection>();

        public void OnTriggerEnter(Collider other)
        {
            CharacterCheckpoint character = other.gameObject.GetComponent<CharacterCheckpoint>();
            if (character != null && !character.HasCrossedCheckpoint(GetCheckpoint()))
            {
                character.UpdateCheckpoint(GetCheckpoint());
                SoundEffectManager.CreateNetworkedSoundEffectAtPoint(character.transform.position, SoundMaterial.Misc, SoundType.Checkpoint);
            }
        }
    }
}
