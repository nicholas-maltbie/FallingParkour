using System;
using System.Collections.Generic;
using NUnit.Framework;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.EditMode.Utils
{
    [TestFixture]
    public class ColliderWrapperTests
    {
        [Test]
        public void VerifyColliderWrapperTestsWithoutError()
        {
            Collision collision = new Collision();
            ICollision collisionWrapper = new CollisionWrapper(collision);
            Assert.IsTrue(collisionWrapper.relativeVelocity == Vector3.zero);
            Assert.IsTrue(collisionWrapper.rigidbody == null);
            Assert.IsTrue(collisionWrapper.collider == null);
            Assert.Throws<NullReferenceException>(() => collisionWrapper.transform.Translate(Vector3.zero));
            Assert.Throws<NullReferenceException>(() => collisionWrapper.gameObject.GetComponent<Transform>());
            Assert.IsTrue(collisionWrapper.contactCount == 0);
            Assert.IsTrue(collisionWrapper.contacts.Length == 0);
            Assert.IsTrue(collisionWrapper.impulse == Vector3.zero);
            Assert.Throws<ArgumentOutOfRangeException>(() => collisionWrapper.GetContact(0));
            Assert.Throws<ArgumentNullException>(() => collisionWrapper.GetContacts(new ContactPoint[0]));
            Assert.Throws<ArgumentNullException>(() => collisionWrapper.GetContacts(new List<ContactPoint>()));
        }
    }
}