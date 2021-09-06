using MLAPI;
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
            foreach (GameObject go in enableOnHost)
            {
                go.SetActive(NetworkManager.Singleton.IsHost);
            }
        }

        public void OnScreenUnloaded()
        {

        }
    }
}