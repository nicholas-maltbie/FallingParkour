using Mirror;
using PropHunt.Environment;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropHunt.Character
{
    [RequireComponent(typeof(CapsuleCollider))]
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
        public CapsuleCollider capsuleCollider;

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
        /// The point in which the player is hitting the ground
        /// </summary>
        [Tooltip("The point in which the player is hitting the ground")]
        [SerializeField]
        public Vector3 groundHitPosition;

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
            this.capsuleCollider = GetComponent<CapsuleCollider>();
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

            // If we are standing on an object marked as a moving platform, move the player
            // with the moving ground object.
            MoveWithGround();

            // Push out of overlapping objects
            // PushOutOverlapping();
            if (CanSnapDown)
            {
                SnapPlayerDown();
            }

            // Update grounded state and increase velocity if falling
            CheckGrounded();

            // Update player velocity based on grounded state
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

            // Compute player jump if they are attempting to jump
            PlayerJump(deltaTime);

            // Rotate movement vector by player yaw (rotation about vertical axis)
            Quaternion horizPlaneView = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            Vector3 playerMovementDirection = horizPlaneView * inputMovement;

            Vector3 movement = playerMovementDirection * (isSprinting ? sprintSpeed : movementSpeed);

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
        /// Give player vertical velocity if they can jump and are attempting to jump
        /// </summary>
        /// <param name="deltaTime">Time in fixed update</param>
        public void PlayerJump(float deltaTime)
        {
            // Give the player some vertical velocity if they are jumping and grounded
            if (!Falling && attemptingJump)
            {
                Vector3 groundVelocity = Vector3.zero;
                IMovingGround movingGround = floor == null ? null : floor.GetComponent<IMovingGround>();
                if (movingGround != null)
                {
                    groundVelocity = movingGround.GetVelocityAtPoint(groundHitPosition);
                }
                velocity = groundVelocity + this.jumpVelocity * -gravity.normalized;
                elapsedSinceJump = 0.0f;
            }
            else
            {
                elapsedSinceJump += deltaTime;
            }
        }

        /// <summary>
        /// Move the player with the ground if possible based on the ground's
        /// velocity at a given point. 
        /// </summary>
        public void MoveWithGround()
        {
            // Check if we were standing on moving ground the previous frame
            IMovingGround movingGround = floor == null ? null : floor.GetComponent<IMovingGround>();
            if (movingGround == null)
            {
                // We aren't standing on something, don't do anything
                return;
            }
            // Otherwise, get the displacement of the floor at the previous position
            Vector3 displacement = movingGround.GetDisplacementAtPoint(groundHitPosition);
            // Move player by that displacement amount
            transform.position += displacement;
        }

        /// <summary>
        /// Snap the player down onto the ground
        /// </summary>
        public void SnapPlayerDown()
        {
            // Cast sphere out of current position
            float radius = capsuleCollider.radius;
            float height = capsuleCollider.height;
            bool didHit = Physics.SphereCast(transform.position, radius, gravity.normalized, out RaycastHit hit,
                verticalSnapDown + height / 2 - radius, ~0, QueryTriggerInteraction.Ignore);

            if (didHit && hit.distance > Epsilon)
            {
                transform.position += gravity.normalized * (hit.distance - height / 2 + radius + Epsilon);
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

            Vector3 center = transform.TransformPoint(capsuleCollider.center);
            Vector3 size = transform.TransformVector(capsuleCollider.radius, capsuleCollider.height, capsuleCollider.radius);
            float radius = size.x;
            float height = size.y;
            Vector3 bottom = new Vector3(center.x, center.y - height  / 2 + radius, center.z);
            Vector3 top = new Vector3(center.x, center.y + height / 2 - radius, center.z);

            foreach (Collider overlap in Physics.OverlapCapsule(top, bottom, radius))
            {
                Physics.ComputePenetration(
                    capsuleCollider, capsuleCollider.bounds.center, transform.rotation,
                    overlap, overlap.transform.position, overlap.transform.rotation,
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
        /// Update the current grounded state of this kinematic character controller
        /// </summary>
        public void CheckGrounded()
        {
            float height = GetComponent<Collider>().bounds.extents.y;
            float radius = 0.35f;

            bool didHit = Physics.SphereCast(transform.position, radius, gravity.normalized, out RaycastHit hit,
                groundCheckDistance + height - radius, ~0, QueryTriggerInteraction.Ignore);

            this.angle = Vector3.Angle(hit.normal, -gravity);
            this.distanceToGround = Mathf.Max(Epsilon, hit.distance - height + radius);
            this.onGround = didHit;
            this.surfaceNormal = hit.normal;
            this.floor = hit.collider != null ? hit.collider.gameObject : null;
            this.groundHitPosition = hit.point;
        }

        public bool CastSelf(Vector3 direction, out RaycastHit hit, float maxDistance)
        {
            Vector3 center = transform.TransformPoint(capsuleCollider.center);
            Vector3 size = transform.TransformVector(capsuleCollider.radius, capsuleCollider.height, capsuleCollider.radius);
            float radius = size.x;
            float height = size.y;
            Vector3 bottom = new Vector3(center.x, center.y - height  / 2 + radius, center.z);
            Vector3 top = new Vector3(center.x, center.y + height / 2 - radius, center.z);

            return Physics.CapsuleCast(bottom, top, radius, direction, out hit, maxDistance);
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
        public bool AttemptSnapUp(float distanceToSnap, RaycastHit hit, Vector3 momentum)
        {
            // If we were to snap the player up and they moved forward, would they hit something?
            Vector3 currentPosition = transform.position;
            Vector3 snapUp = distanceToSnap * Vector3.up;
            transform.position += snapUp;

            Vector3 directionAfterSnap = Vector3.ProjectOnPlane(Vector3.Project(momentum, -hit.normal), Vector3.up).normalized * momentum.magnitude;
            bool didSnaphit = this.CastSelf(directionAfterSnap.normalized, out RaycastHit snapHit, Mathf.Max(stepUpDepth, momentum.magnitude));

            // If they can move without instantly hitting something, then snap them up
            if ((!Falling || elapsedFalling <= snapBufferTime) && snapHit.distance > Epsilon && (!didSnaphit || snapHit.distance > stepUpDepth))
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
                if (!this.CastSelf(momentum.normalized, out RaycastHit hit, momentum.magnitude))
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
                        hit.collider.transform, hit.point, hit.normal, momentum.normalized, movement.magnitude
                    ));
                    // If pushing something, reduce remaining force significantly
                    momentum *= pushDecay;
                }

                // Set the fraction of remaining movement (minus some small value)
                float fraction = hit.distance / momentum.normalized.magnitude;
                transform.position += momentum * (fraction);
                // Push slightly along normal to stop from getting caught in walls
                transform.position += hit.normal * Epsilon;
                // Decrease remaining momentum by fraction of movement remaining
                momentum *= (1 - fraction);

                // Plane to project rest of movement onto
                Vector3 planeNormal = hit.normal;

                // Snap character vertically up if they hit something
                //  close enough to their feet
                float distanceToFeet = hit.point.y - (transform.position - selfCollider.bounds.extents).y;
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
