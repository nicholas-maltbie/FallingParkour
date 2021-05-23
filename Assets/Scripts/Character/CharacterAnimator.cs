using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character
{
    public class CharacterAnimator : NetworkBehaviour
    {
        /// <summary>
        /// Dead zone to consider movement as stopped
        /// </summary>
        public float movementDeadZone = 0.01f;

        /// <summary>
        /// Dead zone to consider turning action as stopped
        /// </summary>
        public float turningDeadZone = 0.01f;

        /// <summary>
        /// Amount of time falling to switch to falling animation
        /// </summary>
        public float fallingThreshold = 0.1f;

        /// <summary>
        /// Network service for managing network calls
        /// </summary>
        public INetworkService networkService;

        /// <summary>
        /// Character controller for getting character motion information
        /// </summary>
        public KinematicCharacterController kcc;

        /// <summary>
        /// Camera controller for getting player rotation information
        /// </summary>
        public CameraController cameraController;

        /// <summary>
        /// Character controller to move character
        /// </summary>
        private CharacterController characterController;

        /// <summary>
        /// Animator for controlling character
        /// </summary>
        public Animator animator;

        public void Start()
        {
            this.networkService = new NetworkService(this);
            this.characterController = this.GetComponent<CharacterController>();
        }

        public void LateUpdate()
        {
            // If local player
            if (!networkService.isLocalPlayer)
            {
                return;
            }

            bool jumping = kcc.Velocity.y >= 0.1f;
            bool falling = kcc.FallingTime >= fallingThreshold;
            bool jumpingOrFalling = jumping || falling;
            bool moving = !jumpingOrFalling && kcc.InputMovement.magnitude > this.movementDeadZone;

            // Set animator fields based on information
            animator.SetFloat("MoveX", kcc.InputMovement.x);
            animator.SetFloat("MoveY", kcc.InputMovement.z);
            // Set moving if movement is greater than dead zone
            animator.SetBool("Moving", moving);
            animator.SetBool("Sprinting", moving && kcc.Sprinting);
            // Set turning value based on turning direction
            animator.SetFloat("Rotation", cameraController.frameRotation > 0 ? 1 : -1);
            animator.SetBool("Turning", !moving && !jumpingOrFalling && Mathf.Abs(cameraController.frameRotation) > this.turningDeadZone);
            animator.SetBool("Jumping", jumping);
            animator.SetBool("Falling", falling);
        }
    }

}
