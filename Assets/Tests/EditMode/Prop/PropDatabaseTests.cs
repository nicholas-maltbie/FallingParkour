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
    public class PropDatabaseTests
    {
        PropDatabase propDatabase;

        [SetUp]
        public void Setup()
        {
            // Create a game object and setup camera follow component
            GameObject go = new GameObject();
            this.propDatabase = go.AddComponent<PropDatabase>();
            this.propDatabase.Awake();
        }

        [TearDown]
        public void TearDown()
        {
            this.propDatabase.OnDestory();
            GameObject.DestroyImmediate(this.propDatabase.gameObject);
        }

        [Test]
        public void TestPropDatabaseLoad()
        {
            // Test when loading duplicate database
            this.propDatabase.Awake();
        }
    }
}