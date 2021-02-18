using System;
using NUnit.Framework;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.EditMode.Utils
{
    public class TestComponent : MonoBehaviour
    {
        public string field;
    }

    [TestFixture]
    public class ComponentUtilsTests
    {
        [Test]
        public void VerifyCopyComponent()
        {
            GameObject source = new GameObject();
            TestComponent sourceComponent = source.AddComponent<TestComponent>();
            sourceComponent.field = "Apples";
            GameObject target = new GameObject();
            ComponentUtils.CopyComponent<TestComponent>(sourceComponent, target);
            TestComponent targetComponent = source.GetComponent<TestComponent>();
            Assert.IsTrue(targetComponent.field == sourceComponent.field);
        }
    }
}