using NUnit.Framework;
using PropHunt.Utils;
using System.Collections;
using UnityEngine;

namespace Tests.EditMode.Utils
{
    [TestFixture]
    public class CharacterControllerHitTests
    {
        [Test]
        public void VerifyKinematicCharacterControllerHit()
        {
            GameObject gameObject = new GameObject();
            KinematicCharacterControllerHit hit = new KinematicCharacterControllerHit(
                null, null,
                gameObject, gameObject.transform, Vector3.zero, Vector3.zero, Vector3.zero, 2.5f
            );

            Assert.IsTrue(hit.collider == null);
            Assert.IsTrue(hit.rigidbody == null);
            Assert.IsTrue(hit.gameObject == gameObject);
            Assert.IsTrue(hit.transform == gameObject.transform);
            Assert.IsTrue(hit.point == Vector3.zero);
            Assert.IsTrue(hit.normal == Vector3.zero);
            Assert.IsTrue(hit.moveDirection == Vector3.zero);
            Assert.IsTrue(hit.moveLength == 2.5f);

            GameObject.DestroyImmediate(gameObject);
        }
    }
}