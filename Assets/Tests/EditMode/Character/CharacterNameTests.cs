using Mirror.Tests.RemoteAttributeTest;
using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Utils;

namespace Tests.EditMode.Character
{
    [TestFixture]
    public class CharacterNameTests : RemoteTestBase
    {
        CharacterName characterName;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            characterName = base.CreateHostObject<CharacterName>(true);
            characterName.Awake();
            characterName.Start();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public void TestAsLocalPlayer()
        {
            Mock<INetworkService> networkServiceMock = new Mock<INetworkService>();
            networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            characterName.networkService = networkServiceMock.Object;
            characterName.Start();
        }

        [Test]
        public void TestCommandChangePlayerName()
        {
            string currentName = CharacterNameManagement.playerName;
            // Assert.IsTrue(CharacterNameManagement.playerName == characterName.characterName);

            string newName = "New Name";
            CharacterNameManagement.playerName = newName;
            characterName.CmdUpdatePlayerName(newName);
            Assert.IsTrue(CharacterNameManagement.playerName == characterName.characterName);
        }

        [Test]
        public void TestGetNameList()
        {
            var nameLookup = CharacterNameManagement.GetPlayerNames();
            Assert.IsTrue(nameLookup.Count == 1);
            Assert.IsTrue(nameLookup.ContainsValue(characterName.characterName));
        }

        [Test]
        public void VerifyNames()
        {
            Assert.IsFalse(CharacterNameManagement.VerifyName(""));
            Assert.IsFalse(CharacterNameManagement.VerifyName("ThisNameIsFarTooLong"));
            Assert.IsFalse(CharacterNameManagement.VerifyName("  "));
            Assert.IsTrue(CharacterNameManagement.VerifyName("Hello World"));
            Assert.IsTrue(CharacterNameManagement.VerifyName("Nick"));
            Assert.IsTrue(CharacterNameManagement.VerifyName("Brian 33"));
            Assert.IsTrue(CharacterNameManagement.VerifyName("Anna12"));
            Assert.IsFalse(CharacterNameManagement.VerifyName("Nick%"));
            Assert.IsTrue(CharacterNameManagement.VerifyName(CharacterNameManagement.GetFilteredName("Nick%")));
            Assert.IsTrue(CharacterNameManagement.GetFilteredName("Nick%") == "Nick");
            Assert.IsFalse(CharacterNameManagement.VerifyName("<div>Nick</div>"));
        }
    }
}