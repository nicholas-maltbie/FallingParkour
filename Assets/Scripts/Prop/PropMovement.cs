using Mirror;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Prop
{
    public class PropMovement : NetworkBehaviour
    {
        /// <summary>
        /// Movement speed (in units per second)
        /// </summary>
        public float movementSpeed = 2.0f;

        /// <summary>
        /// Distance to ground at which player is considered grounded
        /// </summary>
        public float groundedDistance = 0.01f;

        /// <summary>
        /// Distance to check player distance to ground
        /// </summary>
        public float groundCheckDistance = 10f;

        /// <summary>
        /// Current distance of player to ground
        /// </summary>
        public float distanceToGround = 0;

        /// <summary>
        /// Current angle walking between ground
        /// </summary>
        public float currentAngle = 0;

        /// <summary>
        /// Velocity of player when jumping
        /// </summary>
        public float jumpVelocity = 5.0f;

        /// <summary>
        /// Is the prop grounded right now
        /// </summary>
        public bool IsGrounded => distanceToGround <= groundedDistance && currentAngle <= maxWalkAngle;

        /// <summary>
        /// Mocked unity service for accessing inputs, delta time, and
        /// various other static unity inputs in a testable manner.
        /// </summary>
        public IUnityService unityService = new UnityService();

        /// <summary>
        /// Network service for managing network calls
        /// </summary>
        public INetworkService networkService;

        public float maxWalkAngle = 60f;

        public bool collidingGround;

        public void Start()
        {
            this.networkService = new NetworkService(this);
        }

        public void UpdateContactPoint(IContactPoint[] contactPoints)
        {
            foreach (IContactPoint contact in contactPoints)
            {
                float contactAngle = Vector3.Angle(Vector3.up, contact.normal);
                if (contactAngle < currentAngle)
                {
                    currentAngle = contactAngle;
                }
                if (contactAngle == 0)
                {
                    break;
                }
            }
        }

        public void OnCollisionEnter(Collision other)
        {
            collidingGround = true;
            currentAngle = 90f;
            distanceToGround = 0;
            ContactPoint[] contacts = new ContactPoint[other.contactCount];
            UpdateContactPoint(ContactPointWrapper.ConvertContactPoints(contacts));
        }

        public void OnCollisionExit(Collision other)
        {
            collidingGround = false;
            distanceToGround = 0;
        }

        public void FixedUpdate()
        {
            if (!networkService.isLocalPlayer)
            {
                // exit from update if this is not the local player
                return;
            }

            float deltaTime = unityService.deltaTime;

            Vector3 movement = new Vector3(unityService.GetAxis("Horizontal"), 0, unityService.GetAxis("Vertical"));
            // If player is not allowed to move, stop player movement
            if (PlayerInputManager.playerMovementState == PlayerInputState.Deny)
            {
                movement = Vector3.zero;
            }
            // Normalize movement vector to be a max of 1 if greater than one
            movement = movement.magnitude > 1 ? movement / movement.magnitude : movement;

            // Rotate movement vector by player yaw (rotation about vertical axis)
            Quaternion horizPlaneView = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            Vector3 playerMovementVelocity = horizPlaneView * movement * movementSpeed;

            // Get current distance to ground
            Collider collider = GetComponent<Collider>();
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            if (collider != null && rigidbody != null && !collidingGround &&
                rigidbody.SweepTest(Vector3.down, out RaycastHit hitInfo, groundCheckDistance, QueryTriggerInteraction.Ignore))
            {
                this.distanceToGround = hitInfo.distance;
                this.currentAngle = Vector3.Angle(Vector3.up, hitInfo.normal);
            }
            else if (collidingGround)
            {
                distanceToGround = 0.0f;
            }
            else
            {
                distanceToGround = Mathf.Infinity;
            }

            Vector3 movementVelocity = rigidbody.velocity;
            if (IsGrounded && unityService.GetButton("Jump"))
            {
                movementVelocity.y = jumpVelocity;
            }

            // Set player rigidbody velocity based on movement velocity
            rigidbody.velocity = movementVelocity;

            // Move rigidbody towards target
            rigidbody.MovePosition(playerMovementVelocity * unityService.deltaTime + transform.position);
        }
    }
}
