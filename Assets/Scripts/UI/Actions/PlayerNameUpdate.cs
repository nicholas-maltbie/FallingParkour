using PropHunt.Character;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI.Actions
{
    /// <summary>
    /// Player name update to filter and load player name on startup and when edited
    /// </summary>
    public class PlayerNameUpdate : MonoBehaviour
    {
        /// <summary>
        /// Key to store player name information under
        /// </summary>
        public const string playerNamePlayerPrefKey = "PlayerName";

        /// <summary>
        /// Input field that player can edit to adjust the player name
        /// </summary>
        public InputField field;

        public void Start()
        {
            ReloadName();
        }

        /// <summary>
        /// Load saved player name from preferences
        /// </summary>
        private void ReloadName()
        {
            field.text = PlayerPrefs.GetString(playerNamePlayerPrefKey, "");
        }

        /// <summary>
        /// Update player name value and filter based on new input text
        /// </summary>
        public void UpdatePlayerName()
        {
            string name = field.text;
            if (name.Length > CharacterNameManagement.MaxNameLength)
            {
                name = name.Substring(0, CharacterNameManagement.MaxNameLength);
                field.SetTextWithoutNotify(name);
            }
            field.SetTextWithoutNotify(CharacterNameManagement.GetFilteredNameIgnoreWhitespace(name));
            CharacterNameManagement.playerName = CharacterNameManagement.GetFilteredName(name);
            // Save updated player name
            PlayerPrefs.SetString(playerNamePlayerPrefKey, CharacterNameManagement.playerName);
        }
    }
}