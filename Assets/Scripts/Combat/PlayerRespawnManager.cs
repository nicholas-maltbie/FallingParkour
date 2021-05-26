
using System;
using Mirror;
using PropHunt.Character;
using PropHunt.Environment.Sound;
using PropHunt.Game.Communication;
using UnityEngine;

namespace PropHunt.Combat
{
    /// <summary>
    /// Class to manage player respawn events
    /// </summary>
    public class PlayerRespawnManager : NetworkBehaviour
    {
        /// <summary>
        /// Player respawn location when they are attacked
        /// </summary>
        [Tooltip("Location in which player respans when they are attacked")]
        [SerializeField]
        private Vector3 respawnLocation;

        public override void OnStartServer()
        {
            CombatManager.OnPlayerAttack += HandlePlayerAttack;
        }

        public void OnDisable()
        {
            CombatManager.OnPlayerAttack -= HandlePlayerAttack;
        }

        public void OnDestroy()
        {
            CombatManager.OnPlayerAttack -= HandlePlayerAttack;
        }

        public void HandlePlayerAttack(object source, PlayerAttackEvent attack)
        {
            CharacterName sourceName = attack.source.GetComponent<CharacterName>();
            CharacterName targetName = attack.target.GetComponent<CharacterName>();

            DebugChatLog.SendChatMessage(new ChatMessage("", $"Player {sourceName.characterName} attacked {targetName.characterName}"));
            SoundEffectManager.CreateNetworkedSoundEffectAtPoint(targetName.transform.position, SoundMaterial.Misc, SoundType.Death,
                UnityEngine.Random.Range(0.8f, 1.2f));

            attack.target.transform.position = respawnLocation;
            attack.target.transform.rotation = Quaternion.identity;
        }
    }
}
