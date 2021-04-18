using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Animation
{
    /// <summary>
    /// Allow for player feet to stick on ground
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerFootGrounded : MonoBehaviour
    {
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
        /// Previous position for left foot
        /// </summary>
        private Vector3 previousLeftFoot;

        /// <summary>
        /// Previous position for right foot
        /// </summary>
        private Vector3 previousRightFoot;

        public IUnityService unityService = new UnityService();

        public void Start()
        {
            animator = new AnimatorWrapper(GetComponent<Animator>());
        }

        public void OnAnimatorIK()
        {
            if (enableFootGrounded)
            {
                Transform leftFootTransform = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                Transform rightFootTransform = animator.GetBoneTransform(HumanBodyBones.RightFoot);
                Vector3 leftFoot = leftFootTransform.position;
                Vector3 rightFoot = rightFootTransform.position;

                bool leftMoving = (leftFoot - previousLeftFoot).magnitude / unityService.deltaTime >= movementThreshold;
                bool rightMoving = (rightFoot - previousRightFoot).magnitude / unityService.deltaTime >= movementThreshold;

                previousLeftFoot = leftFoot;
                previousRightFoot = rightFoot;

                bool leftHit = Physics.Raycast(leftFoot + up * (kneeHeight + footHeight), -up,
                    out RaycastHit leftFootRaycastHit, maximumFootReach + kneeHeight + footHeight);
                bool rightHit = Physics.Raycast(rightFoot + up * (kneeHeight + footHeight), -up,
                    out RaycastHit rightFootRaycastHit, maximumFootReach + kneeHeight + footHeight);
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
        }
    }
}