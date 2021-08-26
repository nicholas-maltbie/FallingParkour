using PropHunt.Character.Avatar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI
{
    public class CharacterSelectMenu : MonoBehaviour
    {
        /// <summary>
        /// Avatar library for accessing 
        /// </summary>
        [SerializeField]
        private CharacterAvatarLibrary avatarLibrary;
        
        /// <summary>
        /// CharacterSelectButton prefab to instantiate with
        /// </summary>
        [SerializeField]
        private GameObject characterSelectButtonPrefab;

        /// <summary>
        /// Horizontal space between edges of buttons
        /// </summary>
        public float buttonWhitespace = 10f;

        [SerializeField]
        private List<CharacterSelectButton> buttons;

        void Awake()
        {
            // Load the default selected avatar
            CharacterAvatarManager.defaultAvatar = PlayerPrefs.GetString(
                CharacterSelectButton.CharacterSelectPerfKey,
                CharacterAvatarManager.defaultAvatar
            );
        }

        void Start()
        {
            buttons = new List<CharacterSelectButton>();
            var avatarIds = avatarLibrary.GetAvatarIds();
            var count = 0;
            var totalSpace = 0f;

            foreach(var id in avatarIds)
            {
                var button = Instantiate(characterSelectButtonPrefab, transform.position, Quaternion.identity, this.transform);
                var buttonComponent = button.GetComponent<CharacterSelectButton>();
                var toggleGroupComponent = GetComponent<ToggleGroup>();

                try {
                    buttonComponent.InitializeButton(id, avatarLibrary);
                }
                catch {
                    // Skip this button
                    Destroy(button);
                    print("Failed to initialize button for avatar " + id);
                    continue;
                }

                button.GetComponent<Toggle>().group = toggleGroupComponent;
                buttons.Add(buttonComponent);

                // Sprites should all be kept the same size. 
                var offset = buttonWhitespace + count * (button.GetComponent<RectTransform>().rect.width + buttonWhitespace);
                totalSpace = offset;

                button.transform.position += transform.right * offset;
                count++;
            }

            // Increase width of this object's rect so horizontal scrolling works correctly.
            var rt = GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(totalSpace + buttonWhitespace, rt.sizeDelta[1]);
        }
    }
}
