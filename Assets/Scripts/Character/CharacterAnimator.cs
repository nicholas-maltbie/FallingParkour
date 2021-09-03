using Mirror;
using PropHunt.Character.Avatar;
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
        public float turningDeadZone = 0.1f;

        /// <summary>
        /// Amount of time falling to switch to falling animation
        /// </summary>
        public float fallingThreshold = 0.1f;

        /// <summary>
        /// Falling time before player goes into long falling animation
        /// </summary>
        public float longFallingThreshold = 1.5f;

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

            if (animator == null)
            {
                return;
            }

            bool jumping = kcc.CanJump && kcc.AttemptingJump;
            bool falling = kcc.IsProne || kcc.FallingTime >= fallingThreshold;
            bool jumpingOrFalling = falling || jumping;
            bool moving = !kcc.IsProne && !jumpingOrFalling && kcc.InputMovement.magnitude > this.movementDeadZone;

            // Set animator fields based on information
            animator.SetFloat("MoveX", kcc.InputMovement.x);
            animator.SetFloat("MoveY", kcc.InputMovement.z);
            // Set moving if movement is greater than dead zone
            animator.SetBool("Moving", moving);
            // Set turning value based on turning direction
            animator.SetFloat("Rotation", cameraController.frameRotation > 0 ? 1 : -1);
            animator.SetBool("Turning", !moving && !jumpingOrFalling && Mathf.Abs(cameraController.frameRotation) > this.turningDeadZone);
            animator.SetBool("Jumping", jumping);
            animator.SetBool("Falling", falling);
            animator.SetBool("Long Falling", kcc.FallingTime >= longFallingThreshold);
        }
    }

}
