using System;
using System.Collections;
using NUnit.Framework;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Utils
{
    [TestFixture]
    public class PhysicsUtilsTests
    {
        GameObject self;
        GameObject other;

        [UnitySetUp]
        public IEnumerator UnitySetup()
        {
#if UNITY_EDITOR
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
#endif

            self = new GameObject();
            self.AddComponent<BoxCollider>();

            other = new GameObject();
            other.AddComponent<BoxCollider>();
            other.transform.position = new Vector3(0, 0, 2);

            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(self);
            GameObject.DestroyImmediate(other);
        }

        [Test]
        public void VerifyWrapperCommands()
        {
            PhysicsUtils.RaycastFirstHitIgnore(self, Vector3.back, Vector3.forward, 10.0f, ~0,
                QueryTriggerInteraction.Ignore, out RaycastHit closest1);
            PhysicsUtils.SphereCastFirstHitIgnore(self, Vector3.back, 0.1f, Vector3.forward, 10.0f, ~0,
                QueryTriggerInteraction.Ignore, out RaycastHit closest2);
            PhysicsUtils.RaycastHitAllow(self, Vector3.back, Vector3.forward, 10.0f, ~0,
                QueryTriggerInteraction.Ignore, out RaycastHit closest4);
            PhysicsUtils.SphereCastAllow(self, Vector3.back, 0.1f, Vector3.forward, 10.0f, ~0,
                QueryTriggerInteraction.Ignore, out RaycastHit closest3);

            Assert.IsTrue(closest1.collider.gameObject == other);
            Assert.IsTrue(closest2.collider.gameObject == other);
            Assert.IsTrue(closest3.collider.gameObject == self);
            Assert.IsTrue(closest4.collider.gameObject == self);
        }
    }
}