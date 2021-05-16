using NUnit.Framework;
using PropHunt.Character;
using PropHunt.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Tests.EditMode.UI
{
    [TestFixture]
    public class PlayerNameUpdateTests
    {
        private InputField field;
        private PlayerNameUpdate playerNameUpdate;

        [SetUp]
        public void Setup()
        {
            GameObject go = new GameObject();
            this.playerNameUpdate = go.AddComponent<PlayerNameUpdate>();
            this.field = go.AddComponent<InputField>();
            this.playerNameUpdate.field = this.field;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.playerNameUpdate.gameObject);
        }

        [Test]
        public void TestUpdatePlayername()
        {
            this.field.text = "TestName";
            this.playerNameUpdate.UpdatePlayerName();
            Assert.IsTrue(CharacterNameManagement.playerName == "TestName");

            this.field.text = "TestName This Name Is Very Long$$$$";
            this.playerNameUpdate.UpdatePlayerName();
            Assert.IsTrue(CharacterNameManagement.playerName != this.field.text);
        }
    }
}