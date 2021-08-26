using PropHunt.Character.Avatar;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI
{
    /// <summary>
    /// Character select button for loading an avatar and select character's avatar
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class CharacterSelectButton : MonoBehaviour
    {
        /// <summary>
        /// The character selected by this user
        /// </summary>
        public const string CharacterSelectPerfKey = "CharacterSelected";

        /// <summary>
        /// String which contains the avatar ID associated with this button
        /// </summary>
        [SerializeField]
        private string avatarId;

        public void Start()
        {
            Toggle toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(value => 
            {
                if (value)
                {
                    UpdateAvatar();
                }
            });
        }

        /// <summary>
        /// Update character's currently selected avatar based on this button
        /// </summary>
        public void UpdateAvatar()
        {
            // Set default avatar
            CharacterAvatarManager.defaultAvatar = avatarId;

            // Find all character avatars and update their avatar id
            GameObject.FindObjectsOfType<CharacterAvatarManager>()
                .Where(avatar => avatar.isLocalPlayer)
                .ToList()
                .ForEach(avatar => avatar.CmdSetAvatar(avatarId));

            // Update default avatar in perf key
            PlayerPrefs.SetString(CharacterSelectPerfKey, avatarId);
        }

        /// <summary>
        /// Function to be called after initialization that determines which sprite
        /// picture should be used in this button, then sets this button's sprite component
        /// to that picture. 
        /// </summary>
        /// <param name="avatarId">
        /// ID of this character in the avatar library. Used to locate sprite.
        /// </param>
        /// <param name="avatarLibrary">
        /// Library for looking up and referencing character avatars
        /// </param>
        public void InitializeButton(string avatarId, CharacterAvatarLibrary avatarLibrary)
        {
            this.avatarId = avatarId;
            var characterAvatarObj = avatarLibrary.GetCharacterAvatar(avatarId);
            var characterSprite = characterAvatarObj.characterSprite;

            var spriteComponent = GetComponent<Image>();
            spriteComponent.sprite = characterSprite;

            gameObject.name = avatarId + " Button";
        }
    }
}
