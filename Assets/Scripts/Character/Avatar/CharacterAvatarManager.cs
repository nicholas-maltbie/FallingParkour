using System.Linq;
using Mirror;
using UnityEngine;

namespace PropHunt.Character.Avatar
{
    /// <summary>
    /// Have a character controller push any dynamic rigidbody it hits
    /// </summary>
    public class CharacterAvatarManager : NetworkBehaviour
    {
        /// <summary>
        /// Library of all avatars that the player can use
        /// </summary>
        [SerializeField]
        private CharacterAvatarLibrary avatarLibrary;

        /// <summary>
        /// Where is the model stored
        /// </summary>
        [SerializeField]
        private GameObject modelBase;

        /// <summary>
        /// Current avatar selected by this player
        /// </summary>
        [SyncVar(hook = nameof(OnAvatarChange))]
        public string avatarSelected;

        public void Start()
        {
            if (isLocalPlayer)
            {
                CmdSetAvatar(avatarLibrary.DefaultAvater.name);
            }
        }

        /// <summary>
        /// Delete the current avatar stored for this character
        /// </summary>
        private void ClearAvatar()
        {
            Enumerable.Range(0, modelBase.transform.childCount)
                .Select(i => modelBase.transform.GetChild(i))
                .ToList()
                .ForEach(child => GameObject.Destroy(child.gameObject));
        }

        private GameObject SetupAvatar(string avatarName)
        {
            GameObject avatar = avatarLibrary.DefaultAvater;
            if (avatarLibrary.HasCharacterAvatar(avatarName))
            {
                avatar = avatarLibrary.GetCharacterAvatar(avatarName);
            }

            GameObject created = GameObject.Instantiate(avatar);
            created.transform.SetParent(modelBase.transform);
            return created;
        }

        [Command]
        public void CmdSetAvatar(string newlySelectedAvatar)
        {
            avatarSelected = newlySelectedAvatar;
            LoadNewAvatar(newlySelectedAvatar);
        }

        public void OnGUI()
        {
            if(GUI.Button(new Rect(10, 10, 100, 20), "Change to xbot"))
            {
                CmdSetAvatar("xbot");
            }
        }

        public void LoadNewAvatar(string avatarName)
        {
            ClearAvatar();
            GameObject newAvatar = SetupAvatar(avatarName);
            var networkAnimator = GetComponent<NetworkAnimator>();
            var animator = newAvatar.GetComponent<Animator>();
            if (networkAnimator != null)
            {
                networkAnimator.animator = animator;
            }
            this.transform.GetComponentsInChildren<IAvatarChange>().ToList().ForEach(
                change => change.OnAvatarChange(newAvatar)
            );
        }

        public void OnAvatarChange(string _, string avatarName) => LoadNewAvatar(avatarName);
    }
}