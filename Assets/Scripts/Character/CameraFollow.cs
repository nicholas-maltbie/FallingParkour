using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character
{
    /// <summary>
    /// Script to move main camera to follow the local player
    /// </summary>
    public class CameraFollow : NetworkBehaviour
    {
        /// <summary>
        /// Network service for managing network calls
        /// </summary>
        public INetworkService networkService;

        /// <summary>
        /// Position and rotation to move camera to when following player
        /// </summary>
        public Transform cameraTransform;

        public void Start()
        {
            this.networkService = new NetworkService(this);
        }

        public void LateUpdate()
        {
            if (!this.networkService.isLocalPlayer)
            {
                // exit from update if this is not the local player
                return;
            }

            // Do nothing if there is no main camera
            if (Camera.main == null)
            {
                return;
            }

            // Set main camera's parent to be this and set it's relative position and rotation to be zero
            GameObject mainCamera = Camera.main.gameObject;
            mainCamera.transform.rotation = cameraTransform.rotation;
            mainCamera.transform.position = cameraTransform.position;
        }
    }
}