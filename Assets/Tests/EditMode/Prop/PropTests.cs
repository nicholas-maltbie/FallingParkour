using Moq;
using NUnit.Framework;
using PropHunt.Prop;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.EditMode.Prop
{
    /// <summary>
    /// Tests to verify behaviour of prop script
    /// </summary>
    [TestFixture]
    public class PropTests
    {
        PropHunt.Prop.Prop testProp;

        [SetUp]
        public void Setup()
        {
            // Create a game object and setup camera follow component
            GameObject go = new GameObject();
            this.testProp = go.AddComponent<PropHunt.Prop.Prop>();
            this.testProp.disguiseVisual = go;
            this.testProp.disguiseCollider = go.AddComponent<BoxCollider>();
            this.testProp.propName = "TestProp";
            this.testProp.cameraOffset = go.transform;
            this.testProp.Start();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.testProp.gameObject);
        }

        [Test]
        public void TestPropInteract()
        {
            GameObject playerGo = new GameObject();
            this.testProp.Interact(playerGo);
            PropDisguise disguise = playerGo.AddComponent<PropDisguise>();
            this.testProp.Interact(playerGo);
            Assert.IsTrue(disguise.selectedDisguise == this.testProp.propName);
        }
    }
}