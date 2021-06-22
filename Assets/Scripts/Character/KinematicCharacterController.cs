using Mirror;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropHunt.Character
{
    public class KinematicCharacterController : NetworkBehaviour
    {
        /// <summary>
        /// Small offset for computing when player has stopped moving
        /// </summary>
        public const float Epsilon = 0.001f;

        /// <summary>
        /// Maximum angle between two colliding objects
        /// </summary>
        public const float MaxAngleShoveRadians = 90.0f;

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
        /// Collider cast component to abstract movement of player
        /// </summary>
        public IColliderCast colliderCast;

        [Header("Ground Checking")]

        /// <summary>
        /// Distance to ground at which player is considered grounded
        /// </summary>
        [Tooltip("Distance from ground at which a player is considered standing on the ground")]
        [SerializeField]
        public float groundedDistance = 0.01f;

        /// <summary>
        /// Distance to check player distance to ground
        /// </summary>
        [Tooltip("Distance to draw rays down when checking if player is grounded")]
        [SerializeField]
        public float groundCheckDistance = 5f;

        /// <summary>
        /// Maximum angle at which the player can walk (in degrees)
        /// </summary>
        [Tooltip("Maximum angle at which the player can walk")]
        [SerializeField]
        [Range(0, 90)]
        public float maxWalkAngle = 60f;

        /// <summary>
        /// Direction and strength of gravity
        /// </summary>
        [Tooltip("Direction and strength of gravity in units per second squared")]
        [SerializeField]
        public Vector3 gravity = new Vector3(0, -9.807f, 0);

        [Header("Motion Settings")]

        /// <summary>
        /// Speed of player movement
        /// </summary>
        [Tooltip("Speed of player when walking")]
        [SerializeField]
        public float movementSpeed = 5.0f;

        /// <summary>
        /// Speed of player when sprinting
        /// </summary>
        [Tooltip("Speed of player when sprinting")]
        [SerializeField]
        public float sprintSpeed = 7.5f;

        /// <summary>
        /// Velocity of player jump in units per second
        /// </summary>
        [Tooltip("Vertical velocity of player jump")]
        [SerializeField]
        public float jumpVelocity = 5.0f;

        /// <summary>
        /// Maximum number of time player can bounce of walls/floors/objects during an update
        /// </summary>
        [Tooltip("Maximum number of bounces when a player is moving")]
        [SerializeField]
        [Range(1, 10)]
        public int maxBounces = 5;

        /// <summary>
        /// Decay value of momentum when hitting another object.
        /// Should be between [0, 1]
        /// </summary>
        [Tooltip("Decay in momentum when hitting another object")]
        [SerializeField]
        [Range(0, 1)]
        public float pushDecay = 0.9f;

        /// <summary>
        /// Decrease in momentum factor due to angle change when walking.
        /// Should be a positive float value. It's an exponential applied to 
        /// values between [0, 1] so values smaller than 1 create a positive
        /// curve and grater than 1 for a negative curve.
        /// </summary>
        [Tooltip("Decrease in momentum when walking into objects (such as walls) at an angle as an exponential." +
        "Values between [0, 1] so values smaller than 1 create a positive curve and grater than 1 for a negative curve")]
        [SerializeField]
        public float anglePower = 0.5f;

        /// <summary>
        /// Maximum distance the player can be pushed out of overlapping objects in units per second
        /// </summary>
        [Tooltip("Maximum distance a player can be pushed when overlapping other objects in units per second")]
        [SerializeField]
        public float maxPushSpeed = 1.0f;

        /// <summary>
        /// Distance that the character can "snap down" vertical steps
        /// </summary>
        [Tooltip("Snap down distance when snapping onto the floor")]
        [SerializeField]
        public float verticalSnapDown = 0.2f;

        [Header("Stair and Step")]

        /// <summary>
        /// Minimum depth of a stair for a user to climb up
        /// (thinner steps than this value will not let the player climb)
        /// </summary>
        [Tooltip("Minimum depth of stairs when climbing up steps")]
        [SerializeField]
        public float stepUpDepth = 0.1f;

        /// <summary>
        /// Distance that the player can snap up when moving up stairs or vertical steps in terrain
        /// </summary>
        [Tooltip("Maximum height of step the player can step up")]
        [SerializeField]
        public float verticalSnapUp = 0.3f;

        /// <summary>
        /// Time in which the player can snap up or down steps even after starting to fall.
        /// This property is useful to reduce the jerky stopping and moving effects when
        /// going up or down cliffs.
        /// </summary>
        [Tooltip("Time in which the player can snap up or down steps even after starting to fall")]
        [SerializeField]
        public float snapBufferTime = 0.05f;

        [Header("Player Input")]

        /// <summary>
        /// Player's given input movement for this frame
        /// </summary>
        [Tooltip("Current input movement provided by the player")]
        [SerializeField]
        public Vector3 inputMovement;

        /// <summary>
        /// Is the player attempting to jump
        /// </summary>
        [Tooltip("Current jump input from the player")]
        [SerializeField]
        public bool attemptingJump;

        /// <summary>
        /// Is the player sprinting
        /// </summary>
        [Tooltip("Current sprinting state of the player")]
        [SerializeField]
        private bool isSprinting;

        [Header("Current Status")]

        /// <summary>
        /// How long has the player been falling
        /// </summary>
        [Tooltip("How long has the player been falling")]
        [SerializeField]
        private float elapsedFalling;

        /// <summary>
        /// Current player velocity
        /// </summary>
        [Tooltip("Current speed and direction of player motion")]
        [SerializeField]
        private Vector3 velocity;

        [Header("Current Grounded State")]

        /// <summary>
        /// Current distance the player is from the ground
        /// </summary>
        [Tooltip("Current distance the player is from the ground")]
        [SerializeField]
        public float distanceToGround;

        /// <summary>
        /// Was the player grounded this frame
        /// </summary>
        [Tooltip("Is the player grounded this frame")]
        [SerializeField]
        public bool onGround;

        /// <summary>
        /// Angle between the ground and the player
        /// </summary>
        [Tooltip("Current angle between the ground and the player")]
        [SerializeField]
        public float angle;

        /// <summary>
        /// The surface normal vector of the ground the player is standing on
        /// </summary>
        [Tooltip("Normal vector between player and ground")]
        [SerializeField]
        public Vector3 surfaceNormal;

        /// <summary>
        /// What is the player standing on
        /// </summary>
        [Tooltip("Current object the player is standing on")]
        [SerializeField]
        public GameObject floor;

        /// <summary>
        /// Amount of time that has elapsed since the player's last jump action
        /// </summary>
        private float elapsedSinceJump;

        /// <summary>
        /// Get the current player velocity
        /// </summary>
        public Vector3 Velocity => velocity;

        /// <summary>
        /// How long has the player been falling
        /// </summary>
        public float FallingTime => elapsedFalling;

        /// <summary>
        /// Is the player currently sprinting
        /// </summary>
        public bool Sprinting => isSprinting;

        /// <summary>
        /// Intended direction of movement provided by
        /// </summary>
        public Vector3 InputMovement => inputMovement;

        /// <summary>
        /// Is the player currently standing on the ground?
        /// Will be true if the hit the ground and the distance to the ground is less than
        /// the grounded threshold. NOTE, will be false if the player is overlapping with the
        /// ground or another object as it is difficult to tell whether they are stuck in a wall
        /// (and would therefore not be on the ground) versus when they are stuck in the floor.
        /// </summary>
        public bool StandingOnGround => onGround && distanceToGround <= groundedDistance && distanceToGround > 0;

        /// <summary>
        /// Is the player currently falling? this is true if they are either not standing on 
        /// the ground or if the angle between them and the ground is grater than the player's
        /// ability to walk.
        /// </summary>
        public bool Falling => !StandingOnGround || angle > maxWalkAngle;

        /// <summary>
        /// Can a player snap down this frame, a player is only allowed to snap down
        /// if they were standing on the ground this frame or was not falling within a given buffer time.
        /// Additionally, a player must have not jumped within a small buffer time in order to
        /// attempt the action of snapping down. This stops the player from teleporting into the ground right
        /// as they start to jump.
        /// </summary>
        /// <returns>If a player is allowed to snap down</returns>
        private bool CanSnapDown => (StandingOnGround || elapsedFalling <= snapBufferTime) && (elapsedSinceJump >= snapBufferTime);

        public void Start()
        {
            this.networkService = new NetworkService(this);
            this.colliderCast = GetComponent<ColliderCast>();
        }

        public void FixedUpdate()
        {
            if (!networkService.isLocalPlayer)
            {
                // exit from update if this is not the local player
                return;
            }

            float deltaTime = unityService.fixedDeltaTime;

            // If player is not allowed to move, stop player movement
            if (PlayerInputManager.playerMovementState == PlayerInputState.Deny)
            {
                inputMovement = Vector3.zero;
            }

            // Push out of overlapping objects
            PushOutOverlapping();

            // Rotate movement vector by player yaw (rotation about vertical axis)
            Quaternion horizPlaneView = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            Vector3 playerMovementDirection = horizPlaneView * inputMovement;

            Vector3 movement = playerMovementDirection * (isSprinting ? sprintSpeed : movementSpeed);

            // Update grounded state and increase velocity if falling
            CheckGrounded();
            if (!Falling && !attemptingJump)
            {
                velocity = Vector3.zero;
                this.elapsedFalling = 0.0f;
            }
            else if (Falling)
            {
                velocity += gravity * deltaTime;
                this.elapsedFalling += deltaTime;
            }

            // Give the player some vertical velocity if they are jumping and grounded
            if (!Falling && attemptingJump)
            {
                velocity = this.jumpVelocity * -gravity.normalized;
                elapsedSinceJump = 0.0f;
            }
            else
            {
                elapsedSinceJump += deltaTime;
            }

            // If the player is standing on the ground, project their movement onto the ground plane
            // This allows them to walk up gradual slopes without facing a hit in movement speed
            if (!Falling)
            {
                movement = Vector3.ProjectOnPlane(movement, surfaceNormal).normalized * movement.magnitude;
            }
            // These are broken into two steps so the player's world velocity (usually due to falling)
            //    does not interfere with their ability to walk around according to inputs
            // Move the player according to their movement
            MovePlayer(movement * deltaTime);
            // Move the player according to their world velocity
            MovePlayer(velocity * deltaTime);

            // if the player was standing on the ground at the start of the frame and is not 
            //    trying to jump right now, snap them down to the ground
            if (CanSnapDown)
            {
                SnapPlayerDown();
            }

            // make sure the rigidbody doesn't move according to velocity or angular velocity
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
        }

        /// <summary>
        /// Snap the player down onto the ground
        /// </summary>
        public void SnapPlayerDown()
        {
            // Cast current character collider down
            ColliderCastHit hit = colliderCast.CastSelf(Vector3.down, verticalSnapDown);
            if (hit.hit && hit.distance > Epsilon)
            {
                transform.position += Vector3.down * (hit.distance - Epsilon);
            }
        }

        /// <summary>
        /// Push the player out of any overlapping objects. This will constrain movement to only 
        /// pushing the player at maxPushSpeed out of overlapping objects as to not violently teleport
        /// the player when they overlap with another object.
        /// </summary>
        public void PushOutOverlapping()
        {
            float deltaTime = unityService.deltaTime;

            Collider collider = GetComponent<Collider>();

            if (collider == null)
            {
                return;
            }

            foreach (ColliderCastHit overlap in colliderCast.GetOverlappingDirectional())
            {
                Physics.ComputePenetration(
                    collider, collider.bounds.center, transform.rotation,
                    overlap.collider, overlap.collider.transform.position, overlap.collider.transform.rotation,
                    out Vector3 direction, out float distance
                );
                distance += Epsilon;
                float maxPushDistance = maxPushSpeed * unityService.deltaTime;
                if (distance > maxPushDistance)
                {
                    distance = maxPushDistance;
                }
                transform.position += direction.normalized * distance;
            }
        }

        /// <summary>
        /// Update the current grounded state of this prop class
        /// </summary>
        public void CheckGrounded()
        {
            ColliderCastHit hit = colliderCast.CastSelf(Vector3.down, groundCheckDistance);
            this.angle = Vector3.Angle(hit.normal, -gravity);
            this.distanceToGround = hit.distance;
            this.onGround = hit.hit;
            this.surfaceNormal = hit.normal;
            this.floor = hit.collider != null ? hit.collider.gameObject : null;
        }

        /// <summary>
        /// Attempt to snap the player up some distance. This will check if there
        /// is available space on the ledge above the point that the player collided with.
        /// If there is space, the player will be teleported up some distance. If
        /// there is not enough space on the ledge above, then this will move the player back to where
        /// they were before the attempt was made.
        /// </summary>
        /// <param name="distanceToSnap">Distance that the player is teleported up</param>
        /// <param name="hit">Wall/step that the player ran into</param>
        /// <param name="momentum">The remaining momentum of the player</param>
        /// <returns>True if the player had space on the ledge and was able to move, false if
        /// there was not enough room the player is moved back to their original position</returns>
        public bool AttemptSnapUp(float distanceToSnap, ColliderCastHit hit, Vector3 momentum)
        {
            // If we were to snap the player up and they moved forward, would they hit something?
            Vector3 currentPosition = transform.position;
            Vector3 snapUp = distanceToSnap * Vector3.up;
            transform.position += snapUp;

            Vector3 directionAfterSnap = Vector3.ProjectOnPlane(Vector3.Project(momentum, -hit.normal), Vector3.up).normalized * momentum.magnitude;
            ColliderCastHit snapHit = colliderCast.CastSelf(directionAfterSnap.normalized, Mathf.Max(stepUpDepth, momentum.magnitude));

            // If they can move without instantly hitting something, then snap them up
            if ((!Falling || elapsedFalling <= snapBufferTime) && snapHit.distance > Epsilon && (!snapHit.hit || snapHit.distance > stepUpDepth))
            {
                // Project rest of movement onto plane perpendicular to gravity
                transform.position = currentPosition;
                transform.position += distanceToSnap * Vector3.up;
                return true;
            }
            else
            {
                // Otherwise move the player back down
                transform.position = currentPosition;
                return false;
            }
        }

        public void MovePlayer(Vector3 movement)
        {
            // Save current momentum
            Vector3 momentum = movement;

            Collider selfCollider = GetComponent<Collider>();
            // current number of bounces
            int bounces = 0;

            // Character ability to push objects
            CharacterPush push = GetComponent<CharacterPush>();

            // Continue computing while there is momentum and bounces remaining
            while (momentum.magnitude > Epsilon && bounces <= maxBounces)
            {
                // Do a cast of the collider to see if an object is hit during this
                // movement bounce
                ColliderCastHit hit = colliderCast.CastSelf(momentum.normalized, momentum.magnitude);

                if (!hit.hit)
                {
                    // If there is no hit, move to desired position
                    transform.position += momentum;
                    // Exit as we are done bouncing
                    break;
                }

                // Apply some force to the object hit if it is moveable, Apply force on entity hit
                if (push != null && hit.collider.attachedRigidbody != null && !hit.collider.attachedRigidbody.isKinematic)
                {
                    push.PushObject(new KinematicCharacterControllerHit(
                        hit.collider, hit.collider.attachedRigidbody, hit.collider.gameObject,
                        hit.collider.transform, hit.pointHit, hit.normal, momentum.normalized, movement.magnitude
                    ));
                    // If pushing something, reduce remaining force significantly
                    momentum *= pushDecay;
                }

                // Set the fraction of remaining movement (minus some small value)
                transform.position += momentum * (hit.fraction);
                // Push slightly along normal to stop from getting caught in walls
                transform.position += hit.normal * Epsilon;
                // Decrease remaining momentum by fraction of movement remaining
                momentum *= (1 - hit.fraction);

                // Plane to project rest of movement onto
                Vector3 planeNormal = hit.normal;

                // Snap character vertically up if they hit something
                //  close enough to their feet
                float distanceToFeet = hit.pointHit.y - (transform.position - selfCollider.bounds.extents).y;
                if (hit.distance > 0 && !attemptingJump && distanceToFeet < verticalSnapUp && distanceToFeet > 0)
                {
                    // Sometimes snapping up the exact distance leads to odd behaviour around steps and walls.
                    // It's good to check the maximum and minimum snap distances and take whichever one works.
                    // Attempt to snap up the maximum vertical distance
                    if (!AttemptSnapUp(verticalSnapUp, hit, momentum))
                    {
                        // If that movement doesn't work, snap them up the minimum vertical distance
                        AttemptSnapUp(distanceToFeet + Epsilon * 2, hit, momentum);
                    }
                }
                else
                {
                    // Only apply angular change if hitting something
                    // Get angle between surface normal and remaining movement
                    float angleBetween = Vector3.Angle(hit.normal, momentum) - 90.0f;
                    // Normalize angle between to be between 0 and 1
                    // 0 means no angle, 1 means 90 degree angle
                    angleBetween = Mathf.Min(MaxAngleShoveRadians, Mathf.Abs(angleBetween));
                    float normalizedAngle = angleBetween / MaxAngleShoveRadians;
                    // Reduce the momentum by the remaining movement that ocurred
                    momentum *= Mathf.Pow(1 - normalizedAngle, anglePower);
                    // Rotate the remaining remaining movement to be projected along the plane 
                    // of the surface hit (emulate pushing against the object)
                    momentum = Vector3.ProjectOnPlane(momentum, planeNormal).normalized * momentum.magnitude;
                }

                // Track number of times the character has bounced
                bounces++;
            }
            // We're done, player was moved as part of loop
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 movement = context.ReadValue<Vector2>();
            inputMovement = new Vector3(movement.x, 0, movement.y);
            inputMovement = inputMovement.magnitude > 1 ? inputMovement / inputMovement.magnitude : inputMovement;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            attemptingJump = (PlayerInputManager.playerMovementState == PlayerInputState.Allow) && context.ReadValueAsButton();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            isSprinting = context.ReadValueAsButton();
        }
    }
}
