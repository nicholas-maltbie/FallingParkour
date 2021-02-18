using System.Collections;
using System.Collections.Generic;
using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Prop
{
    /// <summary>
    /// Disguise information for hiding a player
    /// </summary>
    public struct Disguise
    {
        /// <summary>
        /// Game object prefab for player disguise
        /// </summary>
        public GameObject disguiseVisual;

        /// <summary>
        /// Collider associated with the disguise
        /// </summary>
        public Collider disguiseCollider;
    }

    /// <summary>
    /// Lookup table for all props
    /// </summary>
    public static class PropDatabase
    {
        private static Dictionary<string, Disguise> disguiseLookup = new Dictionary<string, Disguise>();

        public static void ClearDisguises()
        {
            disguiseLookup.Clear();
        }

        public static Disguise GetDisguise(string name)
        {
            return disguiseLookup[name];
        }

        public static bool HasDisguise(string name)
        {
            return disguiseLookup.ContainsKey(name);
        }

        public static bool AddDisguiseIfNonExists(string name, Disguise disguise)
        {
            if (HasDisguise(name))
            {
                return false;
            }
            disguiseLookup[name] = disguise;
            return true;
        }
    }

    /// <summary>
    /// Disguise associated with a player
    /// </summary>
    public class PropDisguise : NetworkBehaviour
    {
        [SyncVar(hook = nameof(SetDisguise))]
        public string selectedDisguise;

        public Transform disguiseBase;

        public void SetDisguise(string oldDisguise, string newDisguise)
        {
            if (PropDatabase.HasDisguise(newDisguise))
            {
                Disguise disguiseInformation = PropDatabase.GetDisguise(newDisguise);
                StartCoroutine(ChangeDisguise(disguiseInformation.disguiseVisual));
                StartCoroutine(ChangeCollider(disguiseInformation.disguiseCollider));
            }
        }

        public void SetSelectedDisguise(GameObject targetProp)
        {
            Prop prop = targetProp.GetComponent<Prop>();
            if (prop != null)
            {
                selectedDisguise = prop.propName;
            }
        }

        public void Start()
        {
            if (selectedDisguise != "")
            {
                SetDisguise("", selectedDisguise);
            }
        }

        public IEnumerator ChangeCollider(Collider collider)
        {
            // Remove collider if it exists
            Collider currentCollider = gameObject.GetComponent<Collider>();
            if (currentCollider != null)
            {
                GameObject.Destroy(currentCollider);
                yield return null;
            }
            // Setup new collider data
            ComponentUtils.CopyComponent<Collider>(collider, gameObject);
        }

        public IEnumerator ChangeDisguise(GameObject disguise)
        {
            // Clear out old disguise
            while (disguiseBase.childCount > 0)
            {
                GameObject.Destroy(disguiseBase.GetChild(0).gameObject);
                yield return null;
            }

            // Setup player new disguise
            GameObject spawnedDisguise = GameObject.Instantiate(disguise, this.disguiseBase);
        }
    }
}