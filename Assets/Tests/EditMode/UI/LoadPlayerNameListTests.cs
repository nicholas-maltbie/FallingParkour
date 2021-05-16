using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.UI;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Tests.EditMode.UI
{
    [TestFixture]
    public class LoadPlayerNameListTests
    {
        private LoadPlayerNameList characterNameList;
        private Text text;
        private Mock<IUnityService> unityServiceMock;

        private List<CharacterName> characterNames;

        [SetUp]
        public void Setup()
        {
            GameObject go = new GameObject();
            this.characterNameList = go.AddComponent<LoadPlayerNameList>();
            this.text = go.AddComponent<Text>();
            this.characterNameList.text = this.text;

            this.characterNames = new List<CharacterName>();
            this.characterNames.Add(new GameObject().AddComponent<CharacterName>());
            this.characterNames.Add(new GameObject().AddComponent<CharacterName>());
            this.characterNames.Add(new GameObject().AddComponent<CharacterName>());
            this.characterNames[0].characterName = "Name1";
            this.characterNames[0].characterName = "Name2";
            this.characterNames[0].characterName = "Name3";

            this.characterNameList.Start();

            this.unityServiceMock = new Mock<IUnityService>();
            this.characterNameList.unityService = this.unityServiceMock.Object;

        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.characterNameList.gameObject);
            foreach (CharacterName name in this.characterNames)
            {
                GameObject.DestroyImmediate(name.gameObject);
            }
        }

        [Test]
        public void TestAutoUpdate()
        {
            this.unityServiceMock.Setup(e => e.deltaTime).Returns(1f);
            this.characterNameList.updateInterval = 2.0f;

            this.text.text = "";

            this.characterNameList.Update();
            Assert.IsTrue(text.text == "");
            this.characterNameList.Update();
            Assert.IsTrue(text.text != "");
        }
    }
}