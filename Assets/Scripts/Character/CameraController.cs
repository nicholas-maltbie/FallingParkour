
using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character
{
    public class CameraController : NetworkBehaviour
    {
        /// <summary>
        /// Network service for managing network calls
        /// </summary>
        public INetworkService networkService;

        /// <summary>
        /// Mocked unity service for accessing inputs, delta time, and
        /// various other static unity inputs in a testable manner.
        /// </summary>
        public IUnityService unityService = new UnityService();

        /// <summary>
        /// Maximum pitch for rotating character camera in degrees
        /// </summary>
        public float maxPitch = 90;

        /// <summary>
        /// Minimum pitch for rotating character camera in degrees
        /// </summary>
        public float minPitch = -90;

        /// <summary>
        /// Rotation rate of camera in degrees per second per one unit of axis movement
        /// </summary>
        public float rotationRate = 180;

        /// <summary>
        /// How much the character rotated about the vertical axis this frame
        /// </summary>
        public float frameRotation;

        /// <summary>
        /// Transform holding camera position and rotation data
        /// </summary>
        public Transform cameraTransform;

        /// <summary>
        /// Should body change it's pitch (rotation about tye horizontal axis) with camera movement
        /// </summary>
        public bool pitchBody = false;

        public void Start()
        {
            this.networkService = new NetworkService(this);
        }

        public void Update()
        {
            if (!networkService.isLocalPlayer)
            {
                // exit from update if this is not the local player
                return;
            }

            float deltaTime = unityService.deltaTime;

            float yaw = transform.rotation.eulerAngles.y;
            float yawChange = 0;
            // bound pitch between -180 and 180
            float pitch = (cameraTransform.rotation.eulerAngles.x % 360 + 180) % 360 - 180;
            // Only allow rotation if player is allowed to move
            if (PlayerInputManager.playerMovementState == PlayerInputState.Allow)
            {
                yawChange = rotationRate * deltaTime * unityService.GetAxis("Mouse X");
                yaw += yawChange;
                pitch += rotationRate * deltaTime * -1 * unityService.GetAxis("Mouse Y");
            }
            // Clamp rotation of camera between minimum and maximum specified pitch
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            frameRotation = yawChange;

            // Set the player's rotation to be that of the camera's yaw
            transform.rotation = Quaternion.Euler(pitchBody ? pitch : 0, yaw, 0);
            // Set pitch to be camera's rotation
            cameraTransform.localRotation = Quaternion.Euler(pitchBody ? 0 : pitch, 0, 0);
        }
    }
}