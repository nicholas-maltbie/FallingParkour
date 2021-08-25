using PropHunt.Character.Avatar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterSelectButton : MonoBehaviour
{
    /// <summary>
    /// Reference to avatar library for accessing avatar sprites + information
    /// </summary>
    [SerializeField]
    private CharacterAvatarLibrary avatarLibrary;

    /// <summary>
    /// String which contains the avatar ID associated with this button
    /// </summary>
    [SerializeField]
    private string avatarId;

    /// <summary>
    /// Function to be called after initialization that determines which sprite
    /// picture should be used in this button, then sets this button's sprite component
    /// to that picture. 
    /// </summary>
    /// <param name="avatarId">
    /// ID of this character in the avatar library. Used to locate sprite.
    /// </param>
    public void InitializeButton(string avatarId)
    {
        this.avatarId = avatarId;
        var characterAvatarObj = avatarLibrary.GetCharacterAvatar(avatarId);
        var characterSprite = characterAvatarObj.characterSprite;

        var spriteComponent = GetComponent<Image>();
        spriteComponent.sprite = characterSprite;
    }
}
