using System.Collections;
using System.Text.RegularExpressions;
using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Prop;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Prop
{
    /// <summary>
    /// Tests to verify behaviour of PropDisguise script
    /// </summary>
    [TestFixture]
    public class PropDisguiseTests
    {
        PropDisguise propDisguise;

        Disguise[] disguises;

        public Disguise CreateDisguise<E>(string name) where E : Collider
        {
            GameObject disguise = new GameObject();
            disguise.name = name;
            Collider disguiseCollider = disguise.AddComponent<E>();

            return new Disguise { disguiseVisual = disguise, disguiseCollider = disguiseCollider };
        }

        [SetUp]
        public void Setup()
        {
            // Create a game object and setup camera follow component
            GameObject go = new GameObject();
            go.AddComponent<CameraController>();
            // Setup the Prop Database
            go.AddComponent<PropDatabase>().Awake();
            this.propDisguise = go.AddComponent<PropDisguise>();
            this.propDisguise.disguiseBase = go.transform;
            this.propDisguise.selectedDisguise = "";
            this.propDisguise.Start();

            disguises = new Disguise[2];
            for (int i = 0; i < disguises.Length; i++)
            {
                disguises[i] = CreateDisguise<BoxCollider>("Disguise-" + i.ToString());
                PropDatabase.AddDisguiseIfNonExists(i.ToString(), disguises[i]);
            }
            // Test adding a duplicated disguise
            PropDatabase.AddDisguiseIfNonExists("0", disguises[0]);
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.propDisguise.gameObject);
            foreach (Disguise disguise in disguises)
            {
                GameObject.DestroyImmediate(disguise.disguiseVisual);
            }
            PropDatabase.ClearDisguises();
        }

        [UnityTest]
        public IEnumerator TestChangeDisguiseOnLoad()
        {
            propDisguise.selectedDisguise = "0";
            propDisguise.Start();
            // Wait a frame for disguise to change
            yield return null;
            yield return null;
            UnityEngine.Debug.Log(propDisguise.transform.GetChild(0).name);
            Assert.IsTrue(propDisguise.transform.GetChild(0).name == disguises[0].disguiseVisual.name + "(Clone)");
        }

        [UnityTest]
        public IEnumerator TestChangeDisguiseInGame()
        {
            LogAssert.ignoreFailingMessages = true;

            propDisguise.selectedDisguise = "0";
            propDisguise.SetDisguise("", "0");
            // Wait a frame for disguise to change
            yield return null;
            yield return null;
            UnityEngine.Debug.Log(propDisguise.transform.GetChild(0).name);
            Assert.IsTrue(propDisguise.transform.GetChild(0).name == disguises[0].disguiseVisual.name + "(Clone)");
            GameObject testObject = new GameObject("testObj");
            testObject.transform.parent = propDisguise.transform;
            propDisguise.selectedDisguise = "1";
            propDisguise.SetDisguise("0", "1");
            // Wait a frame for disguise to change
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            // Assert.IsTrue(propDisguise.transform.GetChild(0).name == disguises[1].disguiseVisual.name+"(Clone)");
        }

        [UnityTest]
        public IEnumerator TestChangeDisguiseSphere()
        {
            PropDatabase.ClearDisguises();
            disguises[0] = CreateDisguise<SphereCollider>(disguises[0].disguiseCollider.gameObject.name);
            PropDatabase.AddDisguiseIfNonExists("0", disguises[0]);
            return TestChangeDisguiseInGame();
        }

        [UnityTest]
        public IEnumerator TestChangeDisguiseCapsule()
        {
            PropDatabase.ClearDisguises();
            disguises[0] = CreateDisguise<CapsuleCollider>(disguises[0].disguiseCollider.gameObject.name);
            PropDatabase.AddDisguiseIfNonExists("0", disguises[0]);
            return TestChangeDisguiseInGame();
        }

        [UnityTest]
        public IEnumerator TestChangeDisguiseMesh()
        {
            PropDatabase.ClearDisguises();
            disguises[0] = CreateDisguise<MeshCollider>(disguises[0].disguiseCollider.gameObject.name);
            PropDatabase.AddDisguiseIfNonExists("0", disguises[0]);
            return TestChangeDisguiseInGame();
        }
    }
}