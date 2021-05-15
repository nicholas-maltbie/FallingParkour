using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.UI
{
    /// <summary>
    /// Network state for categorizing current network information
    /// </summary>
    public enum NetworkState
    {
        Unloaded,
        Offline,
        Connecting,
        Online,
        ServerOnly
    }

    /// <summary>
    /// Class to set UI Screen depending on Network State
    /// </summary>
    public class ScreenForNetworkState : MonoBehaviour
    {
        /// <summary>
        /// Screen to load when network state changes to offline
        /// </summary>
        public GameObject offlineScreen;

        /// <summary>
        /// Screen to load when network state changes to connecting
        /// </summary>
        public GameObject connectingScreen;

        /// <summary>
        /// Screen to load when network state changes to online
        /// </summary>
        public GameObject onlineScreen;

        /// <summary>
        /// Screen to load when running only a server
        /// </summary>
        public GameObject serverScreen;

        /// <summary>
        /// Network service to detect changes in network state
        /// </summary>
        public INetworkService networkService = new NetworkService(null);

        /// <summary>
        /// Previous network state for detecting changes in network infomation
        /// </summary>
        private NetworkState previousNetworkState = NetworkState.Unloaded;

        /// <summary>
        /// Get the current network state based on this object's network service
        /// </summary>
        /// <returns>Network state based on the network client information</returns>
        public NetworkState GetCurrentNetworkState()
        {
            if (!networkService.activeNetworkClient && !networkService.activeNetworkServer)
            {
                return NetworkState.Offline;
            }
            else if (networkService.activeNetworkClient && !networkService.isConnectedNetworkClient)
            {
                return NetworkState.Connecting;
            }
            else if (networkService.activeNetworkClient && networkService.isConnectedNetworkClient)
            {
                return NetworkState.Online;
            }
            else // if (networkService.activeNetworkServer)
            {
                return NetworkState.ServerOnly;
            }
        }

        public void Update()
        {
            NetworkState currentState = GetCurrentNetworkState();

            // only on state changes
            if (currentState != previousNetworkState)
            {
                if (currentState == NetworkState.Offline && this.offlineScreen != null)
                {
                    UIManager.RequestNewScreen(this, this.offlineScreen.name);
                }
                else if (currentState == NetworkState.Connecting && this.connectingScreen != null)
                {
                    UIManager.RequestNewScreen(this, this.connectingScreen.name);
                }
                else if (currentState == NetworkState.Online && this.onlineScreen != null)
                {
                    UIManager.RequestNewScreen(this, this.onlineScreen.name);
                }
                else if (currentState == NetworkState.ServerOnly && this.serverScreen != null)
                {
                    UIManager.RequestNewScreen(this, this.serverScreen.name);
                }
            }

            this.previousNetworkState = currentState;
        }
    }
}