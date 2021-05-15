using UnityEngine;

namespace PropHunt.UI
{
    /// <summary>
    /// Simple class to load screen when enabled
    /// </summary>
    public class LoadScreenOnEnable : MonoBehaviour
    {
        public GameObject selectedScreen;

        public void OnEnable()
        {
            UIManager.RequestNewScreen(this, this.selectedScreen.name);
        }
    }
}