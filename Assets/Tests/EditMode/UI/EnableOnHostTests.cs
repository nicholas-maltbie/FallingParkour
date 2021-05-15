using Moq;
using NUnit.Framework;
using PropHunt.UI;
using PropHunt.Utils;
using UnityEngine;
using static PropHunt.UI.MenuController;

namespace Tests.EditMode.UI
{
    [TestFixture]
    public class EnableOnHostTests
    {
        [Test]
        public void TestOnEnableForHost()
        {
            GameObject go = new GameObject();
            EnableOnHost enableOnHost = go.AddComponent<EnableOnHost>();

            GameObject[] sampleObjects = new GameObject[3]{
                new GameObject(),
                new GameObject(),
                new GameObject()
            };

            enableOnHost.enableOnHost = sampleObjects;

            enableOnHost.OnEnable();

            Assert.IsTrue(sampleObjects[0].activeSelf == false);
            Assert.IsTrue(sampleObjects[1].activeSelf == false);
            Assert.IsTrue(sampleObjects[2].activeSelf == false);

            GameObject.DestroyImmediate(go);
            GameObject.DestroyImmediate(sampleObjects[0]);
            GameObject.DestroyImmediate(sampleObjects[1]);
            GameObject.DestroyImmediate(sampleObjects[2]);
        }
    }
}