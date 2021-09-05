using MLAPI;
using PropHunt.Character;
using PropHunt.Game.Communication;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI.Actions
{
    /// <summary>
    /// Actions to control the network manager via UI buttons
    /// </summary>
    public class NetworkActions : MonoBehaviour
    {
        /// <summary>
        /// Address to connect to
        /// </summary>
        public InputField connectAddress;

        public void StartHost()
        {
            UnityEngine.Debug.Log("Starting host");
            // Check if player has valid username
            if (!CharacterNameManagement.HasValidPlayerName())
            {
                DebugChatLog.AddInfoMessage("Must have valid player name must be between 2 and 16 alpha-numeric characters before starting host");
                return;
            }

            // Server + Client
            NetworkManager.Singleton.StartHost();
            DebugChatLog.AddInfoMessage("Starting host");
        }

        public void StartClient()
        {
            // Check if player has valid username
            if (!CharacterNameManagement.HasValidPlayerName())
            {
                DebugChatLog.AddInfoMessage("Must have valid player name must be between 2 and 16 alpha-numeric characters before joining game");
                return;
            }

            string networkAddress = connectAddress.text.Trim();
            NetworkManager.Singleton.StartClient();
            DebugChatLog.AddInfoMessage("Starting client");
        }

        public void StartServer()
        {
            NetworkManager.Singleton.StartServer();
            DebugChatLog.AddInfoMessage("Starting server");
        }

        public void StopClientConnecting()
        {
            NetworkManager.Singleton.StopClient();
            DebugChatLog.AddInfoMessage("Ended connection attempt");
        }

        public void StopClient()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.StopHost();
                DebugChatLog.AddInfoMessage("Stopping host");
            }
            // stop client if client-only
            else if (NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.StopClient();
                DebugChatLog.AddInfoMessage("Disconnecting from server");
            }
            // stop server if server-only
            else if (NetworkManager.Singleton.IsServer)
            {
                NetworkManager.Singleton.StopServer();
                DebugChatLog.AddInfoMessage("Stopping server");
            }
        }
    }
}