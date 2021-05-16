using NUnit.Framework;
using PropHunt.Game.Flow;

namespace Tests.EditMode.Game.Flow
{
    [TestFixture]
    public class GameManagerTests : CustomNetworkManagerTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public void TestChangeStates()
        {
            GameManager.ChangePhase(GamePhase.Lobby);
            GameManager.ChangePhase(GamePhase.Setup);
            GameManager.ChangePhase(GamePhase.Setup);
        }
    }
}