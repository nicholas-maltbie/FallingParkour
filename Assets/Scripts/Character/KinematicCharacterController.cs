using Mirror;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;

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
        /// Distance to ground at which player is considered grounded
        /// </summary>
        public float groundedDistance = 0.01f;

        /// <summary>
        /// Distance to check player distance to ground
        /// </summary>
        public float groundCheckDistance = 10f;

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
        /// Maximum angle at which the player can walk (in degrees)
        /// </summary>
        public float maxWalkAngle = 60f;

        /// <summary>
        /// Maximum number of time player can bounce of walls/floors/objects during an update
        /// </summary>
        public int maxBounces = 5;

        /// <summary>
        /// Direction and strength of gravity
        /// </summary>
        public Vector3 gravity = new Vector3(0, -9.807f, 0);

        /// <summary>
        /// Current player velocity
        /// </summary>
        public Vector3 velocity;

        /// <summary>
        /// Velocity of player jump in units per second
        /// </summary>
        public float jumpVelocity = 5.0f;

        /// <summary>
        /// Is the player attempting to jump
        /// </summary>
        public bool attemptingJump;

        /// <summary>
        /// Maximum distance the player can be pushed out of overlapping objects in units per second
        /// </summary>
        public float maxPushSpeed = 1.0f;

        /// <summary>
        /// Decay value of momentum when hitting another object.
        /// Should be between [0, 1]
        /// </summary>
        public float pushDecay = 0.9f;

        /// <summary>
        /// Decrease in momentum factor due to angle change when walking.
        /// Should be a positive float value. It's an exponential applied to 
        /// values between [0, 1] so values smaller than 1 create a positive
        /// curve and grater than 1 for a negative curve.
        /// </summary>
        public float anglePower = 0.5f;

        /// <summary>
        /// Distance that the character can "snap down" vertical steps
        /// </summary>
        public float verticalSnapDown = 0.2f;

        /// <summary>
        /// Was the player grounded this frame
        /// </summary>
        public bool onGround;

        /// <summary>
        /// Minimum depth of a stair for a user to climb up
        /// (thinner steps than this value will not let the player climb)
        /// </summary>
        public float stepUpDepth = 0.1f;

        /// <summary>
        /// Distance that the player can snap up when moving up stairs or vertical steps in terrain
        /// </summary>
        public float verticalSnapUp = 0.3f;

        /// <summary>
        /// Current distance the player is from the ground
        /// </summary>
        public float distanceToGround;

        /// <summary>
        /// The surface normal vector of the ground the player is standing on
        /// </summary>
        public Vector3 surfaceNormal;

        /// <summary>
        /// Angle between the ground and the player
        /// </summary>
        public float angle;

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
        /// Collider cast component to abstract movement of player
        /// </summary>
        public IColliderCast colliderCast;

        /// <summary>
        /// Speed of player movement
        /// </summary>
        public float movementSpeed = 5.0f;

        /// <summary>
        /// Speed of player when sprinting
        /// </summary>
        public float sprintSpeed = 7.5f;

        /// <summary>
        /// Player's given input movement for this frame
        /// </summary>
        public Vector3 inputMovement;

        /// <summary>
        /// Is the player sprinting
        /// </summary>
        public bool isSprinting;

        /// <summary>
        /// How long has the player been falling
        /// </summary>
        public float elapsedFalling;

        public void Start()
        {
            this.networkService = new NetworkService(this);
            this.colliderCast = GetComponent<ColliderCast>();
        }

        public void Update()
        {
            if (!networkService.isLocalPlayer)
            {
                // exit from update if this is not the local player
                return;
            }
            // Get palyer input on a frame by frame basis
            inputMovement = new Vector3(unityService.GetAxis("Horizontal"), 0, unityService.GetAxis("Vertical"));
            // Normalize movement vector to be a max of 1 if greater than one
            inputMovement = inputMovement.magnitude > 1 ? inputMovement / inputMovement.magnitude : inputMovement;

            // Get other movemen inputs
            this.attemptingJump = unityService.GetButton("Jump");
            this.isSprinting = unityService.GetButton("Sprint");
        }

        public void FixedUpdate()
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
            if (StandingOnGround && !attemptingJump)
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
            if (!Falling && snapHit.distance > Epsilon && (!snapHit.hit || snapHit.distance > stepUpDepth))
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
                    float angleBetween = Vector3.Angle(hit.normal, momentum);
                    // Normalize angle between to be between 0 and 1
                    // 0 means no angle, 1 means 90 degree angle
                    angleBetween = Mathf.Min(MaxAngleShoveRadians, Mathf.Abs(angleBetween));
                    float normalizedAngle = angleBetween / MaxAngleShoveRadians;
                    // Create angle factor using 1 / (1 + normalizedAngle)
                    float angleFactor = 1.0f / (1.0f + normalizedAngle);
                    // Reduce the momentum by the remaining movement that ocurred
                    momentum *= Mathf.Pow(angleFactor, anglePower);
                    // Rotate the remaining remaining movement to be projected along the plane 
                    // of the surface hit (emulate pushing against the object)
                    momentum = Vector3.ProjectOnPlane(momentum, planeNormal).normalized * momentum.magnitude;
                }

                // Track number of times the character has bounced
                bounces++;
            }
            // We're done, player was moved as part of loop
        }
    }
}
