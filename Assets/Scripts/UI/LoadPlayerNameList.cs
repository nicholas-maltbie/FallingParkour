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

        public IUnityService unityService = new UnityService();

        public void Start()
        {
            UpdatePlayerList();
        }

        public void Update()
        {
            elapsed += unityService.deltaTime;
            if (elapsed >= updateInterval)
            {
                UpdatePlayerList();
            }
        }

        public void UpdatePlayerList()
        {
            SortedDictionary<int, string> playerNames = CharacterNameManagement.GetPlayerNames();
            text.text = string.Join("\n", playerNames.Values);
        }
    }
}