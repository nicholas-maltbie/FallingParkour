using System;
using NUnit.Framework;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.EditMode.Utils
{
    [TestFixture]
    public class ContactPointWrapperTests
    {
        [Test]
        public void VerifyControllerContactPointWrapperInvokeWithoutErrors()
        {
            ContactPoint contact = new ContactPoint();
            IContactPoint contactPoint = new ContactPointWrapper(contact);
            Assert.IsTrue(contactPoint.point == Vector3.zero);
            Assert.IsTrue(contactPoint.normal == Vector3.zero);
            Assert.IsTrue(contactPoint.thisCollider == null);
            Assert.IsTrue(contactPoint.otherCollider == null);
            Assert.IsTrue(contactPoint.separation == 0.0f);
        }

        [Test]
        public void VerifyConvertPoints()
        {
            ContactPoint[] points = new ContactPoint[2];
            points[0] = new ContactPoint();
            points[1] = new ContactPoint();
            ContactPointWrapper.ConvertContactPoints(points);
        }
    }
}