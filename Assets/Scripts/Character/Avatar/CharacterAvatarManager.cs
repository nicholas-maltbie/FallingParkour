using System.Collections;
using System.Linq;
using Mirror;
using UnityEngine;

namespace PropHunt.Character.Avatar
{
    /// <summary>
    /// Have a character controller push any dynamic rigidbody it hits
    /// </summary>
    [RequireComponent(typeof(NetworkAnimator))]
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
                CmdSetAvatar(avatarLibrary.DefaultAvater.Name);
            }
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

        [Command]
        public void CmdSetAvatar(string newlySelectedAvatar)
        {
            avatarSelected = newlySelectedAvatar;
            LoadNewAvatar(newlySelectedAvatar);
        }

        public void OnGUI()
        {
            if (isLocalPlayer)
            {
                if (GUI.Button(new Rect(10, 10, 100, 20), "Change to xbot"))
                {
                    CmdSetAvatar("xbot");
                }
                if (GUI.Button(new Rect(10, 40, 100, 20), "Change to ybot"))
                {
                    CmdSetAvatar("ybot");
                }
                if (GUI.Button(new Rect(10, 70, 100, 20), "Change to space"))
                {
                    CmdSetAvatar("SpacePerson");
                }
                if (GUI.Button(new Rect(10, 100, 100, 20), "Change to Michelle"))
                {
                    CmdSetAvatar("Michelle");
                }
            }
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