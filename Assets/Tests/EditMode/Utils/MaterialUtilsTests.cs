using System;
using System.Collections;
using NUnit.Framework;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Utils
{
    [TestFixture]
    public class MaterialUtilsTests
    {
        [Test]
        public void TestUpdateMaterialProperty()
        {
            LogAssert.ignoreFailingMessages = true;

            GameObject parent = new GameObject();
            MeshRenderer parentMR = parent.AddComponent<MeshRenderer>();
            GameObject child = new GameObject();
            MeshRenderer childMR = child.AddComponent<MeshRenderer>();

            Material parentMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            Material childMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            parentMR.materials = new Material[] { parentMat };
            childMR.materials = new Material[] { childMat };

            MaterialUtils.RecursiveSetFloatProperty(parent, "_Smoothness", 1.0f);
            // Assert.IsTrue(parentMat.GetFloat("_Smoothness") == 1.0f);
            // Assert.IsTrue(childMat.GetFloat("_Smoothness") == 1.0f);
            MaterialUtils.RecursiveSetFloatProperty(child, "_Smoothness", 5.0f);
            // Assert.IsTrue(parentMat.GetFloat("_Smoothness") == 1.0f);
            // Assert.IsTrue(childMat.GetFloat("_Smoothness") == 5.0f);

            MaterialUtils.RecursiveSetShadowCasingMode(parent, UnityEngine.Rendering.ShadowCastingMode.Off);
            // Assert.IsTrue(parentMR.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.Off);
            // Assert.IsTrue(childMR.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.Off);
            MaterialUtils.RecursiveSetShadowCasingMode(parent, UnityEngine.Rendering.ShadowCastingMode.On);
            // Assert.IsTrue(parentMR.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.On);
            // Assert.IsTrue(childMR.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.On);

        }
    }
}