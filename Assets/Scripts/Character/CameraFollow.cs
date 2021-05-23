using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character
{
    /// <summary>
    /// Script to move main camera to follow the local player
    /// </summary>
    [RequireComponent(typeof(CameraController))]
    public class CameraFollow : NetworkBehaviour
    {
        /// <summary>
        /// Network service for managing network calls
        /// </summary>
        public INetworkService networkService;

        /// <summary>
        /// Position and rotation to control camera position and movement
        /// </summary>
        public CameraController cameraController;

        /// <summary>
        /// AudioListener for moving listening position
        /// </summary>
        public AudioListener audioListener;

        public void Start()
        {
            this.networkService = new NetworkService(this);
            this.cameraController = GetComponent<CameraController>();
            audioListener = GameObject.FindObjectOfType<AudioListener>();
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
            mainCamera.transform.rotation = cameraController.cameraTransform.rotation;
            mainCamera.transform.position = cameraController.cameraTransform.position;

            // If the camera has an audio listener, make sure to move that as well to the character's base 
            //  camera position (simulate sound coming from the character's head, not the camera position)
            if (audioListener != null)
            {
                audioListener.transform.position = cameraController.CameraSource;
                audioListener.transform.rotation = cameraController.cameraTransform.rotation;
            }
        }
    }
}