using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character
{
    /// <summary>
    /// This is the character movement script. It handles moving a character
    /// that a player controls on the client.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement : NetworkBehaviour
    {
        /// <summary>
        /// Mocked unity service for accessing inputs, delta time, and
        /// various other static unity inputs in a testable manner.
        /// </summary>
        public IUnityService unityService = new UnityService();

        /// <summary>
        /// Network service for managing network calls
        /// </summary>
        public INetworkService networkService;

        /// <summary>
        /// Character controller to move character
        /// </summary>
        private CharacterController characterController;

        /// <summary>
        /// Transform holding camera position and rotation data
        /// </summary>
        public Transform cameraTransform;

        /// <summary>
        /// Movement speed (in units per second)
        /// </summary>
        public float movementSpeed = 2.0f;

        /// <summary>
        /// Direction and magnitude of gravity
        /// </summary>
        public Vector3 gravity = new Vector3(0, -9.8f, 0);

        /// <summary>
        /// Current player velocity
        /// </summary>
        private Vector3 velocity = Vector3.zero;

        /// <summary>
        /// Velocity of player jump in units per second
        /// </summary>
        public float jumpVelocity;

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
        /// How the character intended to move this frame. The direction the character tried to move this frame
        /// </summary>
        public Vector3 moveDirection;

        public void Start()
        {
            this.characterController = this.GetComponent<CharacterController>();
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

            // If the player is not grounded, increment velocity by acceleration due to gravity
            if (!characterController.isGrounded)
            {
                velocity += gravity * deltaTime;
            }
            // If the character is grounded, set velocity to zero
            else
            {
                velocity = Vector3.zero;
            }

            float yaw = transform.rotation.eulerAngles.y;
            // bound pitch between -180 and 180
            float pitch = (cameraTransform.rotation.eulerAngles.x % 360 + 180) % 360 - 180;
            // Only allow rotation if player is allowed to move
            if (PlayerInputManager.playerMovementState == PlayerInputState.Allow)
            {
                yaw += rotationRate * deltaTime * unityService.GetAxis("Mouse X");
                pitch += rotationRate * deltaTime * -1 * unityService.GetAxis("Mouse Y");
            }
            // Clamp rotation of camera between minimum and maximum specified pitch
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            // Set the player's rotation to be that of the camera's yaw
            transform.rotation = Quaternion.Euler(0, yaw, 0);
            // Set pitch to be camera's rotation
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);

            // Give the player some vertical velocity if they are jumping
            if (this.characterController.isGrounded && unityService.GetButton("Jump"))
            {
                velocity = new Vector3(0, this.jumpVelocity, 0);
            }

            // handle player input for movement (but only on local player)
            // Setup a movement vector
            // Get user input and move player if moving
            Vector3 movement = new Vector3(unityService.GetAxis("Horizontal"), 0, unityService.GetAxis("Vertical"));
            // If player is not allowed to move, stop player movement
            if (PlayerInputManager.playerMovementState == PlayerInputState.Deny)
            {
                movement = Vector3.zero;
            }

            // Rotate movement vector by player yaw (rotation about vertical axis)
            Quaternion horizPlaneView = transform.rotation;
            Vector3 movementVelocity = horizPlaneView * movement * movementSpeed;

            // Normalize movement vector to be a max of 1 if greater than one
            movement = movement.magnitude > 1 ? movement / movement.magnitude : movement;
            // Set how this character intended to move this frame
            this.moveDirection = (movementVelocity + velocity);
            // Move player by displacement
            this.characterController.Move(this.moveDirection * deltaTime);
        }
    }
}
