using PropHunt.Character;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI
{
    public class PlayerNameUpdate : MonoBehaviour
    {
        public InputField field;

        public void UpdatePlayerName()
        {
            field.text = CharacterNameManagement.GetFilteredNameIgnoreWhitespace(field.text);
            CharacterNameManagement.playerName = CharacterNameManagement.GetFilteredName(field.text);
        }
    }
}