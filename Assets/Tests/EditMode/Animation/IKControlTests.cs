using NUnit.Framework;
using PropHunt.Animation;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Animation
{
    [TestFixture]
    public class IKControlTests
    {
        IKControl ikControl;

        Animator animator;

        [SetUp]
        public void Setup()
        {
            GameObject go = new GameObject();
            animator = go.AddComponent<Animator>();
            this.ikControl = go.AddComponent<IKControl>();
            this.ikControl.ikActive = true;
            this.ikControl.Start();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.ikControl.gameObject);
        }

        [Test]
        public void TestIKDisabled()
        {
            this.ikControl.ikActive = false;
            this.ikControl.OnAnimatorIK();
            this.ikControl.ikActive = true;
            this.ikControl.OnAnimatorIK();
        }

        [Test]
        public void TestSetLook()
        {
            GameObject targetGo = new GameObject();
            targetGo.transform.position = Vector3.left;

            this.ikControl.SetLookTransform(targetGo.transform);
            this.ikControl.SetLookWeight(1.0f);

            // do a test ik update
            this.ikControl.OnAnimatorIK();

            Assert.IsTrue(this.ikControl.lookWeight == 1.0f);
            Assert.IsTrue(this.ikControl.lookObj == targetGo.transform);

            GameObject.DestroyImmediate(targetGo);
        }

        [Test]
        public void TestSetIKHints()
        {
            LogAssert.ignoreFailingMessages = true;
            // Test Setup
            Transform transform1 = new GameObject().transform;
            Transform transform2 = new GameObject().transform;
            Transform transform3 = new GameObject().transform;
            Transform transform4 = new GameObject().transform;
            transform1.transform.position = Vector3.left;
            transform2.transform.position = Vector3.right;
            transform3.transform.position = Vector3.up;
            transform4.transform.position = Vector3.down;

            // Set information of IK Control
            this.ikControl.SetIKHintTransform(AvatarIKHint.LeftElbow, transform1);
            this.ikControl.SetIKHintTransform(AvatarIKHint.RightElbow, transform2);
            this.ikControl.SetIKHintTransform(AvatarIKHint.LeftKnee, transform3);
            this.ikControl.SetIKHintTransform(AvatarIKHint.RightKnee, transform4);
            this.ikControl.SetIKHintWeight(AvatarIKHint.LeftElbow, 0.6f);
            this.ikControl.SetIKHintWeight(AvatarIKHint.RightElbow, 0.7f);
            this.ikControl.SetIKHintWeight(AvatarIKHint.LeftKnee, 0.8f);
            this.ikControl.SetIKHintWeight(AvatarIKHint.RightKnee, 0.9f);

            // do a test ik update
            this.ikControl.OnAnimatorIK();

            // Assert object state
            Assert.IsTrue(this.ikControl.leftElbowTarget == transform1);
            Assert.IsTrue(this.ikControl.rightElbowTarget == transform2);
            Assert.IsTrue(this.ikControl.leftKneeTarget == transform3);
            Assert.IsTrue(this.ikControl.rightKneeTarget == transform4);
            Assert.IsTrue(this.ikControl.leftElbowWeight == 0.6f);
            Assert.IsTrue(this.ikControl.rightElbowWeight == 0.7f);
            Assert.IsTrue(this.ikControl.leftKneeWeight == 0.8f);
            Assert.IsTrue(this.ikControl.rightKneeWeight == 0.9f);

            // Assert animator state
            // Assert.IsTrue(this.animator.GetIKHintPosition(AvatarIKHint.LeftElbow)  == transform1.position);
            // Assert.IsTrue(this.animator.GetIKHintPosition(AvatarIKHint.RightElbow) == transform2.position);
            // Assert.IsTrue(this.animator.GetIKHintPosition(AvatarIKHint.LeftKnee)   == transform3.position);
            // Assert.IsTrue(this.animator.GetIKHintPosition(AvatarIKHint.RightKnee)  == transform4.position);
            // Assert.IsTrue(this.animator.GetIKHintPositionWeight(AvatarIKHint.LeftElbow)  == 0.6f);
            // Assert.IsTrue(this.animator.GetIKHintPositionWeight(AvatarIKHint.RightElbow) == 0.7f);
            // Assert.IsTrue(this.animator.GetIKHintPositionWeight(AvatarIKHint.LeftKnee)   == 0.8f);
            // Assert.IsTrue(this.animator.GetIKHintPositionWeight(AvatarIKHint.RightKnee)  == 0.9f);

            // Test Cleanup
            GameObject.DestroyImmediate(transform1.gameObject);
            GameObject.DestroyImmediate(transform2.gameObject);
            GameObject.DestroyImmediate(transform3.gameObject);
            GameObject.DestroyImmediate(transform4.gameObject);
        }

        [Test]
        public void TestSetIKGoals()
        {
            LogAssert.ignoreFailingMessages = true;
            // Test Setup
            Transform transform1 = new GameObject().transform;
            Transform transform2 = new GameObject().transform;
            Transform transform3 = new GameObject().transform;
            Transform transform4 = new GameObject().transform;
            transform1.transform.position = Vector3.left;
            transform2.transform.position = Vector3.right;
            transform3.transform.position = Vector3.up;
            transform4.transform.position = Vector3.down;
            transform1.transform.rotation = Quaternion.Euler(0, 0, 1);
            transform2.transform.rotation = Quaternion.Euler(0, 1, 0);
            transform3.transform.rotation = Quaternion.Euler(0, 1, 1);
            transform4.transform.rotation = Quaternion.Euler(1, 0, 0);

            // Set information of IK Control
            this.ikControl.SetIKGoalTransform(AvatarIKGoal.LeftHand, transform1);
            this.ikControl.SetIKGoalTransform(AvatarIKGoal.RightHand, transform2);
            this.ikControl.SetIKGoalTransform(AvatarIKGoal.LeftFoot, transform3);
            this.ikControl.SetIKGoalTransform(AvatarIKGoal.RightFoot, transform4);
            this.ikControl.SetIKGoalWeight(AvatarIKGoal.LeftHand, 0.6f);
            this.ikControl.SetIKGoalWeight(AvatarIKGoal.RightHand, 0.7f);
            this.ikControl.SetIKGoalWeight(AvatarIKGoal.LeftFoot, 0.8f);
            this.ikControl.SetIKGoalWeight(AvatarIKGoal.RightFoot, 0.9f);

            // do a test ik update
            this.ikControl.OnAnimatorIK();

            // Assert object state
            Assert.IsTrue(this.ikControl.leftHandTarget == transform1);
            Assert.IsTrue(this.ikControl.rightHandTarget == transform2);
            Assert.IsTrue(this.ikControl.leftFootTarget == transform3);
            Assert.IsTrue(this.ikControl.rightFootTarget == transform4);
            Assert.IsTrue(this.ikControl.leftHandWeight == 0.6f);
            Assert.IsTrue(this.ikControl.rightHandWeight == 0.7f);
            Assert.IsTrue(this.ikControl.leftFootWeight == 0.8f);
            Assert.IsTrue(this.ikControl.rightFootweight == 0.9f);

            // Assert animator state
            // Assert.IsTrue(this.animator.GetIKPosition(AvatarIKGoal.LeftHand)  == transform1.position);
            // Assert.IsTrue(this.animator.GetIKPosition(AvatarIKGoal.RightHand) == transform2.position);
            // Assert.IsTrue(this.animator.GetIKPosition(AvatarIKGoal.LeftFoot)  == transform3.position);
            // Assert.IsTrue(this.animator.GetIKPosition(AvatarIKGoal.RightFoot) == transform4.position);
            // Assert.IsTrue(this.animator.GetIKRotation(AvatarIKGoal.LeftHand)  == transform1.rotation);
            // Assert.IsTrue(this.animator.GetIKRotation(AvatarIKGoal.RightHand) == transform2.rotation);
            // Assert.IsTrue(this.animator.GetIKRotation(AvatarIKGoal.LeftFoot)  == transform3.rotation);
            // Assert.IsTrue(this.animator.GetIKRotation(AvatarIKGoal.RightFoot) == transform4.rotation);
            // Assert.IsTrue(this.animator.GetIKPositionWeight(AvatarIKGoal.LeftHand)  == 0.6f);
            // Assert.IsTrue(this.animator.GetIKPositionWeight(AvatarIKGoal.RightHand) == 0.7f);
            // Assert.IsTrue(this.animator.GetIKPositionWeight(AvatarIKGoal.LeftFoot)  == 0.8f);
            // Assert.IsTrue(this.animator.GetIKPositionWeight(AvatarIKGoal.RightFoot) == 0.9f);
            // Assert.IsTrue(this.animator.GetIKRotationWeight(AvatarIKGoal.LeftHand)  == 0.6f);
            // Assert.IsTrue(this.animator.GetIKRotationWeight(AvatarIKGoal.RightHand) == 0.7f);
            // Assert.IsTrue(this.animator.GetIKRotationWeight(AvatarIKGoal.LeftFoot)  == 0.8f);
            // Assert.IsTrue(this.animator.GetIKRotationWeight(AvatarIKGoal.RightFoot) == 0.9f);

            // Test Cleanup
            GameObject.DestroyImmediate(transform1.gameObject);
            GameObject.DestroyImmediate(transform2.gameObject);
            GameObject.DestroyImmediate(transform3.gameObject);
            GameObject.DestroyImmediate(transform4.gameObject);
        }
    }
}