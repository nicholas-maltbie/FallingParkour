
using UnityEngine;

namespace PropHunt.Utils
{
    public interface IAnimator
    {
        Transform GetBoneTransform(HumanBodyBones bones);
        void SetIKPosition(AvatarIKGoal goal, Vector3 position);
        void SetIKPositionWeight(AvatarIKGoal goal, float weight);
        void SetIKRotation(AvatarIKGoal goal, Quaternion rotation);
        void SetIKRotationWeight(AvatarIKGoal goal, float weight);
    }

    public class AnimatorWrapper : IAnimator
    {
        private Animator animator;

        public AnimatorWrapper(Animator animator)
        {
            this.animator = animator;
        }

        public Transform GetBoneTransform(HumanBodyBones bones) =>
            animator.GetBoneTransform(bones);
        public void SetIKPosition(AvatarIKGoal goal, Vector3 position) =>
            animator.SetIKPosition(goal, position);
        public void SetIKPositionWeight(AvatarIKGoal goal, float weight) =>
            animator.SetIKPositionWeight(goal, weight);
        public void SetIKRotation(AvatarIKGoal goal, Quaternion rotation) =>
            animator.SetIKRotation(goal, rotation);
        public void SetIKRotationWeight(AvatarIKGoal goal, float weight) =>
            animator.SetIKRotationWeight(goal, weight);
    }
}
