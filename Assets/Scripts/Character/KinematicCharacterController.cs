using System.Collections.Generic;
using System.Linq;
using Mirror;
using PropHunt.Environment;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PropHunt.Character
{
    /// <summary>
    /// Kinematic character controller to move the player character
    /// as a kinematic object
    /// </summary>
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
        /// Player collider for checking things
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

        /// <summary>
        /// Direction of down relative to gravity with a unit length of 1
        /// </summary>
        public Vector3 Down => gravity.normalized;

        /// <summary>
        /// Direction of up relative to gravity with a unit length of 1
        /// </summary>
        public Vector3 Up => -gravity.normalized;

        [Header("Motion Settings")]

        /// <summary>
        /// Speed of player movement
        /// </summary>
        [Tooltip("Speed of player when walking")]
        [SerializeField]
        public float movementSpeed = 7.5f;

        /// <summary>
        /// Velocity of player jump in units per second
        /// </summary>
        [Tooltip("Vertical velocity of player jump")]
        [SerializeField]
        public float jumpVelocity = 5.0f;

        /// <summary>
        /// Maximum angle at which the player can jump (in degrees)
        /// </summary>
        [Tooltip("Maximum angle at which the player can jump (in degrees)")]
        [SerializeField]
        [Range(0, 90)]
        public float maxJumpAngle = 85f;

        /// <summary>
        /// Weight to which the player's jump is weighted towards the direction
        /// of the surface they are standing on.
        /// </summary>
        [Tooltip("Weight to which the player's jump is weighted towards the angle of their surface")]
        [SerializeField]
        [Range(0, 1)]
        public float jumpAngleWeightFactor = 0.2f;

        /// <summary>
        /// Minimum time in seconds between player jumps
        /// </summary>
        [Tooltip("Minimum time in seconds between player jumps")]
        [SerializeField]
        public float jumpCooldown = 0.5f;

        /// <summary>
        /// Time in seconds that a player can jump after their feet leave the ground
        /// </summary>
        [Tooltip("Time in seconds that a player can jump after their feet leave the ground")]
        [SerializeField]
        public float coyoteTime = 0.25f;

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
        public bool Falling => FallingAngle(maxWalkAngle);
        public bool FallingAngle(float maxAngle) => !StandingOnGround || angle > maxAngle;

        /// <summary>
        /// Was the player grounded the previous frame.
        /// </summary>
        private bool previousGrounded;

        /// <summary>
        /// Was the player falling the previous frame.
        /// </summary>
        private bool previousFalling;

        /// <summary>
        /// The velocity of the ground the previous frame
        /// </summary>
        private Vector3 previousGroundVelocity;

        /// <summary>
        /// Object for feet to follow
        /// </summary>
        private GameObject feetFollowObj;

        /// <summary>
        /// Can the player jump right now.
        /// </summary>
        public bool CanJump => elapsedFalling >= 0 && (!FallingAngle(maxJumpAngle) || elapsedFalling <= coyoteTime) &&
            attemptingJump && elapsedSinceJump >= jumpCooldown;

        /// <summary>
        /// Can a player snap down this frame, a player is only allowed to snap down
        /// if they were standing on the ground this frame or was not falling within a given buffer time.
        /// Additionally, a player must have not jumped within a small buffer time in order to
        /// attempt the action of snapping down. This stops the player from teleporting into the ground right
        /// as they start to jump.
        /// </summary>
        /// <returns>If a player is allowed to snap down</returns>
        public bool CanSnapDown => (StandingOnGround || elapsedFalling <= snapBufferTime) && (elapsedSinceJump >= snapBufferTime);

        /// <summary>
        /// Gets transformed parameters describing this capsule collider
        /// </summary>
        public (Vector3, Vector3, float, float) GetParams()
        {
            var center = transform.TransformPoint(capsuleCollider.center);
            var radius = capsuleCollider.radius;
            var height = capsuleCollider.height;

            var bottom = center + Down * (height / 2 - radius);
            var top = center + Up * (height / 2 - radius);
            return (top, bottom, radius, height);
        }

        public void OnDestroy()
        {
            GameObject.Destroy(feetFollowObj);
        }

        public void Start()
        {
            this.networkService = new NetworkService(this);
            this.capsuleCollider = GetComponent<CapsuleCollider>();
            feetFollowObj = new GameObject();
            feetFollowObj.transform.SetParent(transform);
        }

        public void Update()
        {
            if (!networkService.isLocalPlayer)
            {
                // exit from update if this is not the local player
                return;
            }

            float deltaTime = unityService.deltaTime;

            // If player is not allowed to move, stop player movement
            if (PlayerInputManager.playerMovementState == PlayerInputState.Deny)
            {
                inputMovement = Vector3.zero;
            }

            // If we are standing on an object marked as a moving platform, move the player
            // with the moving ground object.
            MoveWithGround();

            CheckGrounded();

            PushOutOverlapping();

            // Update player velocity based on grounded state
            if (!Falling)
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
            bool jumped = PlayerJump(deltaTime);

            // Rotate movement vector by player yaw (rotation about vertical axis)
            Quaternion horizPlaneView = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            Vector3 playerMovementDirection = horizPlaneView * inputMovement;

            Vector3 movement = playerMovementDirection * movementSpeed;

            // If the player is standing on the ground, project their movement onto the ground plane
            // This allows them to walk up gradual slopes without facing a hit in movement speed
            if (!Falling)
            {
                movement = Vector3.ProjectOnPlane(movement, surfaceNormal).normalized * movement.magnitude;
            }

            // If the player was standing on the ground and is not now, increment velocity by ground
            // velocity if the player did nto jump this frame
            if (!StandingOnGround && previousGrounded && !jumped)
            {
                velocity += previousGroundVelocity;
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

            CheckGrounded();
            feetFollowObj.transform.position = transform.position;

            // Save state of player
            previousFalling = Falling;
            previousGrounded = StandingOnGround;
            previousGroundVelocity = GetGroundVelocity();

            // make sure the rigidbody doesn't move according to velocity or angular velocity
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
        }

        public IEnumerable<RaycastHit> GetHits(Vector3 direction, float distance)
        {
            (var top, var bottom, var radius, _) = GetParams();
            return Physics.CapsuleCastAll(top, bottom, radius, direction, distance, ~0, QueryTriggerInteraction.Ignore)
                .Where(hit => hit.collider.transform != transform);
        }

        public bool CastSelf(Vector3 direction, float distance, out RaycastHit hit)
        {
            RaycastHit closest = new RaycastHit() { distance = Mathf.Infinity };
            bool hitSomething = false;
            foreach (RaycastHit objHit in GetHits(direction, distance))
            {
                if (objHit.collider.gameObject.transform != gameObject.transform)
                {
                    if (objHit.distance < closest.distance)
                    {
                        closest = objHit;
                    }
                    hitSomething = true;
                }
            }

            hit = closest;
            return hitSomething;
        }

        /// <summary>
        /// Gets the velocity of the ground the player is standing on where the player is currently
        /// </summary>
        /// <returns>The velocity of the ground at the point the player is standong on</returns>
        private Vector3 GetGroundVelocity()
        {
            Vector3 groundVelocity = Vector3.zero;
            IMovingGround movingGround = floor == null ? null : floor.GetComponent<IMovingGround>();
            if (movingGround != null)
            {
                if (distanceToGround > 0)
                {
                    groundVelocity = movingGround.GetVelocityAtPoint(groundHitPosition);
                }
                else
                {
                    groundVelocity = movingGround.GetVelocityAtPoint(transform.position);
                }
            }

            return groundVelocity;
        }

        /// <summary>
        /// Give player vertical velocity if they can jump and are attempting to jump.
        /// </summary>
        /// <param name="deltaTime">Time in fixed update</param>
        /// <returns>true if the player successfully jumped, false otherwise</returns>
        public bool PlayerJump(float deltaTime)
        {
            // Give the player some vertical velocity if they are jumping and grounded
            if (CanJump)
            {
                Vector3 jumpDirection = Vector3.Lerp(StandingOnGround ? surfaceNormal : Up, Up, jumpAngleWeightFactor);
                velocity = GetGroundVelocity() + this.jumpVelocity * jumpDirection;
                elapsedSinceJump = 0.0f;
                return true;
            }
            else
            {
                elapsedSinceJump += deltaTime;
                return false;
            }
        }

        /// <summary>
        /// Move the player with the ground if possible based on the ground's
        /// velocity at a given point. 
        /// </summary>
        public void MoveWithGround()
        {
            if (feetFollowObj.transform.parent != transform)
            {
                transform.position = feetFollowObj.transform.position;
            }
            // Check if we were standing on moving ground the previous frame
            IMovingGround movingGround = floor == null ? null : floor.GetComponent<IMovingGround>();
            if (movingGround == null || Falling)
            {
                // We aren't standing on something, don't do anything
                feetFollowObj.transform.parent = transform;
                return;
            }
            // Otherwise, get the displacement of the floor at the previous position
            Vector3 displacement = movingGround.GetDisplacementAtPoint(groundHitPosition);
            // Move player by that displacement amount
            // transform.position += displacement;
            feetFollowObj.transform.parent = floor.transform;

            PushOutOverlapping(displacement.magnitude * 2);
        }

        /// <summary>
        /// Snap the player down onto the ground
        /// </summary>
        public void SnapPlayerDown()
        {
            bool didHit = CastSelf(Down, verticalSnapDown, out var hit);

            if (didHit && hit.distance > Epsilon)
            {
                transform.position += Down * (hit.distance - Epsilon * 2);
            }
        }

        /// <summary>
        /// Push the player out of any overlapping objects. This will constrain movement to only 
        /// pushing the player at maxPushSpeed out of overlapping objects as to not violently teleport
        /// the player when they overlap with another object.
        /// </summary>
        public void PushOutOverlapping()
        {
            float deltaTime = unityService.fixedDeltaTime;
            PushOutOverlapping(maxPushSpeed * deltaTime);
        }

        public void PushOutOverlapping(float maxDistance)
        {
            foreach (Collider overlap in this.GetOverlapping())
            {
                Physics.ComputePenetration(
                    capsuleCollider, transform.position, transform.rotation,
                    overlap, overlap.gameObject.transform.position, overlap.gameObject.transform.rotation,
                    out Vector3 direction, out float distance
                );
                transform.position += direction.normalized * Mathf.Min(maxDistance, distance + Epsilon);
            }
        }

        public IEnumerable<Collider> GetOverlapping()
        {
            (var top, var bottom, var radius, var height) = GetParams();
            return Physics.OverlapCapsule(top, bottom, radius, ~0, QueryTriggerInteraction.Ignore).Where(c => c.transform != transform);
        }

        /// <summary>
        /// Update the current grounded state of this kinematic character controller
        /// </summary>
        public void CheckGrounded()
        {
            bool didHit = CastSelf(Down, groundCheckDistance, out var hit);
            (var top, var bottom, var radius, var height) = GetParams();

            this.angle = Vector3.Angle(hit.normal, Up);
            this.distanceToGround = hit.distance;
            this.onGround = didHit;
            this.surfaceNormal = hit.normal;
            this.floor = hit.collider != null ? hit.collider.gameObject : null;
            this.groundHitPosition = hit.distance > 0 ? hit.point : transform.position;
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
            Vector3 snapUp = distanceToSnap * Up;
            transform.position += snapUp;

            Vector3 directionAfterSnap = Vector3.ProjectOnPlane(Vector3.Project(momentum, -hit.normal), Vector3.up).normalized * momentum.magnitude;
            bool didSnapHit = this.CastSelf(directionAfterSnap.normalized, Mathf.Max(stepUpDepth, momentum.magnitude), out RaycastHit snapHit);

            // If they can move without instantly hitting something, then snap them up
            if ((!Falling || elapsedFalling <= snapBufferTime) && snapHit.distance > Epsilon && (!didSnapHit || snapHit.distance > stepUpDepth))
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
                float distance = momentum.magnitude;
                if (!this.CastSelf(momentum.normalized, distance, out RaycastHit hit))
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

                float fraction = hit.distance / distance;
                // Set the fraction of remaining movement (minus some small value)
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
            // isSprinting = context.ReadValueAsButton();
        }
    }
}
