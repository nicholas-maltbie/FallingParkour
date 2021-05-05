using Mirror;
using PropHunt.Game.Communication;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI
{
    /// <summary>
    /// Actions to control the network manager via UI buttons
    /// </summary>
    public class NetworkActions : MonoBehaviour
    {
        /// <summary>
        /// Network service for checking network state and interactions
        /// </summary>
        public INetworkService networkService = new NetworkService(null);

        /// <summary>
        /// Network manager to control network state
        /// </summary>
        public NetworkManager manager { get; private set; }

        /// <summary>
        /// Address to connect to
        /// </summary>
        public InputField connectAddress;

        public bool Initialized()
        {
            return manager != null;
        }

        public void StartHost()
        {
            // Server + Client
            // Make sure to not host on web player, web player can't host I think...
            if (Application.platform != RuntimePlatform.WebGLPlayer && Initialized())
            {
                manager.StartHost();
                DebugChatLog.AddInfoMessage("Starting host");
            }
        }

        public void StartClient()
        {
            if (Initialized())
            {
                manager.networkAddress = connectAddress.text.Trim();
                manager.StartClient();
                DebugChatLog.AddInfoMessage("Starting client");
            }
        }

        public void StartServer()
        {
            if (Initialized())
            {
                manager.StartServer();
                DebugChatLog.AddInfoMessage("Starting server");
            }
        }

        public void StopClientConnecting()
        {
            if (Initialized() && networkService.activeNetworkClient)
            {
                manager.StopClient();
                DebugChatLog.AddInfoMessage("Ended connection attempt");
            }
        }

        public void StopClient()
        {
            if (Initialized())
            {
                if (networkService.activeNetworkServer && networkService.isConnectedNetworkClient)
                {
                    manager.StopHost();
                    DebugChatLog.AddInfoMessage("Stopping host");
                }
                // stop client if client-only
                else if (networkService.isConnectedNetworkClient)
                {
                    manager.StopClient();
                    DebugChatLog.AddInfoMessage("Disconnecting from server");
                }
                // stop server if server-only
                else if (networkService.activeNetworkServer)
                {
                    manager.StopServer();
                    DebugChatLog.AddInfoMessage("Stopping server");
                }
            }
        }

        public void Update()
        {
            // Check if manager is attached, if not find a network manager and attach it
            if (!Initialized())
            {
                manager = GameObject.FindObjectOfType<NetworkManager>();
            }
        }

    }
}