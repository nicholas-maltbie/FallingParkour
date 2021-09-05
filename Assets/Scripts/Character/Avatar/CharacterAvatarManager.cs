using System.Collections;
using System.Linq;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace PropHunt.Character.Avatar
{
    /// <summary>
    /// Have a character controller push any dynamic rigidbody it hits
    /// </summary>
    public class CharacterAvatarManager : NetworkBehaviour
    {
        /// <summary>
        /// Default avatar to select for character
        /// </summary>
        public static string defaultAvatar = "";

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
        public NetworkVariableString avatarSelected = new NetworkVariableString(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.Everyone}, "");

        public void Start()
        {
            if (IsLocalPlayer)
            {
                SetAvatarServerRpc(string.IsNullOrEmpty(defaultAvatar) ? avatarLibrary.DefaultAvater.Name : defaultAvatar);
            }
        }

        public void OnEnable()
        {
            avatarSelected.OnValueChanged += OnAvatarChange;
        }

        public void OnDisable()
        {
            avatarSelected.OnValueChanged -= OnAvatarChange;
        }


        private IEnumerator SetupAvatar(string avatarName)
        {
            Enumerable.Range(0, modelBase.transform.childCount)
                .Select(i => modelBase.transform.GetChild(i))
                .ToList()
                .ForEach(child =>
                {
                    GameObject.Destroy(child.gameObject);
                });
            yield return new WaitForFixedUpdate();

            GameObject avatar = avatarLibrary.DefaultAvater.avatar;
            avatar.transform.position = modelBase.transform.position;
            avatar.transform.rotation = modelBase.transform.rotation;
            if (avatarLibrary.HasCharacterAvatar(avatarName))
            {
                avatar = avatarLibrary.GetCharacterAvatar(avatarName).avatar;
            }

            modelBase.GetComponent<Animator>().avatar = null;
            yield return new WaitForFixedUpdate();
            GameObject created = GameObject.Instantiate(avatar);
            created.SetActive(false);
            created.transform.parent = modelBase.transform;
            modelBase.GetComponent<Animator>().avatar = created.GetComponent<Animator>().avatar;
            yield return new WaitForFixedUpdate();

            // Move child components to be parented by this object
            while (created.transform.childCount > 0)
            {
                GameObject child = created.transform.GetChild(0).gameObject;
                child.transform.parent = modelBase.transform;
                created.SetActive(true);
            }

            yield return new WaitForFixedUpdate();
            // Delete the created avatar base
            GameObject.Destroy(created);
            yield return new WaitForFixedUpdate();
            loading = false;
        }

        [ServerRpc]
        public void SetAvatarServerRpc(string newlySelectedAvatar)
        {
            avatarSelected.Value = newlySelectedAvatar;
            LoadNewAvatar(newlySelectedAvatar);
        }

        private bool loading = false;
        public void LoadNewAvatar(string avatarName)
        {
            if (!loading)
            {
                loading = true;
                StartCoroutine(SetupAvatar(avatarName));
            }
        }

        public void OnAvatarChange(string _, string avatarName) => LoadNewAvatar(avatarName);
    }
}