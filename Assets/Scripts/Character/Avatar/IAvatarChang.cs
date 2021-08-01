using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character.Avatar
{
    /// <summary>
    /// Component that needs to know when character avatar has updated
    /// </summary>
    public interface IAvatarChange
    {
        /// <summary>
        /// Invoked whenever the character avatar changes
        /// </summary>
        /// <param name="newAvatar">New avatar loaded as part of a character.</param>
        void OnAvatarChange(GameObject newAvatar);
    }
}