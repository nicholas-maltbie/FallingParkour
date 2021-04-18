using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Animation
{
    /// <summary>
    /// Wrapper object to map the controls of Inverse Kinematics 
    /// for a humanoid character.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class IKControl : MonoBehaviour
    {
        /// <summary>
        /// Are the Inverse Kinematics controls enabled for this character
        /// </summary>
        public bool ikActive = true;

        /// <summary>
        /// Transform (position and rotation) target for the character's right hand
        /// </summary>
        public Transform rightHandTarget = null;

        /// <summary>
        /// Transform (position and rotation) target for the character's left hands
        /// </summary>
        public Transform leftHandTarget = null;

        /// <summary>
        /// Transform (position and rotation) target for the character's right foot
        /// </summary>
        public Transform rightFootTarget = null;

        /// <summary>
        /// Transform (position and rotation) target for the character's left foot
        /// </summary>
        public Transform leftFootTarget = null;

        /// <summary>
        /// Transform (position) target for the character's right knee
        /// </summary>
        public Transform rightKneeTarget = null;

        /// <summary>
        /// Transform (position) target for the character's left knee
        /// </summary>
        public Transform leftKneeTarget = null;

        /// <summary>
        /// Transform (position) target for the character's right elbow
        /// </summary>
        public Transform rightElbowTarget = null;

        /// <summary>
        /// Transform (position) target for the character's left elbow
        /// </summary>
        public Transform leftElbowTarget = null;

        /// <summary>
        /// Where is the player currently looking (just positional data)
        /// </summary>
        public Transform lookObj = null;

        /// <summary>
        /// Weight of right hand target (how much do I override current position).
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float rightHandWeight = 1.0f;

        /// <summary>
        /// Weight of the left hand target (how much do I override current position)
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float leftHandWeight = 1.0f;

        /// <summary>
        /// Weight of the right foot target (how much do I override current position)
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float rightFootweight = 1.0f;

        /// <summary>
        /// Weight of the left foot target (how much do I override current position)
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float leftFootWeight = 1.0f;

        /// <summary>
        /// Weight of overriding right elbow hint position (how much should I override current position)
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float rightElbowWeight = 0.5f;

        /// <summary>
        /// Weight of overriding the left elbow hint position (how much should I override current position)
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float leftElbowWeight = 0.5f;

        /// <summary>
        /// Weight of overriding the right knee hint position (how much should I override current position)
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float rightKneeWeight = 0.5f;

        /// <summary>
        /// Weight of overriding the left knee hint position (how much should I override current position)
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float leftKneeWeight = 0.5f;

        /// <summary>
        /// Weight of overriding the head look rotation (how much do I change it from current)
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float lookWeight = 1.0f;

        /// <summary>
        /// Animator component controlling IK
        /// </summary>
        protected Animator animator;

        public void Start()
        {
            this.animator = GetComponent<Animator>();
        }

        public void SetLookTransform(Transform transform)
        {
            this.lookObj = transform;
        }

        public void SetLookWeight(float weight)
        {
            this.lookWeight = weight;
        }

        public void SetIKHintWeight(AvatarIKHint ikHint, float weight)
        {
            switch (ikHint)
            {
                case AvatarIKHint.LeftElbow:
                    this.leftElbowWeight = weight;
                    break;
                case AvatarIKHint.RightElbow:
                    this.rightElbowWeight = weight;
                    break;
                case AvatarIKHint.LeftKnee:
                    this.leftKneeWeight = weight;
                    break;
                case AvatarIKHint.RightKnee:
                    this.rightKneeWeight = weight;
                    break;
            }
        }

        public void SetIKHintTransform(AvatarIKHint ikGoal, Transform transform)
        {
            switch (ikGoal)
            {
                case AvatarIKHint.LeftKnee:
                    this.leftKneeTarget = transform;
                    break;
                case AvatarIKHint.RightKnee:
                    this.rightKneeTarget = transform;
                    break;
                case AvatarIKHint.LeftElbow:
                    this.leftElbowTarget = transform;
                    break;
                case AvatarIKHint.RightElbow:
                    this.rightElbowTarget = transform;
                    break;
            }
        }

        public void SetIKGoalTransform(AvatarIKGoal ikGoal, Transform transform)
        {
            switch (ikGoal)
            {
                case AvatarIKGoal.LeftHand:
                    this.leftHandTarget = transform;
                    break;
                case AvatarIKGoal.RightHand:
                    this.rightHandTarget = transform;
                    break;
                case AvatarIKGoal.LeftFoot:
                    this.leftFootTarget = transform;
                    break;
                case AvatarIKGoal.RightFoot:
                    this.rightFootTarget = transform;
                    break;
            }
        }

        public void SetIKGoalWeight(AvatarIKGoal ikGoal, float weight)
        {
            switch (ikGoal)
            {
                case AvatarIKGoal.LeftHand:
                    this.leftHandWeight = weight;
                    break;
                case AvatarIKGoal.RightHand:
                    this.rightHandWeight = weight;
                    break;
                case AvatarIKGoal.LeftFoot:
                    this.leftFootWeight = weight;
                    break;
                case AvatarIKGoal.RightFoot:
                    this.rightFootweight = weight;
                    break;
            }
        }

        public void OnAnimatorIK()
        {
            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {
                // Set the look target position, if one has been assigned
                if (lookObj != null)
                {
                    animator.SetLookAtWeight(lookWeight);
                    animator.SetLookAtPosition(lookObj.position);
                }

                if (rightHandTarget != null)
                {
                    animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, rightElbowWeight);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
                }

                if (leftHandTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
                }

                if (rightFootTarget != null)
                {
                    animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, rightKneeWeight);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootweight);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootweight);
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootTarget.rotation);
                }

                if (leftFootTarget != null)
                {
                    animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, leftKneeWeight);
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootTarget.rotation);
                }

                if (leftElbowTarget != null)
                {
                    animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, leftElbowWeight);
                    animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowTarget.position);
                }
                if (rightElbowTarget != null)
                {
                    animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, rightElbowWeight);
                    animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowTarget.position);
                }
                if (leftKneeTarget != null)
                {
                    animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, leftKneeWeight);
                    animator.SetIKHintPosition(AvatarIKHint.LeftKnee, leftKneeTarget.position);
                }
                if (rightKneeTarget != null)
                {
                    animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, rightKneeWeight);
                    animator.SetIKHintPosition(AvatarIKHint.RightKnee, rightKneeTarget.position);
                }
            }

            //if the IK is not active, set the position and rotation of the targets back to the original position
            else
            {
                animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);

                animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);

                animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);

                animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);

                animator.SetLookAtWeight(0);
            }
        }
    }
}
