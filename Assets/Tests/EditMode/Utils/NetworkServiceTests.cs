using Mirror;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.Utils
{
    [TestFixture]
    public class NetworkServiceTests
    {
        [Test]
        public void VerifyNetworkServiceInvokeWithoutErrors()
        {
            GameObject go = new GameObject();
            NetworkBehaviour behaviour = go.AddComponent<CameraFollow>();
            INetworkService networkService = new NetworkService(behaviour);
            Assert.IsFalse(networkService.isServer);
            Assert.IsFalse(networkService.isClient);
            Assert.IsFalse(networkService.isLocalPlayer);
            Assert.IsFalse(networkService.isServerOnly);
            Assert.IsFalse(networkService.isClientOnly);
            Assert.IsFalse(networkService.hasAuthority);
            Assert.IsTrue(networkService.netId == 0u);
            Assert.IsTrue(networkService.connectionToServer == null);
            Assert.IsTrue(networkService.connectionToClient == null);
            Assert.IsTrue(networkService.activeNetworkClient == false);
            Assert.IsTrue(networkService.isConnectedNetworkClient == false);

            GameObject.DestroyImmediate(go);
        }
    }
}