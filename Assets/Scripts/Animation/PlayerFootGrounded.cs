using System;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Animation
{
    /// <summary>
    /// Enum describing left and right foot
    /// </summary>
    public enum PlayerFoot
    {
        LeftFoot,
        RightFoot
    }

    /// <summary>
    /// Type of footstep when the player either goes down "Down" and sets
    /// their foot on the ground or goes "Up" and lifts their foot
    /// off of the ground
    /// </summary>
    public enum FootstepState
    {
        Down,
        Up
    }

    /// <summary>
    /// Footstep event describing whenever a player's foot sets down or lifts
    /// up from the ground.
    /// </summary>
    public class FootstepEvent : EventArgs
    {
        /// <summary>
        /// Foostep position hitting the ground
        /// </summary>
        public Vector3 footstepPosition;

        /// <summary>
        /// Which foot hit the ground
        /// </summary>
        public PlayerFoot foot;

        /// <summary>
        /// Is the player's foot being put down or lifted up
        /// </summary>
        public FootstepState state;

        /// <summary>
        /// Object the player is standing on
        /// </summary>
        public GameObject floor;

        /// <summary>
        /// Create a footstep event form the given parameters
        /// </summary>
        /// <param name="position">Foostep position hitting the ground</param>
        /// <param name="foot">Which foot hit the ground</param>
        /// <param name="state">Is the player's foot being put down or lifted up</param>
        /// <param name="floor">Object that the player's foot hit</param>
        public FootstepEvent(Vector3 position, PlayerFoot foot, FootstepState state, GameObject floor)
        {
            this.footstepPosition = position;
            this.foot = foot;
            this.state = state;
            this.floor = floor;
        }
    }

    /// <summary>
    /// Allow for player feet to stick on ground
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerFootGrounded : MonoBehaviour
    {
        /// <summary>
        /// Event for whenever a footstep event occurs
        /// </summary>
        public event EventHandler<FootstepEvent> PlayerFootstep;

        /// <summary>
        /// Animator for getting current bone positions
        /// </summary>
        public IAnimator animator;

        /// <summary>
        /// Is the behaviour of grounded feet enabled
        /// </summary>
        public bool enableFootGrounded = true;

        /// <summary>
        /// Height of player feet from the ground
        /// </summary>
        public float footHeight = 1.0f;

        /// <summary>
        /// Height of character knee
        /// </summary>
        public float kneeHeight = 1.0f;

        /// <summary>
        /// Threshold of which the feet will be snapped to the ground
        /// when a foot is standing
        /// </summary>
        public float footGroundedThreshold = 0.05f;

        /// <summary>
        /// Maximum distance a foot can reach towards the ground when not 
        /// </summary>
        public float maximumFootReach = 0.5f;

        /// <summary>
        /// Weight of rotation when rotating feet of character
        /// </summary>
        [Range(0, 1)]
        public float rotationWeight = 1f;

        /// <summary>
        /// Weight of rotation when moving feet of character
        /// </summary>
        [Range(0, 1)]
        public float positionWeight = 1f;

        /// <summary>
        /// Direction of up
        /// </summary>
        public readonly Vector3 up = Vector3.up;

        /// <summary>
        /// Threshold for considered stopped moving in units per second
        /// </summary>
        public float movementThreshold = 0.01f;

        /// <summary>
        /// Radius of foot sphere
        /// </summary>
        public float footRadius = 0.05f;

        /// <summary>
        /// Previous position for left foot
        /// </summary>
        public Vector3 leftFootPosition { get; private set; }

        /// <summary>
        /// Previous position for right foot
        /// </summary>
        public Vector3 rightFootPosition { get; private set; }

        /// <summary>
        /// Previous grounded state for left foot
        /// </summary>
        private bool previousLeftFootGrounded;

        /// <summary>
        /// Previous grounded state for right foot
        /// </summary>
        private bool previousRightFootGrounded;

        public IUnityService unityService = new UnityService();

        public void Start()
        {
            animator = new AnimatorWrapper(GetComponent<Animator>());
        }

        public void OnAnimatorIK()
        {
            Transform leftFootTransform = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            Transform rightFootTransform = animator.GetBoneTransform(HumanBodyBones.RightFoot);
            Vector3 leftFoot = leftFootTransform.position;
            Vector3 rightFoot = rightFootTransform.position;

            bool leftMoving = (leftFoot - leftFootPosition).magnitude / unityService.deltaTime >= movementThreshold;
            bool rightMoving = (rightFoot - rightFootPosition).magnitude / unityService.deltaTime >= movementThreshold;

            leftFootPosition = leftFoot;
            rightFootPosition = rightFoot;

            bool leftHit = Physics.SphereCast(leftFoot + up * (kneeHeight + footHeight), footRadius, -up,
                out RaycastHit leftFootRaycastHit, maximumFootReach + kneeHeight + footHeight - footRadius);
            bool rightHit = Physics.SphereCast(rightFoot + up * (kneeHeight + footHeight), footRadius, -up,
                out RaycastHit rightFootRaycastHit, maximumFootReach + kneeHeight + footHeight - footRadius);
            if (!leftHit)
            {
                leftFootRaycastHit.point = leftFoot + up * (kneeHeight + footHeight) -
                    up * (maximumFootReach + kneeHeight + footHeight);
            }
            if (!rightHit)
            {
                rightFootRaycastHit.point = rightFoot + up * (kneeHeight + footHeight) -
                    up * (maximumFootReach + kneeHeight + footHeight);
            }

            // UnityEngine.Debug.DrawRay(leftFoot + up * (kneeHeight), -up * (maximumFootReach + kneeHeight + footHeight), Color.red);
            // UnityEngine.Debug.DrawRay(rightFoot + up * (kneeHeight), -up * (maximumFootReach + kneeHeight + footHeight), Color.red);

            // Decide state of left foot if distance to hit <= kneeHeight + footHeight
            bool leftGrounded = leftHit && leftFootRaycastHit.distance <= kneeHeight + footHeight + footGroundedThreshold;
            bool rightGrounded = rightHit && rightFootRaycastHit.distance <= kneeHeight + footHeight + footGroundedThreshold;

            if (enableFootGrounded)
            {
                // If it is not grounded, set IK position weight to zero
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftGrounded ? positionWeight : 0);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightGrounded ? positionWeight : 0);

                // Set desired position of foot to be hit position + footHeight
                if (leftGrounded || !leftMoving)
                {
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot,
                        leftFootRaycastHit.point + Vector3.up * footHeight);
                    // If grounded, get where toes should hit
                    Vector3 leftToes = animator.GetBoneTransform(HumanBodyBones.LeftToes).position;
                    // Get the distance between toes and ground
                    bool leftToesHit = Physics.Raycast(leftToes + up * (kneeHeight), -up,
                        out RaycastHit leftToesRaycastHit, maximumFootReach + kneeHeight + footHeight);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftToesHit ? rotationWeight : 0);

                    if (!leftToesHit)
                    {
                        leftToesRaycastHit.point = leftToes + up * (kneeHeight) -
                            up * (maximumFootReach + kneeHeight + footHeight);
                    }
                    // UnityEngine.Debug.DrawLine(leftFootRaycastHit.point, leftToesRaycastHit.point, Color.blue);
                    Vector3 footVector = leftToesRaycastHit.point - leftFootRaycastHit.point;
                    Vector3 projectedVector = Vector3.ProjectOnPlane(footVector, Vector3.up);
                    Vector3 targetRotation = new Vector3(
                        Vector3.Angle(footVector, projectedVector),
                        // Mathf.Atan2(footVector.x, footVector.z) * Mathf.Rad2Deg, 0);
                        0, 0);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot,
                        Quaternion.Euler(targetRotation) * transform.rotation);
                }
                if (rightGrounded || !rightMoving)
                {
                    animator.SetIKPosition(AvatarIKGoal.RightFoot,
                        rightFootRaycastHit.point + Vector3.up * footHeight);
                    // If grounded, get where toes should hit
                    Vector3 rightToes = animator.GetBoneTransform(HumanBodyBones.RightToes).position;
                    // Get the distance between toes and ground
                    bool rightToesHit = Physics.Raycast(rightToes + up * (kneeHeight), -up,
                        out RaycastHit rightToesRaycastHit, maximumFootReach + kneeHeight + footHeight);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightToesHit ? rotationWeight : 0);
                    if (!rightToesHit)
                    {
                        rightToesRaycastHit.point = rightToes + up * (kneeHeight) -
                            up * (maximumFootReach + kneeHeight + footHeight);
                    }
                    // UnityEngine.Debug.DrawLine(rightFootRaycastHit.point, rightToesRaycastHit.point, Color.blue);
                    Vector3 footVector = rightToesRaycastHit.point - rightFootRaycastHit.point;
                    Vector3 projectedVector = Vector3.ProjectOnPlane(footVector, Vector3.up);
                    Vector3 targetRotation = new Vector3(
                        Vector3.Angle(footVector, projectedVector),
                        0, 0);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot,
                        Quaternion.Euler(targetRotation) * transform.rotation);
                }
            }

            if (leftGrounded != previousLeftFootGrounded)
            {
                PlayerFootstep?.Invoke(this, new FootstepEvent(
                    leftFootPosition, PlayerFoot.LeftFoot,
                    leftGrounded ? FootstepState.Down : FootstepState.Up,
                    leftFootRaycastHit.collider != null ? leftFootRaycastHit.collider.gameObject : null));
            }
            if (rightGrounded != previousRightFootGrounded)
            {
                PlayerFootstep?.Invoke(this, new FootstepEvent(
                    rightFootPosition, PlayerFoot.RightFoot,
                    rightGrounded ? FootstepState.Down : FootstepState.Up,
                    rightFootRaycastHit.collider != null ? rightFootRaycastHit.collider.gameObject : null));
            }

            previousLeftFootGrounded = leftGrounded;
            previousRightFootGrounded = rightGrounded;
        }
    }
}