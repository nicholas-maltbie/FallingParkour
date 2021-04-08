using System.Collections;
using NUnit.Framework;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Utils
{
    /// <summary>
    /// Tests to verify behaviour of Collider cast scripts
    /// </summary>
    [TestFixture]
    public class ColliderCastTests
    {
        private GameObject go;
        private GameObject hitGo;
        private PrimitiveColliderCast primitiveColliderCast;
        private RigidbodyColliderCast rigidbodyColliderCast;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
#if UNITY_EDITOR
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
#endif
            // Setup character movement player
            this.go = new GameObject();
            this.go.AddComponent<BoxCollider>();
            this.go.AddComponent<Rigidbody>();
            this.primitiveColliderCast = this.go.AddComponent<PrimitiveColliderCast>();
            this.rigidbodyColliderCast = this.go.AddComponent<RigidbodyColliderCast>();

            this.primitiveColliderCast.layerMask = ~0;

            // Setup thing for player to look at
            hitGo = new GameObject();
            // Make it a box
            BoxCollider box = hitGo.AddComponent<BoxCollider>();
            // Move it to position (0, 0, 5), which is 5 units in front of the player
            box.transform.position = new Vector3(0, 0, 5);
            hitGo.name = "Box";

            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.go);
            GameObject.DestroyImmediate(this.hitGo);
        }

        private void VerifyOverlapping(ColliderCast cast, int minimum)
        {
            int count = 0;
            foreach (var h in cast.GetOverlappingDirectional())
            {
                count++;
            }
            Assert.IsTrue(count >= minimum);
            count = 0;
            foreach (var h in cast.GetOverlapping())
            {
                count++;
            }
            Assert.IsTrue(count >= minimum);
        }

        [Test]
        public void TestOverlappingCollider()
        {
            this.go.transform.position = this.hitGo.transform.position;
            // Simulate hitting an object
            var collision = new Collision();
            this.rigidbodyColliderCast.OnCollisionEnter(collision);
            this.VerifyOverlapping(this.primitiveColliderCast, 1);
            this.VerifyOverlapping(this.rigidbodyColliderCast, 1);
            this.rigidbodyColliderCast.OnCollisionExit(collision);

            // Try with each kind of collider
            // No collider
            GameObject.DestroyImmediate(this.go.GetComponent<Collider>());
            this.VerifyOverlapping(this.primitiveColliderCast, -1);
            // Box Collider
            this.go.AddComponent<BoxCollider>();
            this.VerifyOverlapping(this.primitiveColliderCast, 1);
            GameObject.DestroyImmediate(this.go.GetComponent<Collider>());
            // Sphere Collider
            this.go.AddComponent<SphereCollider>();
            this.VerifyOverlapping(this.primitiveColliderCast, 1);
            GameObject.DestroyImmediate(this.go.GetComponent<Collider>());
            // Capsule Collider
            this.go.AddComponent<CapsuleCollider>();
            this.VerifyOverlapping(this.primitiveColliderCast, 1);
            GameObject.DestroyImmediate(this.go.GetComponent<Collider>());
        }

        [Test]
        public void TestCastCollider()
        {
            // Test hitting the object
            ColliderCastHit hit = this.primitiveColliderCast.CastSelf(Vector3.forward, 10.0f);
            Assert.IsTrue(hit.hit == true);
            Assert.IsTrue(hit.distance > 0);
            Assert.IsTrue(hit.normal != Vector3.zero);
            Assert.IsTrue(hit.fraction == hit.distance / 10.0f);
            hit = this.rigidbodyColliderCast.CastSelf(Vector3.forward, 10.0f);
            Assert.IsTrue(hit.hit == true);
            Assert.IsTrue(hit.distance > 0);
            Assert.IsTrue(hit.normal != Vector3.zero);
            Assert.IsTrue(hit.fraction == hit.distance / 10.0f);
        }

        [Test]
        public void TestTriggerColliderBehaviour()
        {
            this.hitGo.GetComponent<Collider>().isTrigger = true;
            this.primitiveColliderCast.queryTriggerInteraction = QueryTriggerInteraction.Ignore;
            // Test hitting the object
            ColliderCastHit hit = this.primitiveColliderCast.CastSelf(Vector3.forward, 10.0f);
            Assert.IsTrue(hit.hit == false);
            this.hitGo.GetComponent<Collider>().isTrigger = false;
            hit = this.rigidbodyColliderCast.CastSelf(Vector3.forward, 10.0f);
            Assert.IsTrue(hit.hit == true);
            Assert.IsTrue(hit.distance > 0);
            Assert.IsTrue(hit.normal != Vector3.zero);
            Assert.IsTrue(hit.fraction == hit.distance / 10.0f);
            Assert.IsTrue(hit.collider.gameObject == this.hitGo);
        }

        [Test]
        public void TestGetHitsExecution()
        {
            // Test with rigidbody collider
            this.rigidbodyColliderCast.GetHits(Vector3.forward, 1.0f);
            GameObject.DestroyImmediate(this.go.GetComponent<Collider>());
            // Test with raycast, no collider
            this.primitiveColliderCast.GetHits(Vector3.forward, 1.0f);
            // Test with Box Collider
            BoxCollider boxCollider = this.go.AddComponent<BoxCollider>();
            this.primitiveColliderCast.GetHits(Vector3.forward, 1.0f);
            GameObject.DestroyImmediate(boxCollider);
            // Test with sphere collider
            SphereCollider sphereCollider = this.go.AddComponent<SphereCollider>();
            this.primitiveColliderCast.GetHits(Vector3.forward, 1.0f);
            GameObject.DestroyImmediate(sphereCollider);
            // Test with capsule collider
            CapsuleCollider capsuleCollider = this.go.AddComponent<CapsuleCollider>();
            this.primitiveColliderCast.GetHits(Vector3.forward, 1.0f);
            GameObject.DestroyImmediate(capsuleCollider);
        }
    }
}