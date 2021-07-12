using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mirror;
using PropHunt.Game.Flow;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character
{
    /// <summary>
    /// Character name for the local player
    /// </summary>
    public static class CharacterNameManagement
    {
        /// <summary>
        /// Maximum name length
        /// </summary>
        public const int MaxNameLength = 16;

        /// <summary>
        /// Regex to check if a name is valid
        /// </summary>
        public static Regex validNamePattern = new Regex("^[A-Za-z0-9][A-Za-z0-9\\x20]{0,14}[A-Za-z0-9]$");

        /// <summary>
        /// Regex to filter out invalid parts of name
        /// </summary>
        public static Regex filterPattern = new Regex("^\\s+|[^A-Za-z0-9\\x20]|\\s+$");

        /// <summary>
        /// Regex to filter out invalid parts of name ignoring trailing and leading whitespace
        /// </summary>
        public static Regex filterPatternIgnoreWhitespace = new Regex("^[^A-Za-z0-9\\x20]$");

        /// <summary>
        /// Regex pattern to identify duplicate whitespaces
        /// </summary>
        public static Regex duplicateWhitespace = new Regex("\\s{2,}");

        /// <summary>
        /// name of the current local player
        /// </summary>
        public static string playerName = "Player";

        /// <summary>
        /// Is the current player name valid
        /// </summary>
        /// <returns>True if the name is valid, false otherwise</returns>
        public static bool HasValidPlayerName()
        {
            return VerifyName(CharacterNameManagement.playerName);
        }

        /// <summary>
        /// Get lookup of all names by player connection id
        /// </summary>
        public static SortedDictionary<int, string> GetPlayerNames()
        {
            SortedDictionary<int, string> playerNames = new SortedDictionary<int, string>();
            foreach (CharacterName name in GameObject.FindObjectsOfType<CharacterName>())
            {
                playerNames[name.playerId] = name.characterName;
            }
            return playerNames;
        }

        /// <summary>
        /// Filters a given name using regex to remove trailing and leading whitespace, duplicate whitespace,
        /// as well as any other invalid (non alpha numeric character) from the string.
        /// </summary>
        /// <param name="name">Name to filter</param>
        /// <returns>Filtered name using filterPattern and duplicateWhitespace pattern</returns>
        public static string GetFilteredName(string name)
        {
            return duplicateWhitespace.Replace(filterPattern.Replace(name, ""), name);
        }

        /// <summary>
        /// Filters a given name using regex but does not modify trailing and leading whitespace.
        /// </summary>
        /// <param name="name">Name to filter</param>
        /// <returns>Filtered name using filterPatternIgnoreWhitespace</returns>
        public static string GetFilteredNameIgnoreWhitespace(string name)
        {
            return filterPatternIgnoreWhitespace.Replace(name, "");
        }

        /// <summary>
        /// Verifying player name if it is valid, should only cotain letters A-Z,a-z and numbers 0-9
        /// </summary>
        /// <param name="name">Character name</param>
        /// <returns>If a given name is valid</returns>
        public static bool VerifyName(string name)
        {
            return validNamePattern.Match(name).Success;
        }
    }

    /// <summary>
    /// Player name change event
    /// </summary>
    public class PlayerNameChange : EventArgs
    {
        /// <summary>
        /// Previous name the player was using
        /// </summary>
        public readonly string previousName;
        /// <summary>
        /// New name associated with the player
        /// </summary>
        public readonly string newName;
        /// <summary>
        /// Network connection id of player joining the server
        /// </summary>
        public readonly int connId;

        /// <summary>
        /// Create a player name change event with previous and new names with the associated
        /// network connection
        /// </summary>
        /// <param name="previousName">Previous name the player used (will
        /// be empty string if the player had no name previously)</param>
        /// <param name="newName">New name the player is using</param>
        /// <param name="connId">Network connection id associated with this player</param>
        public PlayerNameChange(string previousName, string newName, int connId)
        {
            this.previousName = previousName;
            this.newName = newName;
            this.connId = connId;
        }
    }

    /// <summary>
    /// Component to hold a given character's name
    /// </summary>
    public class CharacterName : NetworkBehaviour
    {
        /// <summary>
        /// Events that occur on the server whenever a player changes their name
        /// </summary>
        public static EventHandler<PlayerNameChange> OnPlayerNameChange;

        /// <summary>
        /// Id associated with this player
        /// </summary>
        [SyncVar]
        public int playerId;

        /// <summary>
        /// name associated with this player
        /// </summary>
        [SyncVar]
        public string characterName = "Player";

        /// <summary>
        /// Network service for managing tests
        /// </summary>
        public INetworkService networkService;

        /// <summary>
        /// Update a player name from a client with authority via command
        /// </summary>
        /// <param name="newName">New name to assign a player</param>
        [Command]
        public void CmdUpdatePlayerName(string newName)
        {
            PlayerNameChange changeEvent = new PlayerNameChange(characterName, newName, playerId);
            characterName = newName;
            // Send an event for this change event
            OnPlayerNameChange?.Invoke(this, changeEvent);
        }

        public void Awake()
        {
            networkService = new NetworkService(this);
        }

        public void Start()
        {
            if (networkService.isServer && connectionToClient != null)
            {
                playerId = connectionToClient.connectionId;
            }
            // Synchronize state to server if local player
            if (networkService.isLocalPlayer)
            {
                CmdUpdatePlayerName(CharacterNameManagement.playerName);
            }
        }
    }
}