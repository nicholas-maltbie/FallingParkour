using System;
using NUnit.Framework;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.Utils
{
    [TestFixture]
    public class ControllerColliderHitWrapperTests
    {
        [Test]
        public void VerifyControllerColliderHitWrapperInvokeWithoutErrors()
        {
            ControllerColliderHit hit = new ControllerColliderHit();
            IControllerColliderHit hitWrapper = new ControllerColliderHitWrapper(hit);
            Assert.IsTrue(hitWrapper.controller == null);
            Assert.IsTrue(hitWrapper.collider == null);
            Assert.Throws<NullReferenceException>(() => hitWrapper.gameObject.ToString());
            Assert.Throws<NullReferenceException>(() => hitWrapper.rigidbody.ToString());
            Assert.Throws<NullReferenceException>(() => hitWrapper.transform.ToString());
            Assert.IsTrue(hitWrapper.point == Vector3.zero);
            Assert.IsTrue(hitWrapper.normal == Vector3.zero);
            Assert.IsTrue(hitWrapper.moveDirection == Vector3.zero);
            Assert.IsTrue(hitWrapper.moveLength == 0.0f);
        }
    }
}