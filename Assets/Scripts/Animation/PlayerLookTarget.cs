
using Mirror;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Animation
{
    /// <summary>
    /// Allow for player to look in direction of camera
    /// </summary>
    [RequireComponent(typeof(NetworkIKControl))]
    [RequireComponent(typeof(CameraController))]
    public class PlayerLookTarget : NetworkBehaviour
    {
        /// <summary>
        /// Camera controller for identifying where the player is looking
        /// </summary>
        private CameraController cameraController;

        /// <summary>
        /// Network IK control for synchronizing IK information
        /// </summary>
        private NetworkIKControl networkIKControl;

        /// <summary>
        /// Network service for operating character control
        /// </summary>
        public INetworkService networkService;

        public void Awake()
        {
            this.networkService = new NetworkService(this);

            this.cameraController = GetComponent<CameraController>();
            this.networkIKControl = GetComponent<NetworkIKControl>();
        }

        public void Start()
        {
            if (networkService.isServer)
            {
                this.networkIKControl.lookState = true;
                this.networkIKControl.lookWeight = 1.0f;
            }
        }

        public void LateUpdate()
        {
            // Only configure player look target if local player
            if (!networkService.isLocalPlayer)
            {
                return;
            }

            // Set the look target to be 1 unit in front of the camera base position
            this.networkIKControl.ikLookTarget.position = transform.TransformDirection(
                cameraController.baseCameraOffset) + transform.position +
                cameraController.cameraTransform.forward;
            // With this setting, the player should always look at the correct position
            //   accounting for both the correct pitch and yaw of direction
        }
    }
}