using System.Collections;
using System.Text.RegularExpressions;
using Moq;
using NUnit.Framework;
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

        [SetUp]
        public void Setup()
        {
            // Create a game object and setup camera follow component
            GameObject go = new GameObject();
            this.propDisguise = go.AddComponent<PropDisguise>();
            this.propDisguise.disguiseBase = go.transform;
            this.propDisguise.selectedDisguise = "";
            this.propDisguise.Start();

            disguises = new Disguise[2];
            for (int i = 0; i < disguises.Length; i++)
            {
                GameObject disguise = new GameObject();
                disguise.name = "Disguise-" + i.ToString();
                Collider disguiseCollider = disguise.AddComponent<BoxCollider>();
                disguises[i] = new Disguise
                {
                    disguiseVisual = disguise,
                    disguiseCollider = disguiseCollider
                };
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
            propDisguise.selectedDisguise = "1";
            propDisguise.SetDisguise("0", "1");
            // Wait a frame for disguise to change
            yield return null;
            yield return null;
            // Assert.IsTrue(propDisguise.transform.GetChild(0).name == disguises[1].disguiseVisual.name+"(Clone)");
        }
    }
}