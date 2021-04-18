using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.EditMode.Character
{
    [TestFixture]
    public class EnableOnLocalPlayerTests
    {
        [Test]
        public void TestLocalPlayerBehaviour()
        {
            GameObject go = new GameObject();

            EnableOnLocalPlayer play = go.AddComponent<EnableOnLocalPlayer>();

            play.enableOnLocalPlayer = go;

            play.Update();
        }
    }
}