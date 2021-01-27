using PropHunt.UI;
using System.Collections.Generic;
using UnityEngine;

namespace PropHunt.UI
{
    public class ToggleTransportAction : MonoBehaviour
    {
        /// <summary>
        /// Set multiplayer mode
        /// </summary>
        /// <param name="mode">String name of a multiplayer game mode</param>
        public void SetMultiplayerMode(string mode)
        {
            ToggleTransport.Instance.SetMultiplayerMode(mode);
        }
    }
}