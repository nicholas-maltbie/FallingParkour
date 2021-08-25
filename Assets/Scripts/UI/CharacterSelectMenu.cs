using PropHunt.Character.Avatar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public float buttonWhitespace = 266f;

    [SerializeField]
    private List<CharacterSelectButton> buttons;

    void Start()
    {
        buttons = new List<CharacterSelectButton>();
        var avatarIds = avatarLibrary.GetAvatarIds();
        print(avatarIds.Count);
        var count = 0;

        foreach(var id in avatarIds)
        {
            var button = Instantiate(characterSelectButtonPrefab, transform.position, Quaternion.identity, this.transform);
            var buttonComponent = button.GetComponent<CharacterSelectButton>();
            buttonComponent.InitializeButton(id);
            buttons.Add(buttonComponent);

            // Could create issues if sprites accidentally have different sizes. 
            // As of now, keep sprites to size of 128x128
            var offset = count * buttonWhitespace;
            print(button.GetComponent<RectTransform>().rect.width);

            button.transform.position += transform.right * offset;

            count++;
        }
    }
}
