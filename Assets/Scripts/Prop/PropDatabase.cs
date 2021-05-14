using System.Collections.Generic;
using Mirror;
using PropHunt.Environment;
using UnityEngine;

namespace PropHunt.Prop
{
    /// <summary>
    /// Lookup table for all props in a scene
    /// </summary>
    public class PropDatabase : NetworkBehaviour
    {
        private Dictionary<string, Disguise> disguiseLookup = new Dictionary<string, Disguise>();

        private static PropDatabase Instance;

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // Clear out the prop database
                PropDatabase.ClearDisguises();
            }
            else
            {
                return;
            }
        }

        public void OnDestory()
        {
            ClearDisguises();
            Instance = null;
        }

        public static void ClearDisguises()
        {
            Instance.disguiseLookup.Clear();
        }

        public static Disguise GetDisguise(string name)
        {
            return Instance.disguiseLookup[name];
        }

        public static bool HasDisguise(string name)
        {
            return Instance.disguiseLookup.ContainsKey(name);
        }

        public static bool AddDisguiseIfNonExists(string name, Disguise disguise)
        {
            if (HasDisguise(name))
            {
                return false;
            }
            Instance.disguiseLookup[name] = disguise;
            return true;
        }
    }
}