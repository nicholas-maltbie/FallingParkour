using System;
using NUnit.Framework;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.EditMode.Utils
{
    [TestFixture]
    public class PhysicsUtilsTests
    {
        [Test]
        public void VerifyWrapperCommands()
        {
            GameObject ignore = new GameObject();
            PhysicsUtils.RaycastFirstHitIgnore(ignore, Vector3.zero, Vector3.forward, 2.0f, ~0,
                QueryTriggerInteraction.Ignore, out RaycastHit closest1);
            PhysicsUtils.SpherecastFirstHitIgnore(ignore, Vector3.zero, 1.0f, Vector3.forward, 2.0f, ~0,
                QueryTriggerInteraction.Ignore, out RaycastHit closest2);
            GameObject.DestroyImmediate(ignore);
        }
    }
}