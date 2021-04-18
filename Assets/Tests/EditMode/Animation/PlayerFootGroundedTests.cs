using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PropHunt.Animation;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Animation
{
    [TestFixture]
    public class PlayerFootGroundedTests
    {
        PlayerFootGrounded footGrounded;

        Mock<IAnimator> animatorMock;

        GameObject floor;

        Dictionary<HumanBodyBones, Transform> boneTransforms = new Dictionary<HumanBodyBones, Transform>();
        Dictionary<AvatarIKGoal, float> boneWeights = new Dictionary<AvatarIKGoal, float>();
        Dictionary<AvatarIKGoal, float> boneRotationWeights = new Dictionary<AvatarIKGoal, float>();
        Dictionary<AvatarIKGoal, Vector3> boneGoals = new Dictionary<AvatarIKGoal, Vector3>();

        [UnitySetUp]
        public IEnumerator Setup()
        {
#if UNITY_EDITOR
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
#endif

            GameObject go = new GameObject();
            go.name = "PlayerFootGroundedObj";
            go.AddComponent<Animator>();
            this.footGrounded = go.AddComponent<PlayerFootGrounded>();
            this.footGrounded.Start();
            this.footGrounded.positionWeight = 1.0f;
            this.animatorMock = new Mock<IAnimator>();
            this.footGrounded.animator = animatorMock.Object;
            this.footGrounded.enableFootGrounded = true;
            this.floor = new GameObject();
            this.floor.transform.position = new Vector3(0, -0.5f, 0);
            this.floor.AddComponent<BoxCollider>();
            this.floor.name = "Floor";

            foreach (HumanBodyBones bone in new HumanBodyBones[]{HumanBodyBones.LeftFoot,
                HumanBodyBones.RightFoot, HumanBodyBones.LeftHand, HumanBodyBones.RightHand,
                HumanBodyBones.LeftToes, HumanBodyBones.RightToes})
            {
                boneTransforms[bone] = new GameObject().transform;
                this.animatorMock.Setup(e => e.GetBoneTransform(bone)).Returns(boneTransforms[bone]);
            }
            this.animatorMock.Setup(e => e.SetIKPositionWeight(It.IsAny<AvatarIKGoal>(), It.IsAny<float>()))
                .Callback<AvatarIKGoal, float>((goal, weight) => boneWeights[goal] = weight);
            this.animatorMock.Setup(e => e.SetIKRotationWeight(It.IsAny<AvatarIKGoal>(), It.IsAny<float>()))
                .Callback<AvatarIKGoal, float>((goal, weight) => boneRotationWeights[goal] = weight);
            this.animatorMock.Setup(e => e.SetIKPosition(It.IsAny<AvatarIKGoal>(), It.IsAny<Vector3>()))
                .Callback<AvatarIKGoal, Vector3>((goal, pos) => boneGoals[goal] = pos);

            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.footGrounded.gameObject);
            GameObject.DestroyImmediate(this.floor);
            foreach (HumanBodyBones bone in boneTransforms.Keys)
            {
                GameObject.DestroyImmediate(boneTransforms[bone].gameObject);
            }
        }

        [Test]
        public void TestFeetGrounded()
        {
            // Test when feet are grounded and toes are grounded
            boneTransforms[HumanBodyBones.LeftFoot].position = Vector3.zero;
            boneTransforms[HumanBodyBones.RightFoot].position = Vector3.zero;
            boneTransforms[HumanBodyBones.LeftToes].position = Vector3.zero;
            boneTransforms[HumanBodyBones.RightToes].position = Vector3.zero;
            this.footGrounded.OnAnimatorIK();

            Assert.IsTrue(boneWeights[AvatarIKGoal.LeftFoot] == 1.0f);
            Assert.IsTrue(boneWeights[AvatarIKGoal.RightFoot] == 1.0f);
            Assert.IsTrue(boneRotationWeights[AvatarIKGoal.LeftFoot] == 1.0f);
            Assert.IsTrue(boneRotationWeights[AvatarIKGoal.RightFoot] == 1.0f);

            // Test when feet are grounded and toes aren't grounded
            boneTransforms[HumanBodyBones.LeftFoot].position = Vector3.zero;
            boneTransforms[HumanBodyBones.RightFoot].position = Vector3.zero;
            boneTransforms[HumanBodyBones.LeftToes].position = new Vector3(10, 0, 0);
            boneTransforms[HumanBodyBones.RightToes].position = new Vector3(10, 0, 0);
            this.footGrounded.OnAnimatorIK();

            Assert.IsTrue(boneWeights[AvatarIKGoal.LeftFoot] == 1.0f);
            Assert.IsTrue(boneWeights[AvatarIKGoal.RightFoot] == 1.0f);
            Assert.IsTrue(boneRotationWeights[AvatarIKGoal.LeftFoot] == 0.0f);
            Assert.IsTrue(boneRotationWeights[AvatarIKGoal.RightFoot] == 0.0f);
        }

        [Test]
        public void TestFeetNotGrounded()
        {
            // Test when the feet aren't grounded
            boneTransforms[HumanBodyBones.LeftFoot].position = Vector3.left;
            boneTransforms[HumanBodyBones.RightFoot].position = Vector3.left;
            this.footGrounded.OnAnimatorIK();

            Assert.IsTrue(boneWeights[AvatarIKGoal.LeftFoot] == 0.0f);
            Assert.IsTrue(boneWeights[AvatarIKGoal.RightFoot] == 0.0f);
        }

        [Test]
        public void TestFeetNotMoving()
        {
            // Test when the feet aren't grounded
            boneTransforms[HumanBodyBones.LeftFoot].position = Vector3.left;
            boneTransforms[HumanBodyBones.RightFoot].position = Vector3.left;
            this.footGrounded.OnAnimatorIK();
            Assert.IsTrue(boneWeights[AvatarIKGoal.LeftFoot] == 0.0f);
            Assert.IsTrue(boneWeights[AvatarIKGoal.RightFoot] == 0.0f);

            this.footGrounded.OnAnimatorIK();
            Assert.IsTrue(boneGoals[AvatarIKGoal.LeftFoot] != Vector3.zero);
            Assert.IsTrue(boneGoals[AvatarIKGoal.RightFoot] != Vector3.zero);
        }
    }
}