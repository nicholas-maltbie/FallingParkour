using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using PropHunt.Character;
using UnityEngine;

namespace PropHunt.Game.Flow
{
    /// <summary>
    /// A Box that, when entered by a player, will change player to a spectator.
    /// </summary>
    public class ChangeToSpectator : NetworkBehaviour
    {
        public PlayerSpawnManager playerSpawnManager;

        public void Start()
        {
            playerSpawnManager = GameObject.FindObjectOfType<PlayerSpawnManager>();
        }

        public void OnTriggerEnter(Collider other)
        {
            GameObject otherObj = other.gameObject;
            CharacterName character = otherObj.GetComponent<CharacterName>();
            if (isServer && character != null)
            {
                StartCoroutine(playerSpawnManager.SpawnPlayerCharacter(character.connectionToClient, playerSpawnManager.spectatorPlayer));
            }
        }
    }
}
