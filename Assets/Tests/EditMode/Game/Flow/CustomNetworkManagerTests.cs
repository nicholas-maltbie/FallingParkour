

using System.Collections;
using Mirror;
using Mirror.Tests;
using Mirror.Tests.RemoteAttributeTest;
using NUnit.Framework;
using PropHunt.Game.Communication;
using PropHunt.Game.Flow;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Game.Flow
{
    [TestFixture]
    public class DebugChatCommunicationTests
    {
        CustomNetworkManager networkManager;

        [SetUp]
        public void Setup()
        {
            GameObject go = new GameObject();
            Transport.activeTransport = go.AddComponent<MemoryTransport>();
            networkManager = go.AddComponent<CustomNetworkManager>();

            networkManager.StartHost();
        }

        [TearDown]
        public void TearDown()
        {
            networkManager.StopHost();

            GameObject.DestroyImmediate(networkManager.gameObject);
        }

        [Test]
        public void TestDebugLogUpdates()
        {
            // Don't do anything here, test is ensuring setup and tear down don't create errors
        }
    }
}