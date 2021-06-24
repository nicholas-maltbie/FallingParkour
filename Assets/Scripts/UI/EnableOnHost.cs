using Mirror;
using UnityEngine;

namespace PropHunt.UI
{
    /// <summary>
    /// Simple class to enable a game object(s) when on host (or disable if not on host)
    /// </summary>
    public class EnableOnHost : MonoBehaviour, IScreenComponent
    {
        public GameObject[] enableOnHost;

        public void OnScreenLoaded()
        {
            bool isHost = NetworkServer.active;
            foreach (GameObject go in enableOnHost)
            {
                go.SetActive(isHost);
            }
        }

        public void OnScreenUnloaded()
        {

        }
    }
}