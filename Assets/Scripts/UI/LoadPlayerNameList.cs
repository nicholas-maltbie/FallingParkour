using System.Collections.Generic;
using PropHunt.Character;
using PropHunt.Game.Communication;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI
{
    public class LoadPlayerNameList : MonoBehaviour
    {
        public Text text;

        public float updateInterval = 1.0f;

        private float elapsed = 0.0f;

        public void Start()
        {
            UpdatePlayerList();
        }

        public void Update()
        {
            elapsed += Time.deltaTime;
            if (elapsed >= updateInterval)
            {
                UpdatePlayerList();
            }
        }

        public void UpdatePlayerList()
        {
            SortedDictionary<ulong, string> playerNames = CharacterNameManagement.GetPlayerNames();
            text.text = string.Join("\n", playerNames.Values);
        }
    }
}