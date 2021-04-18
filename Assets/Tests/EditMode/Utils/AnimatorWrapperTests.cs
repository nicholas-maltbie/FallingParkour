using NUnit.Framework;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Utils
{
    [TestFixture]
    public class AnimatorWrapperTests
    {
        [Test]
        public void VerifyAnimatorWrapperBehaviour()
        {
            LogAssert.ignoreFailingMessages = true;
            GameObject go = new GameObject();
            AnimatorWrapper animatorWrapper = new AnimatorWrapper(go.AddComponent<Animator>());

            animatorWrapper.GetBoneTransform(HumanBodyBones.LeftHand);
            animatorWrapper.SetIKPosition(AvatarIKGoal.LeftHand, Vector3.zero);
            animatorWrapper.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            animatorWrapper.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.identity);
            animatorWrapper.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

            GameObject.DestroyImmediate(go);
        }
    }
}