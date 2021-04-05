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
        /// Movement speed (in units per second)
        /// </summary>
        public float movementSpeed = 2.0f;

        /// <summary>
        /// Player speed modifier while sprinting (in units per second)
        /// </summary>
        public float sprintSpeed = 3.5f;

        /// <summary>
        /// Current time player has been falling
        /// </summary>
        public float fallingTime = 0.0f;

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
        /// How the character intended to move this frame. The direction the character tried to move this frame
        /// </summary>
        public Vector3 moveDirection;

        /// <summary>
        /// Parsed and normalized input of player movement on horizontal axis
        /// </summary>
        public Vector2 movementInput;

        /// <summary>
        /// Is the character sprinting this frame
        /// </summary>
        public bool isSprinting;

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
                fallingTime += unityService.deltaTime;
            }
            // If the character is grounded, set velocity to zero
            else
            {
                velocity = Vector3.zero;
                fallingTime = 0;
            }

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
            // Normalize movement vector to be a max of 1 if greater than one
            movement = movement.magnitude > 1 ? movement / movement.magnitude : movement;

            // Set player movement based on this input
            this.movementInput = new Vector2(movement.x, movement.z);

            // Read in sprinting state
            this.isSprinting = unityService.GetButton("Sprint");
            // Speed modifier based on sprinting state
            float speedModifier = this.isSprinting ? this.sprintSpeed : this.movementSpeed;

            // Rotate movement vector by player yaw (rotation about vertical axis)
            Quaternion horizPlaneView = transform.rotation;
            Vector3 movementVelocity = horizPlaneView * movement * speedModifier;
            // Set how this character intended to move this frame
            this.moveDirection = (movementVelocity + velocity);
            // Move player by displacement
            this.characterController.Move(this.moveDirection * deltaTime);
        }

        public void OnControllerColliderHit(ControllerColliderHit hit)
        {
            CharacterPush push = GetComponent<CharacterPush>();
            if (push == null)
            {
                return;
            }
            push.PushObject(new ControllerColliderHitWrapper(hit, this.moveDirection));
        }
    }
}
